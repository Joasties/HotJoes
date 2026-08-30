namespace HotJoes.Infrastructure.ComplianceConsumer;

public enum ComplianceConsumerRunOutcome
{
    NoDelivery = 1,
    AcknowledgedNewReceipt = 2,
    AcknowledgedEquivalentDuplicate = 3,
    InvalidContract = 4,
    ConflictingBytes = 5,
    Retried = 6,
    DeadLettered = 7
}
