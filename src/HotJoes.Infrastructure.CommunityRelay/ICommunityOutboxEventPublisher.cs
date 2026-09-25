namespace HotJoes.Infrastructure.CommunityRelay;

public interface ICommunityOutboxEventPublisher
{
    Task<CommunityOutboxPublicationConfirmation> PublishAsync(
        CommunityOutboxPublication publication,
        CancellationToken cancellationToken = default);
}
