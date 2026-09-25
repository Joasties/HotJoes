namespace HotJoes.Application.Compliance;

public sealed record DailyOpeningHours(
    TradingDay Day,
    bool IsClosed,
    bool IsOpenAllDay,
    TimeOnly? StartTime,
    TimeOnly? EndTime);
