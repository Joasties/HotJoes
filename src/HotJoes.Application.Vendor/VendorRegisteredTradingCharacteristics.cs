namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredTradingCharacteristics(
    string TradingLocation,
    VendorRegisteredOpeningHours OpeningHours,
    bool ServiceIncludesHotFood,
    bool AlcoholService);
