namespace HotJoes.Infrastructure.CommunityConsumer;

public enum CommunityDeliveryOutcome { AcknowledgedNewReceipt = 1, AcknowledgedEquivalentDuplicate = 2, InvalidContract = 3, ConflictingBytes = 4 }
