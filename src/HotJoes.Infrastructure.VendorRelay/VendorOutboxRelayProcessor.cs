using System.Diagnostics;
using System.Diagnostics.Metrics;
using HotJoes.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotJoes.Infrastructure.VendorRelay;

public sealed class VendorOutboxRelayProcessor
{
    private static readonly ActivitySource RelayActivitySource = new(
        "HotJoes.Infrastructure.VendorRelay");
    private static readonly Meter RelayMeter = new(
        "HotJoes.Infrastructure.VendorRelay");
    private static readonly Counter<long> PublicationCounter =
        RelayMeter.CreateCounter<long>(
            "hotjoes.vendor.relay.publications");

    private readonly IOutboxRelayStore _store;
    private readonly IOutboxEventPublisher _publisher;
    private readonly ILogger<VendorOutboxRelayProcessor> _logger;

    public VendorOutboxRelayProcessor(
        IOutboxRelayStore store,
        IOutboxEventPublisher publisher,
        ILogger<VendorOutboxRelayProcessor>? logger = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _publisher = publisher ??
            throw new ArgumentNullException(nameof(publisher));
        _logger = logger ?? NullLogger<VendorOutboxRelayProcessor>.Instance;
    }

    public async Task<OutboxRelayProcessingOutcome> ProcessAsync(
        OutboxRelayClaim claim,
        Guid workerId,
        DateTimeOffset attemptedAtUtc,
        OutboxRelayRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(retryPolicy);

        using Activity? publicationActivity = StartPublicationActivity(
            claim,
            out string? traceParent,
            out string? traceState);

        var publication = new OutboxPublication(
            claim.EventId,
            claim.EventVersion,
            claim.SerializedEvent.Span,
            traceParent,
            traceState);

        OutboxPublicationConfirmation confirmation;

        try
        {
            confirmation = await _publisher.PublishAsync(
                publication,
                cancellationToken);
        }
        catch (OutboxPublicationException)
        {
            await RecordFailureAsync(
                claim,
                workerId,
                attemptedAtUtc,
                retryPolicy,
                cancellationToken);

            RecordRetryScheduled(claim);

            return OutboxRelayProcessingOutcome.RetryScheduled;
        }

        if (confirmation != OutboxPublicationConfirmation.Confirmed)
        {
            await RecordFailureAsync(
                claim,
                workerId,
                attemptedAtUtc,
                retryPolicy,
                cancellationToken);

            RecordRetryScheduled(claim);

            return OutboxRelayProcessingOutcome.RetryScheduled;
        }

        await _store.MarkPublishedAsync(
            claim.EventId,
            workerId,
            attemptedAtUtc,
            cancellationToken);

        RecordPublished(claim);

        return OutboxRelayProcessingOutcome.Published;
    }

    private void RecordPublished(OutboxRelayClaim claim)
    {
        int attempt = checked(claim.AttemptCount + 1);
        const string outcome = "published";

        _logger.LogInformation(
            "Relay {RelayOutcome} {EventType} event {EventId} " +
            "version {EventVersion} on outbox attempt {OutboxAttempt}",
            outcome,
            "VendorRegistered",
            claim.EventId,
            claim.EventVersion,
            attempt);
        PublicationCounter.Add(
            1,
            new KeyValuePair<string, object?>("outcome", outcome));
    }

    private void RecordRetryScheduled(OutboxRelayClaim claim)
    {
        int attempt = checked(claim.AttemptCount + 1);
        const string outcome = "retryScheduled";

        _logger.LogWarning(
            "Relay {RelayOutcome} {EventType} event {EventId} " +
            "version {EventVersion} on outbox attempt {OutboxAttempt}",
            outcome,
            "VendorRegistered",
            claim.EventId,
            claim.EventVersion,
            attempt);
        PublicationCounter.Add(
            1,
            new KeyValuePair<string, object?>("outcome", outcome));
    }

    private static Activity? StartPublicationActivity(
        OutboxRelayClaim claim,
        out string? traceParent,
        out string? traceState)
    {
        if (ActivityContext.TryParse(
            claim.TraceParent,
            claim.TraceState,
            isRemote: true,
            out ActivityContext parentContext))
        {
            traceParent = claim.TraceParent;
            traceState = claim.TraceState;
            return RelayActivitySource.StartActivity(
                "outbox publish",
                ActivityKind.Producer,
                parentContext);
        }

        traceParent = null;
        traceState = null;
        return RelayActivitySource.StartActivity(
            "outbox publish",
            ActivityKind.Producer);
    }

    private Task RecordFailureAsync(
        OutboxRelayClaim claim,
        Guid workerId,
        DateTimeOffset failedAtUtc,
        OutboxRelayRetryPolicy retryPolicy,
        CancellationToken cancellationToken)
    {
        return _store.RecordFailureAsync(
            claim.EventId,
            workerId,
            failedAtUtc,
            OutboxRelayFailureCategory.PublicationFailed,
            retryPolicy,
            cancellationToken);
    }
}
