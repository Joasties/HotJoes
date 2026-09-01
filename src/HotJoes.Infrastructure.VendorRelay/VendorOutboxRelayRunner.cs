using HotJoes.Infrastructure.Persistence;

namespace HotJoes.Infrastructure.VendorRelay;

public sealed class VendorOutboxRelayRunner
{
    private readonly IOutboxRelayClaimStore _store;
    private readonly VendorOutboxRelayProcessor _processor;
    private readonly Guid _workerId;
    private readonly TimeSpan _leaseDuration;
    private readonly int _batchSize;
    private readonly OutboxRelayRetryPolicy _retryPolicy;

    public VendorOutboxRelayRunner(
        IOutboxRelayClaimStore store,
        IOutboxEventPublisher publisher,
        Guid workerId,
        TimeSpan leaseDuration,
        int batchSize,
        OutboxRelayRetryPolicy retryPolicy)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        ArgumentNullException.ThrowIfNull(publisher);

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

        _workerId = workerId;
        _leaseDuration = leaseDuration;
        _batchSize = batchSize;
        _retryPolicy = retryPolicy ??
            throw new ArgumentNullException(nameof(retryPolicy));
        _processor = new VendorOutboxRelayProcessor(store, publisher);
    }

    public async Task<OutboxRelayBatchResult> RunOnceAsync(
        DateTimeOffset attemptedAtUtc,
        CancellationToken cancellationToken = default)
    {
        DateTimeOffset attemptTime = attemptedAtUtc.ToUniversalTime();
        IReadOnlyList<OutboxRelayClaim> claims =
            await _store.ClaimEligibleAsync(
                _workerId,
                attemptTime,
                _leaseDuration,
                _batchSize,
                cancellationToken);

        int publishedCount = 0;
        int retryScheduledCount = 0;

        foreach (OutboxRelayClaim claim in claims)
        {
            OutboxRelayProcessingOutcome outcome =
                await _processor.ProcessAsync(
                    claim,
                    _workerId,
                    attemptTime,
                    _retryPolicy,
                    cancellationToken);

            if (outcome == OutboxRelayProcessingOutcome.Published)
            {
                publishedCount++;
            }
            else
            {
                retryScheduledCount++;
            }
        }

        return new OutboxRelayBatchResult(
            claims.Count,
            publishedCount,
            retryScheduledCount);
    }
}
