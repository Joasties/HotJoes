namespace HotJoes.Api.Vendor;

public sealed record RegisteredVendorDailyOpeningHoursResponse(
    string Day,
    bool IsClosed,
    bool IsOpenAllDay,
    string? StartTime,
    string? EndTime);
