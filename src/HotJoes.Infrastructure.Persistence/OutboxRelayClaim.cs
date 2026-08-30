namespace HotJoes.Infrastructure.Persistence;

public sealed class OutboxRelayClaim
{
    private readonly byte[] _serializedEvent;

    public OutboxRelayClaim(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent)
        : this(
            eventId,
            eventVersion,
            serializedEvent,
            traceParent: null,
            traceState: null,
            attemptCount: 0)
    {
    }

    public OutboxRelayClaim(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent,
        string? traceParent,
        string? traceState)
        : this(
            eventId,
            eventVersion,
            serializedEvent,
            traceParent,
            traceState,
            attemptCount: 0)
    {
    }

    public OutboxRelayClaim(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent,
        string? traceParent,
        string? traceState,
        int attemptCount)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event identifier must not be empty.",
                nameof(eventId));
        }

        if (eventVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eventVersion));
        }

        if (serializedEvent.IsEmpty)
        {
            throw new ArgumentException(
                "Serialized event must not be empty.",
                nameof(serializedEvent));
        }

        if (attemptCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptCount));
        }

        EventId = eventId;
        EventVersion = eventVersion;
        _serializedEvent = serializedEvent.ToArray();
        TraceParent = traceParent;
        TraceState = traceState;
        AttemptCount = attemptCount;
    }

    public Guid EventId { get; }

    public int EventVersion { get; }

    public ReadOnlyMemory<byte> SerializedEvent => _serializedEvent;

    public string? TraceParent { get; }

    public string? TraceState { get; }

    public int AttemptCount { get; }
}
