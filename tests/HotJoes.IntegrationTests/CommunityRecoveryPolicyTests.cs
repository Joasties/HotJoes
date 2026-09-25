using HotJoes.Infrastructure.CommunityConsumer;

namespace HotJoes.IntegrationTests;

public sealed class CommunityRecoveryPolicyTests
{
    private static readonly Guid EventId = Guid.Parse(
        "771841ce-5229-4707-8e96-43802213cde0");

    [Fact]
    public void AI_COMMUNITY_003_ValidPolicyPreservesInjectedBounds()
    {
        var policy = new CommunityConsumerRetryPolicy(
            maximumAutomaticAttempts: 3,
            retryDelay: TimeSpan.FromSeconds(7));
        Assert.Equal(3, policy.MaximumAutomaticAttempts);
        Assert.Equal(TimeSpan.FromSeconds(7), policy.RetryDelay);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AI_COMMUNITY_003_NonPositiveAttemptLimitIsRejected(int attempts) =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CommunityConsumerRetryPolicy(attempts, TimeSpan.FromSeconds(1)));

    [Fact]
    public async Task AI_COMMUNITY_003_RetryPublishesBeforeAcknowledgement()
    {
        var calls = new List<string>();
        var dispatcher = new CommunityRecoveryDispatcher(
            new RecordingPublisher(calls));
        var acknowledgement = new RecordingAcknowledgement(calls);
        await dispatcher.DispatchAsync(CommunityRecoveryRoute.Retry,
            Publication(), acknowledgement);
        Assert.Equal(["publish:Retry", "acknowledge"], calls);
    }

    [Fact]
    public async Task AI_COMMUNITY_003_UnconfirmedRecoveryDoesNotAcknowledge()
    {
        var calls = new List<string>();
        var dispatcher = new CommunityRecoveryDispatcher(
            new RecordingPublisher(calls,
                new InvalidOperationException("confirmation failed")));
        var acknowledgement = new RecordingAcknowledgement(calls);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            dispatcher.DispatchAsync(CommunityRecoveryRoute.DeadLetter,
                Publication(), acknowledgement));
        Assert.Equal(["publish:DeadLetter"], calls);
        Assert.Equal(0, acknowledgement.Count);
    }

    [Theory]
    [InlineData(1, true, CommunityRecoveryRoute.Retry)]
    [InlineData(3, true, CommunityRecoveryRoute.DeadLetter)]
    [InlineData(1, false, CommunityRecoveryRoute.DeadLetter)]
    public async Task AI_COMMUNITY_003_HandlerSelectsBoundedRoute(
        int attempt, bool retryable, CommunityRecoveryRoute expected)
    {
        var handler = new CommunityDeliveryRecoveryHandler(
            new CommunityConsumerRetryPolicy(3, TimeSpan.FromSeconds(1)),
            new CommunityRecoveryDispatcher(new RecordingPublisher([])));
        CommunityRecoveryRoute route = await handler.RecoverAsync(EventId, 1,
            new byte[] { 1, 2, 3 }, attempt,
            "processingUnavailable", retryable,
            new RecordingAcknowledgement([]));
        Assert.Equal(expected, route);
    }

    private static CommunityRecoveryPublication Publication() => new(
        EventId, 1, [0, 1, 2, 254, 255], 2, "processingUnavailable");

    private sealed class RecordingPublisher : ICommunityRecoveryPublisher
    {
        private readonly List<string> calls;
        private readonly Exception? exception;
        public RecordingPublisher(List<string> calls, Exception? exception = null)
        { this.calls = calls; this.exception = exception; }
        public Task PublishAsync(CommunityRecoveryRoute route,
            CommunityRecoveryPublication publication,
            CancellationToken cancellationToken = default)
        {
            calls.Add($"publish:{route}");
            return exception is null ? Task.CompletedTask : Task.FromException(exception);
        }
    }

    private sealed class RecordingAcknowledgement : ICommunityDeliveryAcknowledgement
    {
        private readonly List<string> calls;
        public RecordingAcknowledgement(List<string> calls) => this.calls = calls;
        public int Count { get; private set; }
        public Task AcknowledgeAsync(CancellationToken cancellationToken = default)
        { calls.Add("acknowledge"); Count++; return Task.CompletedTask; }
    }
}
