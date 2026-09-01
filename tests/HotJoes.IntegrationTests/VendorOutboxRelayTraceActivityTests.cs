using System.Diagnostics;
using HotJoes.Infrastructure.Persistence;
using HotJoes.Infrastructure.VendorRelay;

namespace HotJoes.IntegrationTests;

[Collection(RelayObservabilityTestCollection.Name)]
public sealed class VendorOutboxRelayTraceActivityTests
{
    private const string TraceParent =
        "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
    private const string TraceState = "vendor=hotjoes";

    [Fact]
    public async Task ProcessAsync_PersistedContext_CreatesLinkedProducerActivityAndPreservesPublication()
    {
        ActivityContext.TryParse(
            TraceParent,
            TraceState,
            isRemote: true,
            out ActivityContext originatingContext);
        var publisher = new ActivityRecordingPublisher();
        var processor = new VendorOutboxRelayProcessor(
            new SuccessfulRelayStore(),
            publisher);
        byte[] storedBytes = [0, 1, 2, 3, 254, 255];
        var claim = new OutboxRelayClaim(
            Guid.Parse("d5268294-cdf7-4ccf-a7dd-b126245b75de"),
            1,
            storedBytes,
            TraceParent,
            TraceState);
        using ActivityListener listener = ListenToAllActivities();

        OutboxRelayProcessingOutcome outcome = await processor.ProcessAsync(
            claim,
            Guid.Parse("fb1de593-4a42-45d3-a156-13724eb804eb"),
            new DateTimeOffset(2026, 8, 29, 15, 0, 0, TimeSpan.Zero),
            new OutboxRelayRetryPolicy(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(4),
                automaticAttemptLimit: 3));

        Assert.Equal(OutboxRelayProcessingOutcome.Published, outcome);
        Assert.Equal(ActivityKind.Producer, publisher.ActivityKind);
        Assert.Equal(ActivityIdFormat.W3C, publisher.ActivityIdFormat);
        Assert.Equal(originatingContext.TraceId, publisher.TraceId);
        Assert.Equal(originatingContext.SpanId, publisher.ParentSpanId);
        Assert.Equal(TraceState, publisher.TraceState);

        OutboxPublication publication = Assert.IsType<OutboxPublication>(
            publisher.Publication);
        Assert.Equal(claim.EventId, publication.EventId);
        Assert.Equal(claim.EventVersion, publication.EventVersion);
        Assert.Equal(storedBytes, publication.SerializedEvent.ToArray());
        Assert.Equal(TraceParent, publication.TraceParent);
        Assert.Equal(TraceState, publication.TraceState);
    }

    private static ActivityListener ListenToAllActivities()
    {
        var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllDataAndRecorded,
            SampleUsingParentId = static (
                ref ActivityCreationOptions<string> _) =>
                    ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);
        return listener;
    }

    private sealed class ActivityRecordingPublisher : IOutboxEventPublisher
    {
        public OutboxPublication? Publication { get; private set; }

        public ActivityKind? ActivityKind { get; private set; }

        public ActivityIdFormat? ActivityIdFormat { get; private set; }

        public ActivityTraceId? TraceId { get; private set; }

        public ActivitySpanId? ParentSpanId { get; private set; }

        public string? TraceState { get; private set; }

        public Task<OutboxPublicationConfirmation> PublishAsync(
            OutboxPublication publication,
            CancellationToken cancellationToken = default)
        {
            Publication = publication;
            Activity? activity = Activity.Current;
            Assert.NotNull(activity);
            ActivityKind = activity.Kind;
            ActivityIdFormat = activity.IdFormat;
            TraceId = activity.TraceId;
            ParentSpanId = activity.ParentSpanId;
            TraceState = activity.TraceStateString;

            return Task.FromResult(
                OutboxPublicationConfirmation.Confirmed);
        }
    }

    private sealed class SuccessfulRelayStore : IOutboxRelayStore
    {
        public Task MarkPublishedAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset publishedAtUtc,
            CancellationToken cancellationToken = default)
        {
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
            throw new Xunit.Sdk.XunitException(
                "A confirmed publication must not record a failure.");
        }
    }
}
