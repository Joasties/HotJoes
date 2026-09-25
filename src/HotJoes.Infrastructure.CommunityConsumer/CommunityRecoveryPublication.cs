namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed class CommunityRecoveryPublication
{
    private readonly byte[] bytes;
    public CommunityRecoveryPublication(Guid eventId, int eventVersion,
        ReadOnlySpan<byte> serializedEvent, int automaticAttempt, string failureCategory)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Event identifier is required.", nameof(eventId));
        if (eventVersion <= 0) throw new ArgumentOutOfRangeException(nameof(eventVersion));
        if (serializedEvent.IsEmpty) throw new ArgumentException("Serialized event is required.", nameof(serializedEvent));
        if (automaticAttempt <= 0) throw new ArgumentOutOfRangeException(nameof(automaticAttempt));
        if (string.IsNullOrWhiteSpace(failureCategory) || failureCategory.Length > 64 ||
            !failureCategory.All(c => char.IsAsciiLetterOrDigit(c) || c is '.' or '-' or '_'))
            throw new ArgumentException("Failure category must be a safe stable identifier.", nameof(failureCategory));
        EventId = eventId; EventVersion = eventVersion; bytes = serializedEvent.ToArray();
        AutomaticAttempt = automaticAttempt; FailureCategory = failureCategory;
    }
    public Guid EventId { get; }
    public int EventVersion { get; }
    public ReadOnlyMemory<byte> SerializedEvent => bytes;
    public int AutomaticAttempt { get; }
    public string FailureCategory { get; }
}
