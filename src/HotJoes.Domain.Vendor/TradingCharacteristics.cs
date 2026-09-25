namespace HotJoes.Domain.Vendor;

public sealed record TradingCharacteristics
{
    public TradingCharacteristics(
        TradingLocation tradingLocation,
        WeeklyOpeningHours weeklyOpeningHours,
        bool serviceIncludesHotFood,
        bool alcoholService)
    {
        TradingLocation = tradingLocation;
        ArgumentNullException.ThrowIfNull(weeklyOpeningHours);
        WeeklyOpeningHours = weeklyOpeningHours;
        ServiceIncludesHotFood = serviceIncludesHotFood;
        AlcoholService = alcoholService;
    }

    public TradingLocation TradingLocation { get; }
    public WeeklyOpeningHours WeeklyOpeningHours { get; }
    public bool ServiceIncludesHotFood { get; }
    public bool AlcoholService { get; }
}
