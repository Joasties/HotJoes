using HotJoes.Infrastructure.Persistence;

namespace HotJoes.IntegrationTests;

public sealed class OutboxRelayRetryPolicyTests
{
    [Fact]
    public void Constructor_ValidValues_RetainsConfiguration()
    {
        var policy = new OutboxRelayRetryPolicy(
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(25),
            automaticAttemptLimit: 4);

        Assert.Equal(TimeSpan.FromSeconds(10), policy.InitialDelay);
        Assert.Equal(TimeSpan.FromSeconds(25), policy.MaximumDelay);
        Assert.Equal(4, policy.AutomaticAttemptLimit);
    }

    [Theory]
    [InlineData(0, 25, 4)]
    [InlineData(-1, 25, 4)]
    [InlineData(10, 0, 4)]
    [InlineData(10, -1, 4)]
    [InlineData(25, 10, 4)]
    [InlineData(10, 25, 0)]
    [InlineData(10, 25, -1)]
    public void Constructor_InvalidValues_Throws(
        int initialDelaySeconds,
        int maximumDelaySeconds,
        int automaticAttemptLimit)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OutboxRelayRetryPolicy(
                TimeSpan.FromSeconds(initialDelaySeconds),
                TimeSpan.FromSeconds(maximumDelaySeconds),
                automaticAttemptLimit));
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(3, 25)]
    [InlineData(4, 25)]
    public void DelayForAttempt_UsesBoundedExponentialBackoff(
        int attemptNumber,
        int expectedDelaySeconds)
    {
        var policy = new OutboxRelayRetryPolicy(
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(25),
            automaticAttemptLimit: 4);

        Assert.Equal(
            TimeSpan.FromSeconds(expectedDelaySeconds),
            policy.DelayForAttempt(attemptNumber));
    }
}
