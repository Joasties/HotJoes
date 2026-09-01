using System.Diagnostics;

namespace HotJoes.Infrastructure.VendorRelay;

public sealed class OutboxPublication
{
    private readonly byte[] _serializedEvent;

    public OutboxPublication(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent)
        : this(
            eventId,
            eventVersion,
            serializedEvent,
            traceParent: null,
            traceState: null)
    {
    }

    public OutboxPublication(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent,
        string? traceParent,
        string? traceState)
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

        if (traceParent is null && traceState is not null)
        {
            throw new ArgumentException(
                "Trace state requires a trace parent.",
                nameof(traceState));
        }

        if (traceParent is not null && !ActivityContext.TryParse(
            traceParent,
            traceState,
            isRemote: true,
            out _))
        {
            throw new ArgumentException(
                "Trace metadata must contain valid W3C context.",
                nameof(traceParent));
        }

        EventId = eventId;
        EventVersion = eventVersion;
        _serializedEvent = serializedEvent.ToArray();
        TraceParent = traceParent;
        TraceState = traceState;
    }

    public Guid EventId { get; }

    public int EventVersion { get; }

    public ReadOnlyMemory<byte> SerializedEvent => _serializedEvent;

    public string? TraceParent { get; }

    public string? TraceState { get; }
}
