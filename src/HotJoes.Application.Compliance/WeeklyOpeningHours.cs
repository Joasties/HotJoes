namespace HotJoes.Application.Compliance;

public sealed class WeeklyOpeningHours
{
    private readonly IReadOnlyList<DailyOpeningHours> _days;

    public WeeklyOpeningHours(IEnumerable<DailyOpeningHours> days)
    {
        ArgumentNullException.ThrowIfNull(days);
        _days = Array.AsReadOnly(days.ToArray());
    }

    public IReadOnlyList<DailyOpeningHours> Days => _days;
}
