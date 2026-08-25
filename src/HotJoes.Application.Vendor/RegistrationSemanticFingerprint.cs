using System.Buffers;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed record RegistrationSemanticFingerprint
{
    private const short CurrentVersion = 1;

    private RegistrationSemanticFingerprint(short version, string sha256Digest)
    {
        Version = version;
        Sha256Digest = sha256Digest;
    }

    public short Version { get; }

    public string Sha256Digest { get; }

    public static RegistrationSemanticFingerprint Create(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(addressValues);

        byte[] canonicalMaterial = CreateCanonicalMaterial(command, addressValues);
        byte[] digest = SHA256.HashData(canonicalMaterial);

        return new RegistrationSemanticFingerprint(
            CurrentVersion,
            Convert.ToHexString(digest).ToLowerInvariant());
    }

    private static byte[] CreateCanonicalMaterial(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
    {
        var buffer = new ArrayBufferWriter<byte>();

        using (var writer = new Utf8JsonWriter(buffer))
        {
            BusinessAddressSnapshot address =
                addressValues.BusinessAddressSnapshot;

            writer.WriteStartObject();
            writer.WriteNumber("version", CurrentVersion);
            writer.WriteString(
                "tradingName",
                NormalizeIdentityName(command.TradingName));
            writer.WriteString(
                "legalOperatorName",
                NormalizeIdentityName(command.LegalOperatorName));
            writer.WriteString(
                "legalOperatorType",
                MapLegalOperatorType(command.LegalOperatorType));
            WriteOptionalString(
                writer,
                "companyRegistrationNumber",
                command.CompanyRegistrationNumber?.ToUpperInvariant());
            writer.WriteString(
                "tradingLocation",
                MapTradingLocation(command.TradingLocation));
            writer.WriteString(
                "openingHoursStart",
                FormatTime(command.OpeningHoursStartTime));
            writer.WriteString(
                "openingHoursEnd",
                FormatTime(command.OpeningHoursEndTime));
            writer.WriteBoolean(
                "serviceIncludesHotFood",
                command.ServiceIncludesHotFood);
            writer.WriteBoolean("alcoholService", command.AlcoholService);
            writer.WriteString("contactName", command.ContactName);
            writer.WriteString("contactEmail", command.ContactEmail);
            writer.WriteString("contactTelephone", command.ContactTelephone);
            WriteOptionalString(writer, "website", command.Website);
            WriteOptionalString(
                writer,
                "businessDescription",
                command.BusinessDescription);
            writer.WriteString(
                "canonicalAddressId",
                addressValues.CanonicalAddressId.Value);
            WriteOptionalString(
                writer,
                "recipientOrOrganisationName",
                address.RecipientOrOrganisationName);
            writer.WriteString("addressLine1", address.AddressLine1);
            WriteOptionalString(writer, "addressLine2", address.AddressLine2);
            WriteOptionalString(writer, "addressLine3", address.AddressLine3);
            writer.WriteString("postTown", address.PostTown);
            writer.WriteString("postcode", address.Postcode);
            WriteOptionalString(writer, "county", address.County);
            writer.WriteString(
                "foodRegistrationAuthority",
                addressValues.FoodRegistrationAuthority.Value);
            WriteOptionalString(
                writer,
                "primaryTradingAuthority",
                addressValues.PrimaryTradingAuthority?.Value);
            writer.WriteEndObject();
        }

        return buffer.WrittenSpan.ToArray();
    }

    private static string NormalizeIdentityName(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static string FormatTime(TimeOnly value)
    {
        return value.ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
    }

    private static void WriteOptionalString(
        Utf8JsonWriter writer,
        string propertyName,
        string? value)
    {
        if (value is null)
        {
            writer.WriteNull(propertyName);
            return;
        }

        writer.WriteString(propertyName, value);
    }

    private static string MapLegalOperatorType(LegalOperatorType value)
    {
        return value switch
        {
            LegalOperatorType.SoleTrader => "soleTrader",
            LegalOperatorType.GeneralPartnership => "generalPartnership",
            LegalOperatorType.LimitedCompany => "limitedCompany",
            LegalOperatorType.LimitedLiabilityPartnership =>
                "limitedLiabilityPartnership",
            LegalOperatorType.CharitableCommunityGroup =>
                "charitableCommunityGroup",
            LegalOperatorType.CharitableIncorporatedOrganisation =>
                "charitableIncorporatedOrganisation",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }

    private static string MapTradingLocation(TradingLocation value)
    {
        return value switch
        {
            TradingLocation.Restaurant => "restaurant",
            TradingLocation.Stall => "stall",
            TradingLocation.Kitchen => "kitchen",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }
}
