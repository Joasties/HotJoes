namespace HotJoes.Infrastructure.CommunityConsumer;
public interface ICommunityRecoveryPublisher
{
    Task PublishAsync(CommunityRecoveryRoute route,
        CommunityRecoveryPublication publication,
        CancellationToken cancellationToken = default);
}
