namespace HotJoes.Infrastructure.ComplianceConsumer;

public enum ComplianceReceiptOutcome
{
    Recorded = 1,
    EquivalentDuplicate = 2,
    ConflictingBytes = 3
}
