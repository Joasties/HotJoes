namespace HotJoes.Domain.Vendor;

public sealed class WeeklyOpeningHours : IEquatable<WeeklyOpeningHours>
{
    private static readonly TradingDay[] CanonicalDays =
        Enum.GetValues<TradingDay>();

    private readonly IReadOnlyList<DailyOpeningHours> _days;

    public WeeklyOpeningHours(IEnumerable<DailyOpeningHours> days)
    {
        ArgumentNullException.ThrowIfNull(days);

        DailyOpeningHours[] supplied = days.ToArray();
        if (supplied.Any(day => day is null) ||
            supplied.Length != CanonicalDays.Length ||
            supplied.Select(day => day.Day).Distinct().Count() !=
                CanonicalDays.Length ||
            supplied.Any(day => !Enum.IsDefined(day.Day)) ||
            CanonicalDays.Any(day => supplied.All(value => value.Day != day)))
        {
            throw new ArgumentException(
                "Weekly opening hours must contain exactly one entry for every day Monday through Sunday.",
                nameof(days));
        }

        _days = supplied.OrderBy(day => day.Day).ToArray();
    }

    public IReadOnlyList<DailyOpeningHours> Days => _days;

    public static WeeklyOpeningHours EveryDay(
        TimeOnly startTime,
        TimeOnly endTime) =>
        new(CanonicalDays.Select(
            day => DailyOpeningHours.OpenDuring(day, startTime, endTime)));

    public bool Equals(WeeklyOpeningHours? other) =>
        other is not null && _days.SequenceEqual(other._days);

    public override bool Equals(object? obj) =>
        obj is WeeklyOpeningHours other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (DailyOpeningHours day in _days)
        {
            hash.Add(day);
        }

        return hash.ToHashCode();
    }
}
