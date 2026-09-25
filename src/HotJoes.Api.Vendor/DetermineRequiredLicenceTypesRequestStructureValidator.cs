using System.Globalization;
using System.Text.Json;

namespace HotJoes.Api.Vendor;

public sealed class DetermineRequiredLicenceTypesRequestStructureValidator
{
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

    private static readonly HashSet<string> TradingDays =
        new(StringComparer.Ordinal)
        {
            "monday", "tuesday", "wednesday", "thursday",
            "friday", "saturday", "sunday"
        };

    public bool IsValid(JsonElement root)
    {
        return root.ValueKind == JsonValueKind.Object
            && IsRequiredControlledString(
                root,
                "legalOperatorType",
                LegalOperatorTypes)
            && IsRequiredControlledString(
                root,
                "tradingLocation",
                TradingLocations)
            && IsWeeklyOpeningHoursValid(root)
            && IsRequiredBoolean(root, "serviceIncludesHotFood")
            && IsRequiredBoolean(root, "alcoholService")
            && IsRequiredString(root, "addressResolutionReference");
    }

    private static bool IsWeeklyOpeningHoursValid(JsonElement root)
    {
        return TryGetObject(root, "weeklyOpeningHours", out JsonElement hours)
            && hours.TryGetProperty("days", out JsonElement days)
            && days.ValueKind == JsonValueKind.Array
            && days.GetArrayLength() == 7
            && days.EnumerateArray().All(IsDayValid);
    }

    private static bool IsDayValid(JsonElement day)
    {
        return day.ValueKind == JsonValueKind.Object
            && IsRequiredControlledString(day, "day", TradingDays)
            && IsRequiredBoolean(day, "isClosed")
            && IsRequiredBoolean(day, "isOpenAllDay")
            && IsOptionalTime(day, "startTime")
            && IsOptionalTime(day, "endTime");
    }

    private static bool IsRequiredString(JsonElement parent, string name) =>
        parent.TryGetProperty(name, out JsonElement value)
        && value.ValueKind == JsonValueKind.String;

    private static bool IsRequiredControlledString(
        JsonElement parent,
        string name,
        IReadOnlySet<string> approvedValues) =>
        parent.TryGetProperty(name, out JsonElement value)
        && value.ValueKind == JsonValueKind.String
        && approvedValues.Contains(value.GetString()!);

    private static bool IsRequiredBoolean(JsonElement parent, string name) =>
        parent.TryGetProperty(name, out JsonElement value)
        && value.ValueKind is JsonValueKind.True or JsonValueKind.False;

    private static bool IsOptionalTime(JsonElement parent, string name) =>
        parent.TryGetProperty(name, out JsonElement value)
        && (value.ValueKind == JsonValueKind.Null
            || value.ValueKind == JsonValueKind.String
            && TimeOnly.TryParseExact(
                value.GetString(),
                "HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _));

    private static bool TryGetObject(
        JsonElement parent,
        string name,
        out JsonElement value) =>
        parent.TryGetProperty(name, out value)
        && value.ValueKind == JsonValueKind.Object;
}
