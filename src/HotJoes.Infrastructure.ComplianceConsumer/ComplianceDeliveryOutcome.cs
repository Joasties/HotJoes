namespace HotJoes.Infrastructure.ComplianceConsumer;

public enum ComplianceDeliveryOutcome
{
    AcknowledgedNewReceipt = 1,
    AcknowledgedEquivalentDuplicate = 2,
    InvalidContract = 3,
    ConflictingBytes = 4
}
