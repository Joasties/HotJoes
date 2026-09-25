namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed class CommunityRecoveryDispatcher
{
    private readonly ICommunityRecoveryPublisher publisher;
    public CommunityRecoveryDispatcher(ICommunityRecoveryPublisher publisher) => this.publisher = publisher;
    public async Task DispatchAsync(CommunityRecoveryRoute route,
        CommunityRecoveryPublication publication,
        ICommunityDeliveryAcknowledgement acknowledgement,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(route)) throw new ArgumentOutOfRangeException(nameof(route));
        await publisher.PublishAsync(route, publication, cancellationToken);
        await acknowledgement.AcknowledgeAsync(cancellationToken);
    }
}
