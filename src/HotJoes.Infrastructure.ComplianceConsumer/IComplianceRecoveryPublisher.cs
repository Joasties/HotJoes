namespace HotJoes.Infrastructure.ComplianceConsumer;

public interface IComplianceRecoveryPublisher
{
    Task PublishAsync(
        ComplianceRecoveryRoute route,
        ComplianceRecoveryPublication publication,
        CancellationToken cancellationToken = default);
}
