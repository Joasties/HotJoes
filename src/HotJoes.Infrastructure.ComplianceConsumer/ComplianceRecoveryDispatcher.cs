namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class ComplianceRecoveryDispatcher
{
    private readonly IComplianceRecoveryPublisher _publisher;

    public ComplianceRecoveryDispatcher(IComplianceRecoveryPublisher publisher)
    {
        _publisher = publisher ??
            throw new ArgumentNullException(nameof(publisher));
    }

    public async Task DispatchAsync(
        ComplianceRecoveryRoute route,
        ComplianceRecoveryPublication publication,
        IComplianceDeliveryAcknowledgement acknowledgement,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(route))
        {
            throw new ArgumentOutOfRangeException(nameof(route));
        }

        ArgumentNullException.ThrowIfNull(publication);
        ArgumentNullException.ThrowIfNull(acknowledgement);

        await _publisher.PublishAsync(
            route,
            publication,
            cancellationToken);
        await acknowledgement.AcknowledgeAsync(cancellationToken);
    }
}
