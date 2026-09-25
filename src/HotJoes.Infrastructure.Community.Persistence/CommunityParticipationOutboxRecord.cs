namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityParticipationOutboxRecord
{
    public Guid EventId { get; set; }

    public Guid CommunityParticipationId { get; set; }

    public int EventVersion { get; set; }

    public byte[] SerializedEvent { get; set; } = null!;

    public string? TraceParent { get; set; }

    public string? TraceState { get; set; }

    public int AttemptCount { get; set; }

    public DateTimeOffset? NextAttemptAtUtc { get; set; }

    public Guid? ClaimedBy { get; set; }

    public DateTimeOffset? ClaimExpiresAtUtc { get; set; }

    public DateTimeOffset? LastAttemptAtUtc { get; set; }

    public string? LastFailureCategory { get; set; }

    public bool IsStalled { get; set; }

    public DateTimeOffset? PublishedAtUtc { get; set; }
}
