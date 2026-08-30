namespace HotJoes.Infrastructure.VendorRelay;

public interface IOutboxEventPublisher
{
    Task<OutboxPublicationConfirmation> PublishAsync(
        OutboxPublication publication,
        CancellationToken cancellationToken = default);
}
