using System.Data;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.Persistence;

public sealed class PostgreSqlOutboxRelayStore : IOutboxRelayClaimStore
{
    private readonly VendorRegistrationDbContext _context;

    public PostgreSqlOutboxRelayStore(VendorRegistrationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<OutboxRelayClaim>> ClaimEligibleAsync(
        Guid workerId,
        DateTimeOffset claimedAtUtc,
        TimeSpan leaseDuration,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        if (workerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Worker identifier must not be empty.",
                nameof(workerId));
        }

        if (leaseDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(leaseDuration));
        }

        if (batchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        DateTimeOffset claimTime = claimedAtUtc.ToUniversalTime();
        DateTimeOffset claimExpiry = claimTime.Add(leaseDuration);

        await using var transaction = await _context.Database
            .BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken);

        List<VendorRegistrationOutboxRecord> records = await _context
            .Set<VendorRegistrationOutboxRecord>()
            .FromSqlInterpolated($"""
                SELECT *
                FROM vendor_registration_outbox
                WHERE published_at_utc IS NULL
                  AND is_stalled = FALSE
                  AND (
                    next_attempt_at_utc IS NULL
                    OR next_attempt_at_utc <= {claimTime})
                  AND (
                    claim_expires_at_utc IS NULL
                    OR claim_expires_at_utc <= {claimTime})
                ORDER BY event_id
                LIMIT {batchSize}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(cancellationToken);

        foreach (VendorRegistrationOutboxRecord record in records)
        {
            record.NextAttemptAtUtc = null;
            record.ClaimedBy = workerId;
            record.ClaimExpiresAtUtc = claimExpiry;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return records
            .Select(record => new OutboxRelayClaim(
                record.EventId,
                record.EventVersion,
                record.SerializedEvent,
                record.TraceParent,
                record.TraceState,
                record.AttemptCount))
            .ToArray();
    }

    public async Task MarkPublishedAsync(
        Guid eventId,
        Guid workerId,
        DateTimeOffset publishedAtUtc,
        CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event identifier must not be empty.",
                nameof(eventId));
        }

        if (workerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Worker identifier must not be empty.",
                nameof(workerId));
        }

        VendorRegistrationOutboxRecord record = await FindOwnedRecordAsync(
            eventId,
            workerId,
            cancellationToken);

        record.NextAttemptAtUtc = null;
        record.ClaimedBy = null;
        record.ClaimExpiresAtUtc = null;
        record.IsStalled = false;
        record.PublishedAtUtc = publishedAtUtc.ToUniversalTime();

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordFailureAsync(
        Guid eventId,
        Guid workerId,
        DateTimeOffset failedAtUtc,
        OutboxRelayFailureCategory failureCategory,
        OutboxRelayRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event identifier must not be empty.",
                nameof(eventId));
        }

        if (workerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Worker identifier must not be empty.",
                nameof(workerId));
        }

        if (!Enum.IsDefined(failureCategory))
        {
            throw new ArgumentOutOfRangeException(nameof(failureCategory));
        }

        ArgumentNullException.ThrowIfNull(retryPolicy);

        VendorRegistrationOutboxRecord record = await FindOwnedRecordAsync(
            eventId,
            workerId,
            cancellationToken);

        DateTimeOffset attemptTime = failedAtUtc.ToUniversalTime();
        int attemptCount = checked(record.AttemptCount + 1);
        bool isStalled = attemptCount >= retryPolicy.AutomaticAttemptLimit;

        record.AttemptCount = attemptCount;
        record.NextAttemptAtUtc = isStalled
            ? null
            : attemptTime.Add(retryPolicy.DelayForAttempt(attemptCount));
        record.ClaimedBy = null;
        record.ClaimExpiresAtUtc = null;
        record.LastAttemptAtUtc = attemptTime;
        record.LastFailureCategory = failureCategory;
        record.IsStalled = isStalled;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RequeueStalledAsync(
        Guid eventId,
        DateTimeOffset requeuedAtUtc,
        CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event identifier must not be empty.",
                nameof(eventId));
        }

        VendorRegistrationOutboxRecord? record = await _context
            .Set<VendorRegistrationOutboxRecord>()
            .SingleOrDefaultAsync(
                item => item.EventId == eventId &&
                    item.IsStalled &&
                    item.PublishedAtUtc == null,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        record.AttemptCount = 0;
        record.NextAttemptAtUtc = requeuedAtUtc.ToUniversalTime();
        record.ClaimedBy = null;
        record.ClaimExpiresAtUtc = null;
        record.IsStalled = false;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<VendorRegistrationOutboxRecord> FindOwnedRecordAsync(
        Guid eventId,
        Guid workerId,
        CancellationToken cancellationToken)
    {
        return await _context
            .Set<VendorRegistrationOutboxRecord>()
            .SingleOrDefaultAsync(
                item => item.EventId == eventId &&
                    item.ClaimedBy == workerId &&
                    item.PublishedAtUtc == null,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "The worker does not own the unpublished outbox record.");
    }
}
