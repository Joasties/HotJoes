namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredDailyOpeningHours(
    string Day,
    bool IsClosed,
    bool IsOpenAllDay,
    TimeOnly? StartTime,
    TimeOnly? EndTime);
