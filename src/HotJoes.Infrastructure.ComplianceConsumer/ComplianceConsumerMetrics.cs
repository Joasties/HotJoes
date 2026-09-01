using System.Diagnostics.Metrics;

namespace HotJoes.Infrastructure.ComplianceConsumer;

internal static class ComplianceConsumerMetrics
{
    private static readonly Meter Meter = new(
        "HotJoes.Infrastructure.ComplianceConsumer");

    private static readonly Counter<long> ConsumerOutcomeCounter =
        Meter.CreateCounter<long>(
            "hotjoes.compliance.consumer.outcomes");

    private static readonly Counter<long> DuplicateReceiptCounter =
        Meter.CreateCounter<long>(
            "hotjoes.compliance.duplicate_receipts");

    private static readonly Counter<long> RecoveryRouteCounter =
        Meter.CreateCounter<long>(
            "hotjoes.compliance.recovery.routes");

    public static void RecordConsumerOutcome(string outcome)
    {
        ConsumerOutcomeCounter.Add(
            1,
            new KeyValuePair<string, object?>("outcome", outcome));
    }

    public static void RecordDuplicateReceipt()
    {
        DuplicateReceiptCounter.Add(1);
    }

    public static void RecordRecoveryRoute(string outcome)
    {
        RecoveryRouteCounter.Add(
            1,
            new KeyValuePair<string, object?>("outcome", outcome));
    }
}
