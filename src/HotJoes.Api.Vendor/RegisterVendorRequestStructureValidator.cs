using System.Globalization;
using System.Text.Json;

namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorRequestStructureValidator
{
    private static readonly HashSet<string> ProhibitedMemberNames =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "canonicalAddressId",
            "businessAddressSnapshot",
            "foodRegistrationAuthority",
            "primaryTradingAuthority",
            "vendorId",
            "vendorState",
            "registeredAt",
            "registrationIdentity",
            "semanticFingerprint",
            "outbox",
            "registrationSession"
        };

    private static readonly HashSet<string> LegalOperatorTypes =
        new(StringComparer.Ordinal)
        {
            "soleTrader",
            "generalPartnership",
            "limitedCompany",
            "limitedLiabilityPartnership",
            "charitableCommunityGroup",
            "charitableIncorporatedOrganisation"
        };

    private static readonly HashSet<string> TradingLocations =
        new(StringComparer.Ordinal)
        {
            "restaurant",
            "stall",
            "kitchen"
        };

    public bool IsValid(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object || ContainsProhibitedMember(root))
        {
            return false;
        }

        return IsRequiredString(root, "tradingName")
            && IsRequiredString(root, "legalOperatorName")
            && IsRequiredControlledString(
                root,
                "legalOperatorType",
                LegalOperatorTypes)
            && IsOptionalString(root, "companyRegistrationNumber")
            && IsTradingCharacteristicsValid(root)
            && IsPrimaryContactValid(root)
            && IsRequiredString(root, "addressResolutionReference")
            && IsOptionalString(root, "website")
            && IsOptionalString(root, "businessDescription")
            && AreRegistrationDeclarationsValid(root);
    }

    private static bool IsTradingCharacteristicsValid(JsonElement root)
    {
        return TryGetObject(root, "tradingCharacteristics", out JsonElement trading)
            && IsRequiredControlledString(
                trading,
                "tradingLocation",
                TradingLocations)
            && TryGetObject(trading, "openingHours", out JsonElement hours)
            && IsRequiredTime(hours, "startTime")
            && IsRequiredTime(hours, "endTime")
            && IsRequiredBoolean(trading, "serviceIncludesHotFood")
            && IsRequiredBoolean(trading, "alcoholService");
    }

    private static bool IsPrimaryContactValid(JsonElement root)
    {
        return TryGetObject(root, "primaryContact", out JsonElement contact)
            && IsRequiredString(contact, "contactName")
            && IsRequiredString(contact, "contactEmail")
            && IsRequiredString(contact, "contactTelephone");
    }

    private static bool AreRegistrationDeclarationsValid(JsonElement root)
    {
        return TryGetObject(
                root,
                "registrationDeclarations",
                out JsonElement declarations)
            && IsRequiredBoolean(declarations, "authorisedToRegisterBusiness")
            && IsRequiredBoolean(declarations, "informationAccurate")
            && IsRequiredBoolean(declarations, "acceptHotJoesPlatformTerms");
    }

    private static bool IsRequiredString(JsonElement parent, string propertyName)
    {
        return parent.TryGetProperty(propertyName, out JsonElement value)
            && value.ValueKind == JsonValueKind.String;
    }

    private static bool IsOptionalString(JsonElement parent, string propertyName)
    {
        return !parent.TryGetProperty(propertyName, out JsonElement value)
            || value.ValueKind is JsonValueKind.String or JsonValueKind.Null;
    }

    private static bool IsRequiredControlledString(
        JsonElement parent,
        string propertyName,
        IReadOnlySet<string> approvedValues)
    {
        return parent.TryGetProperty(propertyName, out JsonElement value)
            && value.ValueKind == JsonValueKind.String
            && approvedValues.Contains(value.GetString()!);
    }

    private static bool IsRequiredTime(JsonElement parent, string propertyName)
    {
        return parent.TryGetProperty(propertyName, out JsonElement value)
            && value.ValueKind == JsonValueKind.String
            && TimeOnly.TryParseExact(
                value.GetString(),
                "HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
    }

    private static bool IsRequiredBoolean(JsonElement parent, string propertyName)
    {
        return parent.TryGetProperty(propertyName, out JsonElement value)
            && value.ValueKind is JsonValueKind.True or JsonValueKind.False;
    }

    private static bool TryGetObject(
        JsonElement parent,
        string propertyName,
        out JsonElement value)
    {
        return parent.TryGetProperty(propertyName, out value)
            && value.ValueKind == JsonValueKind.Object;
    }

    private static bool ContainsProhibitedMember(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (ProhibitedMemberNames.Contains(property.Name)
                    || ContainsProhibitedMember(property.Value))
                {
                    return true;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                if (ContainsProhibitedMember(item))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
