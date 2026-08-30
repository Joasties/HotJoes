namespace HotJoes.Infrastructure.Persistence;

internal sealed class VendorRegistrationOutboxRecord
{
    public Guid EventId { get; set; }

    public Guid VendorId { get; set; }

    public int EventVersion { get; set; }

    public byte[] SerializedEvent { get; set; } = null!;

    public string? TraceParent { get; set; }

    public string? TraceState { get; set; }

    public int AttemptCount { get; set; }

    public DateTimeOffset? NextAttemptAtUtc { get; set; }

    public Guid? ClaimedBy { get; set; }

    public DateTimeOffset? ClaimExpiresAtUtc { get; set; }

    public DateTimeOffset? LastAttemptAtUtc { get; set; }

    public OutboxRelayFailureCategory? LastFailureCategory { get; set; }

    public bool IsStalled { get; set; }

    public DateTimeOffset? PublishedAtUtc { get; set; }
}
