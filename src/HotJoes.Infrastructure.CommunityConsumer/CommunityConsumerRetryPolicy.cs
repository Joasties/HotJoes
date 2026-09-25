namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed class CommunityConsumerRetryPolicy
{
    public CommunityConsumerRetryPolicy(int maximumAutomaticAttempts, TimeSpan retryDelay)
    {
        if (maximumAutomaticAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maximumAutomaticAttempts));
        if (retryDelay <= TimeSpan.Zero || retryDelay.TotalMilliseconds > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(retryDelay));
        MaximumAutomaticAttempts = maximumAutomaticAttempts; RetryDelay = retryDelay;
    }
    public int MaximumAutomaticAttempts { get; }
    public TimeSpan RetryDelay { get; }
}
