namespace HotJoes.Infrastructure.CommunityConsumer;

public enum CommunityConsumerRunOutcome { NoDelivery = 1, AcknowledgedNewReceipt = 2, AcknowledgedEquivalentDuplicate = 3, InvalidContract = 4, ConflictingBytes = 5 }
