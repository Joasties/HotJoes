namespace HotJoes.Infrastructure.Persistence;

internal sealed class VendorRegistrationOutboxRecord
{
    public Guid EventId { get; set; }

    public Guid VendorId { get; set; }

    public int EventVersion { get; set; }

    public byte[] SerializedEvent { get; set; } = null!;

    public DateTimeOffset? PublishedAtUtc { get; set; }
}
