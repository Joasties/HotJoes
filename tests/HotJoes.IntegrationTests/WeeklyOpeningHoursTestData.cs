using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Vendor.Persistence;

namespace HotJoes.IntegrationTests;

internal static class WeeklyOpeningHoursTestData
{
    public static List<VendorOpeningHoursRecord> Records(
        TimeOnly startTime,
        TimeOnly endTime) =>
        Records(WeeklyOpeningHours.EveryDay(startTime, endTime));

    public static List<VendorOpeningHoursRecord> Records(
        WeeklyOpeningHours openingHours) =>
        openingHours.Days
            .Select(hours => new VendorOpeningHoursRecord
            {
                Day = ToContractValue(hours.Day),
                IsClosed = hours.IsClosed,
                IsOpenAllDay = hours.IsOpenAllDay,
                StartTime = hours.StartTime,
                EndTime = hours.EndTime
            })
            .ToList();

    public static WeeklyOpeningHours ToDomain(
        RegisterVendorWeeklyOpeningHours openingHours) =>
        new(openingHours.Days.Select(ToDomain));

    private static DailyOpeningHours ToDomain(
        RegisterVendorDailyOpeningHours day)
    {
        if (day.IsClosed)
        {
            return DailyOpeningHours.Closed(day.Day);
        }

        if (day.IsOpenAllDay)
        {
            return DailyOpeningHours.OpenAllDay(day.Day);
        }

        return DailyOpeningHours.OpenDuring(
            day.Day,
            day.StartTime!.Value,
            day.EndTime!.Value);
    }

    private static string ToContractValue(TradingDay day) => day switch
    {
        TradingDay.Monday => "monday",
        TradingDay.Tuesday => "tuesday",
        TradingDay.Wednesday => "wednesday",
        TradingDay.Thursday => "thursday",
        TradingDay.Friday => "friday",
        TradingDay.Saturday => "saturday",
        TradingDay.Sunday => "sunday",
        _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
    };
}
