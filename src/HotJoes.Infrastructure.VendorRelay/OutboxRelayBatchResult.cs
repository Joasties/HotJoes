namespace HotJoes.Infrastructure.VendorRelay;

public sealed class OutboxRelayBatchResult
{
    public OutboxRelayBatchResult(
        int claimedCount,
        int publishedCount,
        int retryScheduledCount)
    {
        if (claimedCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(claimedCount));
        }

        if (publishedCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(publishedCount));
        }

        if (retryScheduledCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(retryScheduledCount));
        }

        if (publishedCount + retryScheduledCount != claimedCount)
        {
            throw new ArgumentException(
                "Every claimed item must have one processing outcome.");
        }

        ClaimedCount = claimedCount;
        PublishedCount = publishedCount;
        RetryScheduledCount = retryScheduledCount;
    }

    public int ClaimedCount { get; }

    public int PublishedCount { get; }

    public int RetryScheduledCount { get; }
}
