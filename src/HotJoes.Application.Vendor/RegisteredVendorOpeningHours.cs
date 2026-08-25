namespace HotJoes.Application.Vendor;

public sealed class RegisteredVendorOpeningHours
{
    public RegisteredVendorOpeningHours(
        TimeOnly startTime,
        TimeOnly endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public TimeOnly StartTime { get; }

    public TimeOnly EndTime { get; }
}
