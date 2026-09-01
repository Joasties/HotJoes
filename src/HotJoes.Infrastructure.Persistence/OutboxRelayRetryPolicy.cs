namespace HotJoes.Infrastructure.Persistence;

public sealed class OutboxRelayRetryPolicy
{
    public OutboxRelayRetryPolicy(
        TimeSpan initialDelay,
        TimeSpan maximumDelay,
        int automaticAttemptLimit)
    {
        if (initialDelay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(initialDelay));
        }

        if (maximumDelay <= TimeSpan.Zero || maximumDelay < initialDelay)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumDelay));
        }

        if (automaticAttemptLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(automaticAttemptLimit));
        }

        InitialDelay = initialDelay;
        MaximumDelay = maximumDelay;
        AutomaticAttemptLimit = automaticAttemptLimit;
    }

    public TimeSpan InitialDelay { get; }

    public TimeSpan MaximumDelay { get; }

    public int AutomaticAttemptLimit { get; }

    public TimeSpan DelayForAttempt(int attemptNumber)
    {
        if (attemptNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber));
        }

        long delayTicks = InitialDelay.Ticks;

        for (int attempt = 1; attempt < attemptNumber; attempt++)
        {
            if (delayTicks >= MaximumDelay.Ticks)
            {
                return MaximumDelay;
            }

            delayTicks = delayTicks > MaximumDelay.Ticks / 2
                ? MaximumDelay.Ticks
                : delayTicks * 2;
        }

        return TimeSpan.FromTicks(
            Math.Min(delayTicks, MaximumDelay.Ticks));
    }
}
