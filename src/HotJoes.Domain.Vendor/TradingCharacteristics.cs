namespace HotJoes.Domain.Vendor;

public sealed record TradingCharacteristics
{
    public TradingCharacteristics(
        TradingLocation tradingLocation,
        OpeningHours openingHours,
        bool serviceIncludesHotFood,
        bool alcoholService)
    {
        TradingLocation = tradingLocation;
        OpeningHours = openingHours;
        ServiceIncludesHotFood = serviceIncludesHotFood;
        AlcoholService = alcoholService;
    }

    public TradingLocation TradingLocation { get; }
    public OpeningHours OpeningHours { get; }
    public bool ServiceIncludesHotFood { get; }
    public bool AlcoholService { get; }
}
