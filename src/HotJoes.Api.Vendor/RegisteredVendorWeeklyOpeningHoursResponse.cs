namespace HotJoes.Api.Vendor;

public sealed record RegisteredVendorWeeklyOpeningHoursResponse(
    IReadOnlyList<RegisteredVendorDailyOpeningHoursResponse> Days);
