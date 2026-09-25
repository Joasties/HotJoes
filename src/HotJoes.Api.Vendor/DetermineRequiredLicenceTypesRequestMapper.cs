using System.Globalization;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using ApplicationRequest = HotJoes.Application.Vendor.DetermineRequiredLicenceTypesRequest;

namespace HotJoes.Api.Vendor;

public sealed class DetermineRequiredLicenceTypesRequestMapper
{
    public ApplicationRequest Map(DetermineRequiredLicenceTypesRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        RegisterVendorWeeklyOpeningHoursRequest openingHours =
            request.WeeklyOpeningHours
            ?? throw new ArgumentException(
                "Weekly Opening Hours must be structurally valid before mapping.",
                nameof(request));

        return new ApplicationRequest(
            ParseEnum<LegalOperatorType>(request.LegalOperatorType),
            ParseEnum<TradingLocation>(request.TradingLocation),
            new RegisterVendorWeeklyOpeningHours(
                openingHours.Days!.Select(MapDay)),
            RequireBoolean(request.ServiceIncludesHotFood),
            RequireBoolean(request.AlcoholService),
            request.AddressResolutionReference!);
    }

    private static RegisterVendorDailyOpeningHours MapDay(
        RegisterVendorDailyOpeningHoursRequest day) =>
        new(
            ParseEnum<TradingDay>(day.Day),
            RequireBoolean(day.IsClosed),
            RequireBoolean(day.IsOpenAllDay),
            ParseOptionalTime(day.StartTime),
            ParseOptionalTime(day.EndTime));

    private static TEnum ParseEnum<TEnum>(string? value)
        where TEnum : struct, Enum
    {
        if (!Enum.TryParse(value, ignoreCase: true, out TEnum result))
        {
            throw new ArgumentException(
                $"{typeof(TEnum).Name} must be structurally valid before mapping.");
        }

        return result;
    }

    private static TimeOnly? ParseOptionalTime(string? value)
    {
        if (value is null)
        {
            return null;
        }

        return TimeOnly.ParseExact(
            value,
            "HH:mm:ss",
            CultureInfo.InvariantCulture);
    }

    private static bool RequireBoolean(bool? value) =>
        value ?? throw new ArgumentException(
            "Required booleans must be present before mapping.");
}
