namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityOutboxRelayClaim
{
    private readonly byte[] serializedEvent;

    public CommunityOutboxRelayClaim(Guid eventId, int eventVersion,
        ReadOnlySpan<byte> serializedEvent, string? traceParent = null,
        string? traceState = null, int attemptCount = 0)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Event identifier must not be empty.", nameof(eventId));
        if (eventVersion <= 0) throw new ArgumentOutOfRangeException(nameof(eventVersion));
        if (serializedEvent.IsEmpty) throw new ArgumentException("Serialized event must not be empty.", nameof(serializedEvent));
        if (attemptCount < 0) throw new ArgumentOutOfRangeException(nameof(attemptCount));
        EventId = eventId;
        EventVersion = eventVersion;
        this.serializedEvent = serializedEvent.ToArray();
        TraceParent = traceParent;
        TraceState = traceState;
        AttemptCount = attemptCount;
    }

    public Guid EventId { get; }
    public int EventVersion { get; }
    public ReadOnlyMemory<byte> SerializedEvent => serializedEvent;
    public string? TraceParent { get; }
    public string? TraceState { get; }
    public int AttemptCount { get; }
}
