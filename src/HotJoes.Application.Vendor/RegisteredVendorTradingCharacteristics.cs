using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RegisteredVendorTradingCharacteristics
{
    public RegisteredVendorTradingCharacteristics(
        TradingLocation tradingLocation,
        RegisteredVendorWeeklyOpeningHours weeklyOpeningHours,
        bool serviceIncludesHotFood,
        bool alcoholService)
    {
        ArgumentNullException.ThrowIfNull(weeklyOpeningHours);

        TradingLocation = tradingLocation;
        WeeklyOpeningHours = weeklyOpeningHours;
        ServiceIncludesHotFood = serviceIncludesHotFood;
        AlcoholService = alcoholService;
    }

    public TradingLocation TradingLocation { get; }

    public RegisteredVendorWeeklyOpeningHours WeeklyOpeningHours { get; }

    public bool ServiceIncludesHotFood { get; }

    public bool AlcoholService { get; }
}
