using System.Diagnostics.Metrics;

namespace HotJoes.Infrastructure.Persistence;

internal static class RegistrationPersistenceMetrics
{
    private static readonly Meter Meter = new(
        "HotJoes.Infrastructure.Persistence");

    private static readonly Counter<long> IdempotencyOutcomeCounter =
        Meter.CreateCounter<long>(
            "hotjoes.vendor.registration.idempotency");

    private static readonly Counter<long> PersistenceOutcomeCounter =
        Meter.CreateCounter<long>(
            "hotjoes.vendor.registration.persistence");

    public static void RecordIdempotencyOutcome(string outcome)
    {
        IdempotencyOutcomeCounter.Add(
            1,
            new KeyValuePair<string, object?>("outcome", outcome));
    }

    public static void RecordPersistenceOutcome(string outcome)
    {
        PersistenceOutcomeCounter.Add(
            1,
            new KeyValuePair<string, object?>("outcome", outcome));
    }
}
