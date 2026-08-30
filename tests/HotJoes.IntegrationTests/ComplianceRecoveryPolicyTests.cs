using HotJoes.Infrastructure.ComplianceConsumer;

namespace HotJoes.IntegrationTests;

public sealed class ComplianceRecoveryPolicyTests
{
    private static readonly Guid EventId = Guid.Parse(
        "0b543f02-042c-43ea-9a85-d18216351df4");

    [Fact]
    public void Constructor_ValidValues_PreservesInjectedPolicyWithoutSelectingProductionValues()
    {
        var policy = new ComplianceConsumerRetryPolicy(
            maximumAutomaticAttempts: 3,
            retryDelay: TimeSpan.FromSeconds(7));

        Assert.Equal(3, policy.MaximumAutomaticAttempts);
        Assert.Equal(TimeSpan.FromSeconds(7), policy.RetryDelay);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NonPositiveAttemptLimit_RejectsPolicy(
        int maximumAutomaticAttempts)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ComplianceConsumerRetryPolicy(
                maximumAutomaticAttempts,
                TimeSpan.FromSeconds(1)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NonPositiveRetryDelay_RejectsPolicy(
        int retryDelayMilliseconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ComplianceConsumerRetryPolicy(
                maximumAutomaticAttempts: 1,
                TimeSpan.FromMilliseconds(retryDelayMilliseconds)));
    }

    [Fact]
    public async Task DispatchAsync_ConfirmedRetry_AcknowledgesOnlyAfterPublication()
    {
        var calls = new List<string>();
        var publisher = new RecordingRecoveryPublisher(calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var dispatcher = new ComplianceRecoveryDispatcher(publisher);
        ComplianceRecoveryPublication publication = CreatePublication();

        await dispatcher.DispatchAsync(
            ComplianceRecoveryRoute.Retry,
            publication,
            acknowledgement);

        Assert.Equal(["publish:Retry", "acknowledge"], calls);
        Assert.Same(publication, Assert.Single(publisher.Publications));
    }

    [Fact]
    public async Task DispatchAsync_ConfirmedDeadLetter_AcknowledgesOnlyAfterPublication()
    {
        var calls = new List<string>();
        var publisher = new RecordingRecoveryPublisher(calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var dispatcher = new ComplianceRecoveryDispatcher(publisher);

        await dispatcher.DispatchAsync(
            ComplianceRecoveryRoute.DeadLetter,
            CreatePublication(),
            acknowledgement);

        Assert.Equal(["publish:DeadLetter", "acknowledge"], calls);
    }

    [Fact]
    public async Task DispatchAsync_UnconfirmedPublication_DoesNotAcknowledge()
    {
        var calls = new List<string>();
        var publisher = new RecordingRecoveryPublisher(
            calls,
            new InvalidOperationException("publisher confirmation failed"));
        var acknowledgement = new RecordingAcknowledgement(calls);
        var dispatcher = new ComplianceRecoveryDispatcher(publisher);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            dispatcher.DispatchAsync(
                ComplianceRecoveryRoute.Retry,
                CreatePublication(),
                acknowledgement));

        Assert.Equal(["publish:Retry"], calls);
        Assert.Equal(0, acknowledgement.CallCount);
    }

    private static ComplianceRecoveryPublication CreatePublication()
    {
        return new ComplianceRecoveryPublication(
            EventId,
            eventVersion: 1,
            serializedEvent: [0, 1, 2, 254, 255],
            automaticAttempt: 2,
            failureCategory: "receiptUnavailable");
    }

    private sealed class RecordingRecoveryPublisher
        : IComplianceRecoveryPublisher
    {
        private readonly List<string> _calls;
        private readonly Exception? _exception;

        public RecordingRecoveryPublisher(
            List<string> calls,
            Exception? exception = null)
        {
            _calls = calls;
            _exception = exception;
        }

        public List<ComplianceRecoveryPublication> Publications { get; } = [];

        public Task PublishAsync(
            ComplianceRecoveryRoute route,
            ComplianceRecoveryPublication publication,
            CancellationToken cancellationToken = default)
        {
            _calls.Add($"publish:{route}");
            Publications.Add(publication);

            if (_exception is not null)
            {
                throw _exception;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class RecordingAcknowledgement
        : IComplianceDeliveryAcknowledgement
    {
        private readonly List<string> _calls;

        public RecordingAcknowledgement(List<string> calls)
        {
            _calls = calls;
        }

        public int CallCount { get; private set; }

        public Task AcknowledgeAsync(
            CancellationToken cancellationToken = default)
        {
            _calls.Add("acknowledge");
            CallCount++;
            return Task.CompletedTask;
        }
    }
}
