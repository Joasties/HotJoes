namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityOutboxRelayRetryPolicy
{
    public CommunityOutboxRelayRetryPolicy(TimeSpan initialDelay,
        TimeSpan maximumDelay, int automaticAttemptLimit)
    {
        if (initialDelay <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay));
        if (maximumDelay < initialDelay) throw new ArgumentOutOfRangeException(nameof(maximumDelay));
        if (automaticAttemptLimit <= 0) throw new ArgumentOutOfRangeException(nameof(automaticAttemptLimit));
        InitialDelay = initialDelay;
        MaximumDelay = maximumDelay;
        AutomaticAttemptLimit = automaticAttemptLimit;
    }

    public TimeSpan InitialDelay { get; }
    public TimeSpan MaximumDelay { get; }
    public int AutomaticAttemptLimit { get; }

    public TimeSpan DelayForAttempt(int attemptNumber)
    {
        if (attemptNumber <= 0) throw new ArgumentOutOfRangeException(nameof(attemptNumber));
        long ticks = InitialDelay.Ticks;
        for (int attempt = 1; attempt < attemptNumber; attempt++)
            ticks = ticks > MaximumDelay.Ticks / 2 ? MaximumDelay.Ticks : ticks * 2;
        return TimeSpan.FromTicks(Math.Min(ticks, MaximumDelay.Ticks));
    }
}
