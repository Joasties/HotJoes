namespace HotJoes.Application.Vendor;

public sealed class RegisteredVendorWeeklyOpeningHours
{
    private readonly IReadOnlyList<RegisteredVendorDailyOpeningHours> _days;

    public RegisteredVendorWeeklyOpeningHours(
        IEnumerable<RegisteredVendorDailyOpeningHours> days)
    {
        ArgumentNullException.ThrowIfNull(days);
        _days = days.ToArray();
    }

    public IReadOnlyList<RegisteredVendorDailyOpeningHours> Days => _days;
}
