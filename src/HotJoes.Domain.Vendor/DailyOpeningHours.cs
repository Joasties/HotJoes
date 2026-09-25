namespace HotJoes.Domain.Vendor;

public sealed record DailyOpeningHours
{
    private DailyOpeningHours(
        TradingDay day,
        bool isClosed,
        bool isOpenAllDay,
        TimeOnly? startTime,
        TimeOnly? endTime)
    {
        Day = day;
        IsClosed = isClosed;
        IsOpenAllDay = isOpenAllDay;
        StartTime = startTime;
        EndTime = endTime;
    }

    public TradingDay Day { get; }
    public bool IsClosed { get; }
    public bool IsOpenAllDay { get; }
    public TimeOnly? StartTime { get; }
    public TimeOnly? EndTime { get; }

    public static DailyOpeningHours Closed(TradingDay day)
    {
        EnsureDefined(day);
        return new DailyOpeningHours(day, true, false, null, null);
    }

    public static DailyOpeningHours OpenAllDay(TradingDay day)
    {
        EnsureDefined(day);
        return new DailyOpeningHours(day, false, true, null, null);
    }

    public static DailyOpeningHours OpenDuring(
        TradingDay day,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        EnsureDefined(day);

        if (startTime == endTime)
        {
            throw new ArgumentException(
                "Start and end times must differ; use OpenAllDay for a full day.",
                nameof(endTime));
        }

        return new DailyOpeningHours(
            day,
            false,
            false,
            startTime,
            endTime);
    }

    private static void EnsureDefined(TradingDay day)
    {
        if (!Enum.IsDefined(day))
        {
            throw new ArgumentOutOfRangeException(nameof(day), day, null);
        }
    }
}
