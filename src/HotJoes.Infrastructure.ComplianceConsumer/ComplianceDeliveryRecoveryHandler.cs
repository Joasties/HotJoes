using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class ComplianceDeliveryRecoveryHandler
{
    private readonly ComplianceConsumerRetryPolicy _policy;
    private readonly ComplianceRecoveryDispatcher _dispatcher;
    private readonly ILogger<ComplianceDeliveryRecoveryHandler> _logger;

    public ComplianceDeliveryRecoveryHandler(
        ComplianceConsumerRetryPolicy policy,
        ComplianceRecoveryDispatcher dispatcher,
        ILogger<ComplianceDeliveryRecoveryHandler>? logger = null)
    {
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _dispatcher = dispatcher ??
            throw new ArgumentNullException(nameof(dispatcher));
        _logger = logger ??
            NullLogger<ComplianceDeliveryRecoveryHandler>.Instance;
    }

    public async Task<ComplianceRecoveryRoute> RecoverAsync(
        Guid eventId,
        int eventVersion,
        ReadOnlyMemory<byte> serializedEvent,
        int currentAutomaticAttempt,
        string failureCategory,
        bool retryable,
        IComplianceDeliveryAcknowledgement acknowledgement,
        CancellationToken cancellationToken = default)
    {
        if (currentAutomaticAttempt <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentAutomaticAttempt));
        }

        ComplianceRecoveryRoute route =
            retryable &&
            currentAutomaticAttempt < _policy.MaximumAutomaticAttempts
                ? ComplianceRecoveryRoute.Retry
                : ComplianceRecoveryRoute.DeadLetter;
        int publicationAttempt = route == ComplianceRecoveryRoute.Retry
            ? checked(currentAutomaticAttempt + 1)
            : currentAutomaticAttempt;
        var publication = new ComplianceRecoveryPublication(
            eventId,
            eventVersion,
            serializedEvent.Span,
            publicationAttempt,
            failureCategory);

        await _dispatcher.DispatchAsync(
            route,
            publication,
            acknowledgement,
            cancellationToken);

        RecordRecovery(publication, route);

        return route;
    }

    private void RecordRecovery(
        ComplianceRecoveryPublication publication,
        ComplianceRecoveryRoute route)
    {
        string outcome = route == ComplianceRecoveryRoute.Retry
            ? "retry"
            : "deadLetter";

        _logger.LogWarning(
            "Compliance recovery {ConsumerRecoveryOutcome} for " +
            "{EventType} event {EventId} version {EventVersion} on " +
            "consumer attempt {ConsumerAttempt} after {FailureCategory}",
            outcome,
            "VendorRegistered",
            publication.EventId,
            publication.EventVersion,
            publication.AutomaticAttempt,
            publication.FailureCategory);
        ComplianceConsumerMetrics.RecordRecoveryRoute(outcome);
    }
}
