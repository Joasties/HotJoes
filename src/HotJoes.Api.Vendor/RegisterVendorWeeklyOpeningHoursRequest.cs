namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorWeeklyOpeningHoursRequest
{
    public IReadOnlyList<RegisterVendorDailyOpeningHoursRequest>? Days { get; init; }
}
