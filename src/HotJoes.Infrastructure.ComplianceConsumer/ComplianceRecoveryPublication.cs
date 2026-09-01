namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class ComplianceRecoveryPublication
{
    private readonly byte[] _serializedEvent;

    public ComplianceRecoveryPublication(
        Guid eventId,
        int eventVersion,
        ReadOnlySpan<byte> serializedEvent,
        int automaticAttempt,
        string failureCategory)
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

        if (automaticAttempt <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(automaticAttempt));
        }

        if (!IsSafeFailureCategory(failureCategory))
        {
            throw new ArgumentException(
                "Failure category must be a safe stable identifier.",
                nameof(failureCategory));
        }

        EventId = eventId;
        EventVersion = eventVersion;
        _serializedEvent = serializedEvent.ToArray();
        AutomaticAttempt = automaticAttempt;
        FailureCategory = failureCategory;
    }

    public Guid EventId { get; }

    public int EventVersion { get; }

    public ReadOnlyMemory<byte> SerializedEvent => _serializedEvent;

    public int AutomaticAttempt { get; }

    public string FailureCategory { get; }

    private static bool IsSafeFailureCategory(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 64)
        {
            return false;
        }

        return value.All(character =>
            char.IsAsciiLetterOrDigit(character) ||
            character is '.' or '-' or '_');
    }
}
