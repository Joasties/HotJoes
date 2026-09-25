using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed record RegisteredVendorDailyOpeningHours(
    TradingDay Day,
    bool IsClosed,
    bool IsOpenAllDay,
    TimeOnly? StartTime,
    TimeOnly? EndTime);
