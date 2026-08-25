using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RegisteredVendorTradingCharacteristics
{
    public RegisteredVendorTradingCharacteristics(
        TradingLocation tradingLocation,
        RegisteredVendorOpeningHours openingHours,
        bool serviceIncludesHotFood,
        bool alcoholService)
    {
        ArgumentNullException.ThrowIfNull(openingHours);

        TradingLocation = tradingLocation;
        OpeningHours = openingHours;
        ServiceIncludesHotFood = serviceIncludesHotFood;
        AlcoholService = alcoholService;
    }

    public TradingLocation TradingLocation { get; }

    public RegisteredVendorOpeningHours OpeningHours { get; }

    public bool ServiceIncludesHotFood { get; }

    public bool AlcoholService { get; }
}
