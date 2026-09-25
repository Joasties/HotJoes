using System.Diagnostics;
using HotJoes.Infrastructure.Community.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotJoes.Infrastructure.CommunityRelay;

public sealed class CommunityOutboxRelayProcessor
{
    private static readonly ActivitySource ActivitySource = new(
        "HotJoes.Infrastructure.CommunityRelay");
    private readonly ICommunityOutboxRelayStore store;
    private readonly ICommunityOutboxEventPublisher publisher;
    private readonly ILogger<CommunityOutboxRelayProcessor> logger;

    public CommunityOutboxRelayProcessor(ICommunityOutboxRelayStore store,
        ICommunityOutboxEventPublisher publisher,
        ILogger<CommunityOutboxRelayProcessor>? logger = null)
    {
        this.store = store ?? throw new ArgumentNullException(nameof(store));
        this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        this.logger = logger ?? NullLogger<CommunityOutboxRelayProcessor>.Instance;
    }

    public async Task<CommunityOutboxRelayProcessingOutcome> ProcessAsync(
        CommunityOutboxRelayClaim claim, Guid workerId,
        DateTimeOffset attemptedAtUtc, CommunityOutboxRelayRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(retryPolicy);
        using Activity? activity = StartActivity(claim, out string? parent,
            out string? state);
        var publication = new CommunityOutboxPublication(claim.EventId,
            claim.EventVersion, claim.SerializedEvent.Span, parent, state);
        try
        {
            var confirmation = await publisher.PublishAsync(publication,
                cancellationToken);
            if (confirmation == CommunityOutboxPublicationConfirmation.Confirmed)
            {
                await store.MarkPublishedAsync(claim.EventId, workerId,
                    attemptedAtUtc, cancellationToken);
                logger.LogInformation("Community relay published event {EventId} version {EventVersion}", claim.EventId, claim.EventVersion);
                return CommunityOutboxRelayProcessingOutcome.Published;
            }
        }
        catch (CommunityOutboxPublicationException)
        {
        }
        await store.RecordFailureAsync(claim.EventId, workerId, attemptedAtUtc,
            CommunityOutboxRelayFailureCategory.PublicationFailed, retryPolicy,
            cancellationToken);
        logger.LogWarning("Community relay scheduled retry for event {EventId} version {EventVersion}", claim.EventId, claim.EventVersion);
        return CommunityOutboxRelayProcessingOutcome.RetryScheduled;
    }

    private static Activity? StartActivity(CommunityOutboxRelayClaim claim,
        out string? traceParent, out string? traceState)
    {
        if (ActivityContext.TryParse(claim.TraceParent, claim.TraceState, true,
            out ActivityContext context))
        {
            traceParent = claim.TraceParent;
            traceState = claim.TraceState;
            return ActivitySource.StartActivity("community outbox publish",
                ActivityKind.Producer, context);
        }
        traceParent = null;
        traceState = null;
        return ActivitySource.StartActivity("community outbox publish",
            ActivityKind.Producer);
    }
}
