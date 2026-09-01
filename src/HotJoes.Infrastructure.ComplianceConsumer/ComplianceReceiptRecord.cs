namespace HotJoes.Infrastructure.ComplianceConsumer;

internal sealed class ComplianceReceiptRecord
{
    public Guid EventId { get; set; }

    public string EventType { get; set; } = null!;

    public int EventVersion { get; set; }

    public DateTimeOffset ReceivedAtUtc { get; set; }

    public byte[] SerializedEventSha256 { get; set; } = null!;
}
