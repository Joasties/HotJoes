namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorDailyOpeningHoursRequest
{
    public string? Day { get; init; }
    public bool? IsClosed { get; init; }
    public bool? IsOpenAllDay { get; init; }
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }
}
