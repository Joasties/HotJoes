namespace HotJoes.Infrastructure.ComplianceConsumer;

public interface IComplianceDeliveryAcknowledgement
{
    Task AcknowledgeAsync(
        CancellationToken cancellationToken = default);
}
