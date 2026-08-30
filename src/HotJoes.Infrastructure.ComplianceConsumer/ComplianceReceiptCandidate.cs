namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class ComplianceReceiptCandidate
{
    private readonly byte[] _serializedEvent;

    public ComplianceReceiptCandidate(
        Guid eventId,
        string eventType,
        int eventVersion,
        DateTimeOffset receivedAtUtc,
        ReadOnlySpan<byte> serializedEvent)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event identifier must not be empty.",
                nameof(eventId));
        }

        if (!string.Equals(
                eventType,
                "VendorRegistered",
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Only VendorRegistered is supported.",
                nameof(eventType));
        }

        if (eventVersion != 1)
        {
            throw new ArgumentOutOfRangeException(nameof(eventVersion));
        }

        if (serializedEvent.IsEmpty)
        {
            throw new ArgumentException(
                "Serialized event must not be empty.",
                nameof(serializedEvent));
        }

        EventId = eventId;
        EventType = eventType;
        EventVersion = eventVersion;
        ReceivedAtUtc = receivedAtUtc.ToUniversalTime();
        _serializedEvent = serializedEvent.ToArray();
    }

    public Guid EventId { get; }

    public string EventType { get; }

    public int EventVersion { get; }

    public DateTimeOffset ReceivedAtUtc { get; }

    public ReadOnlyMemory<byte> SerializedEvent => _serializedEvent;
}
