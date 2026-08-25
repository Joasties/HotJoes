namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredOpeningHours(
    TimeOnly StartTime,
    TimeOnly EndTime);
