namespace HotJoes.Infrastructure.CommunityConsumer;
public interface ICommunityDeliveryAcknowledgement
{
    Task AcknowledgeAsync(CancellationToken cancellationToken = default);
}
