using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RegisterVendorWeeklyOpeningHours
{
    private readonly IReadOnlyList<RegisterVendorDailyOpeningHours> _days;

    public RegisterVendorWeeklyOpeningHours(
        IEnumerable<RegisterVendorDailyOpeningHours> days)
    {
        ArgumentNullException.ThrowIfNull(days);
        _days = days.ToArray();
    }

    public IReadOnlyList<RegisterVendorDailyOpeningHours> Days => _days;

    public static RegisterVendorWeeklyOpeningHours EveryDay(
        TimeOnly startTime,
        TimeOnly endTime) =>
        new(Enum.GetValues<TradingDay>().Select(day =>
            new RegisterVendorDailyOpeningHours(
                day,
                false,
                false,
                startTime,
                endTime)));
}
