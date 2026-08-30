namespace HotJoes.Infrastructure.ComplianceConsumer;

public interface IComplianceReceiptStore
{
    Task<ComplianceReceiptOutcome> RecordAsync(
        ComplianceReceiptCandidate candidate,
        CancellationToken cancellationToken = default);
}
