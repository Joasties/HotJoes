using System.Diagnostics;

namespace HotJoes.Infrastructure.CommunityRelay;

public sealed class CommunityOutboxPublication
{
    private readonly byte[] serializedEvent;

    public CommunityOutboxPublication(Guid eventId, int eventVersion,
        ReadOnlySpan<byte> serializedEvent, string? traceParent = null,
        string? traceState = null)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Event identifier must not be empty.", nameof(eventId));
        if (eventVersion <= 0) throw new ArgumentOutOfRangeException(nameof(eventVersion));
        if (serializedEvent.IsEmpty) throw new ArgumentException("Serialized event must not be empty.", nameof(serializedEvent));
        if (traceParent is null && traceState is not null) throw new ArgumentException("Trace state requires a trace parent.", nameof(traceState));
        if (traceParent is not null && !ActivityContext.TryParse(traceParent,
            traceState, true, out _)) throw new ArgumentException(
                "Trace metadata must contain valid W3C context.", nameof(traceParent));
        EventId = eventId;
        EventVersion = eventVersion;
        this.serializedEvent = serializedEvent.ToArray();
        TraceParent = traceParent;
        TraceState = traceState;
    }

    public Guid EventId { get; }
    public int EventVersion { get; }
    public ReadOnlyMemory<byte> SerializedEvent => serializedEvent;
    public string? TraceParent { get; }
    public string? TraceState { get; }
}
