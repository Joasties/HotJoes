namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredTradingCharacteristics(
    string TradingLocation,
    VendorRegisteredWeeklyOpeningHours WeeklyOpeningHours,
    bool ServiceIncludesHotFood,
    bool AlcoholService);
