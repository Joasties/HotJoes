using System.Data;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class PostgreSqlCommunityOutboxRelayStore
    : ICommunityOutboxRelayClaimStore
{
    private readonly CommunityPersistenceDbContext context;
    public PostgreSqlCommunityOutboxRelayStore(CommunityPersistenceDbContext context) =>
        this.context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IReadOnlyList<CommunityOutboxRelayClaim>> ClaimEligibleAsync(
        Guid workerId, DateTimeOffset claimedAtUtc, TimeSpan leaseDuration,
        int batchSize, CancellationToken cancellationToken = default)
    {
        if (workerId == Guid.Empty) throw new ArgumentException("Worker identifier must not be empty.", nameof(workerId));
        if (leaseDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(leaseDuration));
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
        DateTimeOffset claimTime = claimedAtUtc.ToUniversalTime();
        DateTimeOffset claimExpiry = claimTime.Add(leaseDuration);
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        List<CommunityParticipationOutboxRecord> records = await context.Set<CommunityParticipationOutboxRecord>()
            .FromSqlInterpolated($"""
                SELECT * FROM community.community_participation_outbox
                WHERE published_at_utc IS NULL AND is_stalled = FALSE
                  AND (next_attempt_at_utc IS NULL OR next_attempt_at_utc <= {claimTime})
                  AND (claim_expires_at_utc IS NULL OR claim_expires_at_utc <= {claimTime})
                ORDER BY event_id LIMIT {batchSize} FOR UPDATE SKIP LOCKED
                """).ToListAsync(cancellationToken);
        foreach (var record in records)
        {
            record.NextAttemptAtUtc = null;
            record.ClaimedBy = workerId;
            record.ClaimExpiresAtUtc = claimExpiry;
        }
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return records.Select(r => new CommunityOutboxRelayClaim(r.EventId,
            r.EventVersion, r.SerializedEvent, r.TraceParent, r.TraceState,
            r.AttemptCount)).ToArray();
    }

    public async Task MarkPublishedAsync(Guid eventId, Guid workerId,
        DateTimeOffset publishedAtUtc, CancellationToken cancellationToken = default)
    {
        var record = await FindOwnedAsync(eventId, workerId, cancellationToken);
        record.NextAttemptAtUtc = null;
        record.ClaimedBy = null;
        record.ClaimExpiresAtUtc = null;
        record.IsStalled = false;
        record.PublishedAtUtc = publishedAtUtc.ToUniversalTime();
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordFailureAsync(Guid eventId, Guid workerId,
        DateTimeOffset failedAtUtc, CommunityOutboxRelayFailureCategory failureCategory,
        CommunityOutboxRelayRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(failureCategory)) throw new ArgumentOutOfRangeException(nameof(failureCategory));
        ArgumentNullException.ThrowIfNull(retryPolicy);
        var record = await FindOwnedAsync(eventId, workerId, cancellationToken);
        int attempts = checked(record.AttemptCount + 1);
        bool stalled = attempts >= retryPolicy.AutomaticAttemptLimit;
        DateTimeOffset failed = failedAtUtc.ToUniversalTime();
        record.AttemptCount = attempts;
        record.NextAttemptAtUtc = stalled ? null : failed.Add(retryPolicy.DelayForAttempt(attempts));
        record.ClaimedBy = null;
        record.ClaimExpiresAtUtc = null;
        record.LastAttemptAtUtc = failed;
        record.LastFailureCategory = failureCategory.ToString();
        record.IsStalled = stalled;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RequeueStalledAsync(Guid eventId,
        DateTimeOffset requeuedAtUtc, CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Event identifier must not be empty.", nameof(eventId));
        var record = await context.Set<CommunityParticipationOutboxRecord>()
            .SingleOrDefaultAsync(r => r.EventId == eventId && r.IsStalled &&
                r.PublishedAtUtc == null, cancellationToken);
        if (record is null) return false;
        record.AttemptCount = 0;
        record.NextAttemptAtUtc = requeuedAtUtc.ToUniversalTime();
        record.ClaimedBy = null;
        record.ClaimExpiresAtUtc = null;
        record.IsStalled = false;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<CommunityParticipationOutboxRecord> FindOwnedAsync(
        Guid eventId, Guid workerId, CancellationToken cancellationToken)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Event identifier must not be empty.", nameof(eventId));
        if (workerId == Guid.Empty) throw new ArgumentException("Worker identifier must not be empty.", nameof(workerId));
        return await context.Set<CommunityParticipationOutboxRecord>()
            .SingleOrDefaultAsync(r => r.EventId == eventId &&
                r.ClaimedBy == workerId && r.PublishedAtUtc == null,
                cancellationToken) ?? throw new InvalidOperationException(
                    "The worker does not own the unpublished Community outbox record.");
    }
}
