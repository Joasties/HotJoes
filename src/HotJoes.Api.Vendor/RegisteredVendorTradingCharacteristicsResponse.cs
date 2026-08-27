namespace HotJoes.Api.Vendor;

public sealed record RegisteredVendorTradingCharacteristicsResponse(
    string TradingLocation,
    RegisteredVendorOpeningHoursResponse OpeningHours,
    bool ServiceIncludesHotFood,
    bool AlcoholService);
