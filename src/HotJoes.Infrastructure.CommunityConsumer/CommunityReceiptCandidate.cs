namespace HotJoes.Infrastructure.CommunityConsumer;
public sealed class CommunityReceiptCandidate
{
    private readonly byte[] bytes;
    public CommunityReceiptCandidate(Guid eventId, string eventType,
        int eventVersion, Guid communityParticipationId, Guid vendorId,
        DateTimeOffset receivedAtUtc, ReadOnlySpan<byte> serializedEvent)
    {
        if (eventId == Guid.Empty || communityParticipationId == Guid.Empty || vendorId == Guid.Empty) throw new ArgumentException("Identifiers must not be empty.");
        if (string.IsNullOrWhiteSpace(eventType)) throw new ArgumentException("Event type is required.", nameof(eventType));
        if (eventVersion <= 0) throw new ArgumentOutOfRangeException(nameof(eventVersion));
        if (serializedEvent.IsEmpty) throw new ArgumentException("Serialized event is required.", nameof(serializedEvent));
        EventId = eventId; EventType = eventType; EventVersion = eventVersion;
        CommunityParticipationId = communityParticipationId; VendorId = vendorId;
        ReceivedAtUtc = receivedAtUtc.ToUniversalTime(); bytes = serializedEvent.ToArray();
    }
    public Guid EventId { get; }
    public string EventType { get; }
    public int EventVersion { get; }
    public Guid CommunityParticipationId { get; }
    public Guid VendorId { get; }
    public DateTimeOffset ReceivedAtUtc { get; }
    public ReadOnlyMemory<byte> SerializedEvent => bytes;
}
