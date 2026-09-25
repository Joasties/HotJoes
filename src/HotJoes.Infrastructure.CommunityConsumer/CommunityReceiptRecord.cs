namespace HotJoes.Infrastructure.CommunityConsumer;
public sealed class CommunityReceiptRecord
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = null!;
    public int EventVersion { get; set; }
    public Guid CommunityParticipationId { get; set; }
    public Guid VendorId { get; set; }
    public DateTimeOffset ReceivedAtUtc { get; set; }
    public byte[] SerializedEventSha256 { get; set; } = null!;
}
