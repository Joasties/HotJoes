namespace HotJoes.Domain.Vendor;

public readonly record struct OpeningHours(TimeOnly StartTime, TimeOnly EndTime);
