namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredWeeklyOpeningHours(
    IReadOnlyList<VendorRegisteredDailyOpeningHours> Days);
