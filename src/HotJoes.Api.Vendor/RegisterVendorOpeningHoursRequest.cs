namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorOpeningHoursRequest
{
    public string? StartTime { get; init; }

    public string? EndTime { get; init; }
}
