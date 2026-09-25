namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class SerializedCommunityIntegrationEvent
{
    private readonly byte[] _serializedEvent;

    public SerializedCommunityIntegrationEvent(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event ID must not be empty.",
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

        EventId = eventId;
        EventVersion = eventVersion;
        _serializedEvent = serializedEvent.ToArray();
    }

    public Guid EventId { get; }

    public int EventVersion { get; }

    public ReadOnlyMemory<byte> SerializedEvent => _serializedEvent;
}
