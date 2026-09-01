using HotJoes.Infrastructure.Persistence;
using HotJoes.Infrastructure.VendorRelay;
using Microsoft.Extensions.Logging;

namespace HotJoes.IntegrationTests;

public sealed class VendorOutboxRelayProcessorTests
{
    private static readonly Guid EventId = Guid.Parse(
        "8224a081-bc72-48d6-8bea-b20c1a77e1ef");

    private static readonly Guid WorkerId = Guid.Parse(
        "1db6666f-d093-4f09-91d4-4d372e6a0a91");

    private static readonly DateTimeOffset AttemptedAtUtc = new(
        2026,
        8,
        28,
        14,
        0,
        0,
        TimeSpan.Zero);

    private static readonly OutboxRelayRetryPolicy RetryPolicy = new(
        TimeSpan.FromSeconds(10),
        TimeSpan.FromMinutes(1),
        automaticAttemptLimit: 3);

    [Fact]
    public async Task ProcessAsync_PositiveConfirmation_PublishesExactStoredEventThenMarksPublished()
    {
        var activity = new List<string>();
        var store = new RecordingRelayStore(activity);
        var publisher = new RecordingPublisher(
            activity,
            OutboxPublicationConfirmation.Confirmed);
        var processor = new VendorOutboxRelayProcessor(store, publisher);
        byte[] storedBytes = [0, 1, 2, 3, 255];
        var claim = new OutboxRelayClaim(EventId, 1, storedBytes);

        OutboxRelayProcessingOutcome outcome = await processor.ProcessAsync(
            claim,
            WorkerId,
            AttemptedAtUtc,
            RetryPolicy);

        Assert.Equal(OutboxRelayProcessingOutcome.Published, outcome);
        Assert.Equal(
            new[] { "publish", "confirmed", "markPublished" },
            activity);

        OutboxPublication publication = Assert.IsType<OutboxPublication>(
            publisher.Publication);
        Assert.Equal(EventId, publication.EventId);
        Assert.Equal(1, publication.EventVersion);
        Assert.Equal(storedBytes, publication.SerializedEvent.ToArray());

        Assert.Equal(EventId, store.PublishedEventId);
        Assert.Equal(WorkerId, store.PublishedByWorkerId);
        Assert.Equal(AttemptedAtUtc, store.PublishedAtUtc);
        Assert.Null(store.FailedEventId);
    }

    [Fact]
    public async Task ProcessAsync_NegativeConfirmation_RecordsRetryWithoutMarkingPublished()
    {
        var activity = new List<string>();
        var store = new RecordingRelayStore(activity);
        var publisher = new RecordingPublisher(
            activity,
            OutboxPublicationConfirmation.NotConfirmed);
        var processor = new VendorOutboxRelayProcessor(store, publisher);
        var claim = new OutboxRelayClaim(EventId, 1, [4, 5, 6]);

        OutboxRelayProcessingOutcome outcome = await processor.ProcessAsync(
            claim,
            WorkerId,
            AttemptedAtUtc,
            RetryPolicy);

        Assert.Equal(OutboxRelayProcessingOutcome.RetryScheduled, outcome);
        Assert.Equal(
            new[] { "publish", "notConfirmed", "recordFailure" },
            activity);
        AssertFailureWasRecorded(store);
        Assert.Null(store.PublishedEventId);
    }

    [Fact]
    public async Task ProcessAsync_PublicationException_RecordsRetryWithoutMarkingPublished()
    {
        var activity = new List<string>();
        var store = new RecordingRelayStore(activity);
        var publisher = new ThrowingPublisher(activity);
        var processor = new VendorOutboxRelayProcessor(store, publisher);
        var claim = new OutboxRelayClaim(EventId, 1, [7, 8, 9]);

        OutboxRelayProcessingOutcome outcome = await processor.ProcessAsync(
            claim,
            WorkerId,
            AttemptedAtUtc,
            RetryPolicy);

        Assert.Equal(OutboxRelayProcessingOutcome.RetryScheduled, outcome);
        Assert.Equal(
            new[] { "publish", "exception", "recordFailure" },
            activity);
        AssertFailureWasRecorded(store);
        Assert.Null(store.PublishedEventId);
    }

    [Fact]
    public void Constructor_RequiresOnlyRelayStorePublisherAndLoggerCollaborators()
    {
        Type[] parameterTypes = Assert.Single(
                typeof(VendorOutboxRelayProcessor).GetConstructors())
            .GetParameters()
            .Select(parameter => parameter.ParameterType)
            .ToArray();

        Assert.Equal(
            new[]
            {
                typeof(IOutboxRelayStore),
                typeof(IOutboxEventPublisher),
                typeof(ILogger<VendorOutboxRelayProcessor>)
            },
            parameterTypes);
    }

    private static void AssertFailureWasRecorded(RecordingRelayStore store)
    {
        Assert.Equal(EventId, store.FailedEventId);
        Assert.Equal(WorkerId, store.FailedByWorkerId);
        Assert.Equal(AttemptedAtUtc, store.FailedAtUtc);
        Assert.Equal(
            OutboxRelayFailureCategory.PublicationFailed,
            store.FailureCategory);
        Assert.Same(RetryPolicy, store.RetryPolicy);
    }

    private sealed class RecordingPublisher : IOutboxEventPublisher
    {
        private readonly List<string> _activity;
        private readonly OutboxPublicationConfirmation _confirmation;

        public RecordingPublisher(
            List<string> activity,
            OutboxPublicationConfirmation confirmation)
        {
            _activity = activity;
            _confirmation = confirmation;
        }

        public OutboxPublication? Publication { get; private set; }

        public Task<OutboxPublicationConfirmation> PublishAsync(
            OutboxPublication publication,
            CancellationToken cancellationToken = default)
        {
            Publication = publication;
            _activity.Add("publish");
            _activity.Add(
                _confirmation == OutboxPublicationConfirmation.Confirmed
                    ? "confirmed"
                    : "notConfirmed");

            return Task.FromResult(_confirmation);
        }
    }

    private sealed class ThrowingPublisher : IOutboxEventPublisher
    {
        private readonly List<string> _activity;

        public ThrowingPublisher(List<string> activity)
        {
            _activity = activity;
        }

        public Task<OutboxPublicationConfirmation> PublishAsync(
            OutboxPublication publication,
            CancellationToken cancellationToken = default)
        {
            _activity.Add("publish");
            _activity.Add("exception");
            throw new OutboxPublicationException("Controlled test failure.");
        }
    }

    private sealed class RecordingRelayStore : IOutboxRelayStore
    {
        private readonly List<string> _activity;

        public RecordingRelayStore(List<string> activity)
        {
            _activity = activity;
        }

        public Guid? PublishedEventId { get; private set; }

        public Guid? PublishedByWorkerId { get; private set; }

        public DateTimeOffset? PublishedAtUtc { get; private set; }

        public Guid? FailedEventId { get; private set; }

        public Guid? FailedByWorkerId { get; private set; }

        public DateTimeOffset? FailedAtUtc { get; private set; }

        public OutboxRelayFailureCategory? FailureCategory { get; private set; }

        public OutboxRelayRetryPolicy? RetryPolicy { get; private set; }

        public Task MarkPublishedAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset publishedAtUtc,
            CancellationToken cancellationToken = default)
        {
            _activity.Add("markPublished");
            PublishedEventId = eventId;
            PublishedByWorkerId = workerId;
            PublishedAtUtc = publishedAtUtc;
            return Task.CompletedTask;
        }

        public Task RecordFailureAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset failedAtUtc,
            OutboxRelayFailureCategory failureCategory,
            OutboxRelayRetryPolicy retryPolicy,
            CancellationToken cancellationToken = default)
        {
            _activity.Add("recordFailure");
            FailedEventId = eventId;
            FailedByWorkerId = workerId;
            FailedAtUtc = failedAtUtc;
            FailureCategory = failureCategory;
            RetryPolicy = retryPolicy;
            return Task.CompletedTask;
        }
    }
}
