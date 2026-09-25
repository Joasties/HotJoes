namespace HotJoes.Infrastructure.CommunityConsumer;
public sealed class CommunityDeliveryRecoveryHandler
{
    private readonly CommunityConsumerRetryPolicy policy;
    private readonly CommunityRecoveryDispatcher dispatcher;
    public CommunityDeliveryRecoveryHandler(CommunityConsumerRetryPolicy policy,
        CommunityRecoveryDispatcher dispatcher)
    { this.policy = policy; this.dispatcher = dispatcher; }
    public async Task<CommunityRecoveryRoute> RecoverAsync(Guid eventId,
        int eventVersion, ReadOnlyMemory<byte> serializedEvent,
        int currentAutomaticAttempt, string failureCategory, bool retryable,
        ICommunityDeliveryAcknowledgement acknowledgement,
        CancellationToken cancellationToken = default)
    {
        if (currentAutomaticAttempt <= 0) throw new ArgumentOutOfRangeException(nameof(currentAutomaticAttempt));
        CommunityRecoveryRoute route = retryable && currentAutomaticAttempt < policy.MaximumAutomaticAttempts
            ? CommunityRecoveryRoute.Retry : CommunityRecoveryRoute.DeadLetter;
        int next = route == CommunityRecoveryRoute.Retry ? checked(currentAutomaticAttempt + 1) : currentAutomaticAttempt;
        await dispatcher.DispatchAsync(route, new CommunityRecoveryPublication(
            eventId, eventVersion, serializedEvent.Span, next, failureCategory),
            acknowledgement, cancellationToken);
        return route;
    }
}
