using HotJoes.Infrastructure.Community.Persistence;
using HotJoes.Infrastructure.CommunityRelay;
using Microsoft.Extensions.Logging;

namespace HotJoes.IntegrationTests;

public sealed class CommunityOutboxRelayProcessorTests
{
    private static readonly Guid EventId = Guid.Parse(
        "82b27e4d-c889-441d-b2d6-d2d3d63f222c");
    private static readonly Guid WorkerId = Guid.Parse(
        "126a4cbb-1524-44c0-a027-b777537bde0c");
    private static readonly DateTimeOffset AttemptedAtUtc = new(
        2026, 9, 23, 12, 0, 0, TimeSpan.Zero);
    private static readonly CommunityOutboxRelayRetryPolicy RetryPolicy = new(
        TimeSpan.FromSeconds(10),
        TimeSpan.FromMinutes(1),
        automaticAttemptLimit: 3);

    [Fact]
    public async Task AI_EVT_005_ConfirmedPublicationUsesExactStoredBytesThenMarksPublished()
    {
        var activity = new List<string>();
        var store = new RecordingStore(activity);
        var publisher = new RecordingPublisher(
            activity,
            CommunityOutboxPublicationConfirmation.Confirmed);
        var processor = new CommunityOutboxRelayProcessor(store, publisher);
        byte[] storedBytes = [0, 1, 2, 3, 254, 255];
        var claim = new CommunityOutboxRelayClaim(
            EventId,
            eventVersion: 1,
            storedBytes,
            traceParent: null,
            traceState: null,
            attemptCount: 0);

        CommunityOutboxRelayProcessingOutcome outcome =
            await processor.ProcessAsync(
                claim,
                WorkerId,
                AttemptedAtUtc,
                RetryPolicy);

        Assert.Equal(CommunityOutboxRelayProcessingOutcome.Published, outcome);
        Assert.Equal(["publish", "confirmed", "markPublished"], activity);
        CommunityOutboxPublication publication = Assert.IsType<
            CommunityOutboxPublication>(publisher.Publication);
        Assert.Equal(EventId, publication.EventId);
        Assert.Equal(1, publication.EventVersion);
        Assert.Equal(storedBytes, publication.SerializedEvent.ToArray());
        Assert.Equal(EventId, store.PublishedEventId);
        Assert.Null(store.FailedEventId);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AI_EVT_005_UnconfirmedOrFailedPublicationSchedulesRetry(
        bool throws)
    {
        var activity = new List<string>();
        var store = new RecordingStore(activity);
        ICommunityOutboxEventPublisher publisher = throws
            ? new ThrowingPublisher(activity)
            : new RecordingPublisher(
                activity,
                CommunityOutboxPublicationConfirmation.NotConfirmed);
        var processor = new CommunityOutboxRelayProcessor(store, publisher);
        var claim = new CommunityOutboxRelayClaim(EventId, 1, [7, 8, 9]);

        CommunityOutboxRelayProcessingOutcome outcome =
            await processor.ProcessAsync(
                claim,
                WorkerId,
                AttemptedAtUtc,
                RetryPolicy);

        Assert.Equal(
            CommunityOutboxRelayProcessingOutcome.RetryScheduled,
            outcome);
        Assert.Equal(EventId, store.FailedEventId);
        Assert.Equal(
            CommunityOutboxRelayFailureCategory.PublicationFailed,
            store.FailureCategory);
        Assert.Null(store.PublishedEventId);
    }

    [Fact]
    public void AI_COMMUNITY_001_ProcessorHasOnlyStorePublisherAndLoggerCollaborators()
    {
        Type[] parameterTypes = Assert.Single(
                typeof(CommunityOutboxRelayProcessor).GetConstructors())
            .GetParameters()
            .Select(parameter => parameter.ParameterType)
            .ToArray();

        Assert.Equal(
            [
                typeof(ICommunityOutboxRelayStore),
                typeof(ICommunityOutboxEventPublisher),
                typeof(ILogger<CommunityOutboxRelayProcessor>)
            ],
            parameterTypes);
    }

    private sealed class RecordingPublisher : ICommunityOutboxEventPublisher
    {
        private readonly List<string> activity;
        private readonly CommunityOutboxPublicationConfirmation confirmation;

        public RecordingPublisher(
            List<string> activity,
            CommunityOutboxPublicationConfirmation confirmation)
        {
            this.activity = activity;
            this.confirmation = confirmation;
        }

        public CommunityOutboxPublication? Publication { get; private set; }

        public Task<CommunityOutboxPublicationConfirmation> PublishAsync(
            CommunityOutboxPublication publication,
            CancellationToken cancellationToken = default)
        {
            Publication = publication;
            activity.Add("publish");
            activity.Add(confirmation ==
                CommunityOutboxPublicationConfirmation.Confirmed
                    ? "confirmed"
                    : "notConfirmed");
            return Task.FromResult(confirmation);
        }
    }

    private sealed class ThrowingPublisher : ICommunityOutboxEventPublisher
    {
        private readonly List<string> activity;

        public ThrowingPublisher(List<string> activity) =>
            this.activity = activity;

        public Task<CommunityOutboxPublicationConfirmation> PublishAsync(
            CommunityOutboxPublication publication,
            CancellationToken cancellationToken = default)
        {
            activity.Add("publish");
            throw new CommunityOutboxPublicationException(
                "Controlled test failure.");
        }
    }

    private sealed class RecordingStore : ICommunityOutboxRelayStore
    {
        private readonly List<string> activity;

        public RecordingStore(List<string> activity) =>
            this.activity = activity;

        public Guid? PublishedEventId { get; private set; }
        public Guid? FailedEventId { get; private set; }
        public CommunityOutboxRelayFailureCategory? FailureCategory { get; private set; }

        public Task MarkPublishedAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset publishedAtUtc,
            CancellationToken cancellationToken = default)
        {
            activity.Add("markPublished");
            PublishedEventId = eventId;
            return Task.CompletedTask;
        }

        public Task RecordFailureAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset failedAtUtc,
            CommunityOutboxRelayFailureCategory failureCategory,
            CommunityOutboxRelayRetryPolicy retryPolicy,
            CancellationToken cancellationToken = default)
        {
            activity.Add("recordFailure");
            FailedEventId = eventId;
            FailureCategory = failureCategory;
            return Task.CompletedTask;
        }
    }
}
