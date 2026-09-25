namespace HotJoes.Api.Vendor;

public sealed record RegisteredVendorTradingCharacteristicsResponse(
    string TradingLocation,
    RegisteredVendorWeeklyOpeningHoursResponse WeeklyOpeningHours,
    bool ServiceIncludesHotFood,
    bool AlcoholService);
