namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorTradingCharacteristicsRequest
{
    public string? TradingLocation { get; init; }

    public RegisterVendorWeeklyOpeningHoursRequest? WeeklyOpeningHours { get; init; }

    public bool? ServiceIncludesHotFood { get; init; }

    public bool? AlcoholService { get; init; }
}
