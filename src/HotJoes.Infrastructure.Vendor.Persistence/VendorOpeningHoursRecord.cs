namespace HotJoes.Infrastructure.Vendor.Persistence;

internal sealed class VendorOpeningHoursRecord
{
    public Guid VendorId { get; set; }
    public string Day { get; set; } = null!;
    public bool IsClosed { get; set; }
    public bool IsOpenAllDay { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public VendorRegistrationRecord Vendor { get; set; } = null!;
}
