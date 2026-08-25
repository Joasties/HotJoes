namespace HotJoes.Infrastructure.Persistence;

public sealed class SerializedIntegrationEvent
{
    private readonly byte[] _serializedEvent;

    internal SerializedIntegrationEvent(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("EventId cannot be empty.", nameof(eventId));
        }

        if (eventVersion < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(eventVersion));
        }

        if (serializedEvent.IsEmpty)
        {
            throw new ArgumentException(
                "Serialized event cannot be empty.",
                nameof(serializedEvent));
        }

        EventId = eventId;
        EventVersion = eventVersion;
        _serializedEvent = serializedEvent.ToArray();
    }

    public Guid EventId { get; }

    public int EventVersion { get; }

    public ReadOnlyMemory<byte> SerializedEvent => _serializedEvent;
}
