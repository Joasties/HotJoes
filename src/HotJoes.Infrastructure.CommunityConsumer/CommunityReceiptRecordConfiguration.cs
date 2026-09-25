using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace HotJoes.Infrastructure.CommunityConsumer;
internal sealed class CommunityReceiptRecordConfiguration : IEntityTypeConfiguration<CommunityReceiptRecord>
{
    public void Configure(EntityTypeBuilder<CommunityReceiptRecord> builder)
    {
        builder.ToTable("community_participation_receipts", "community");
        builder.HasKey(x => x.EventId);
        builder.Property(x => x.EventId).HasColumnName("event_id").HasColumnType("uuid");
        builder.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(128).IsRequired();
        builder.Property(x => x.EventVersion).HasColumnName("event_version");
        builder.Property(x => x.CommunityParticipationId).HasColumnName("community_participation_id").HasColumnType("uuid");
        builder.Property(x => x.VendorId).HasColumnName("vendor_id").HasColumnType("uuid");
        builder.Property(x => x.ReceivedAtUtc).HasColumnName("received_at_utc").HasColumnType("timestamp with time zone");
        builder.Property(x => x.SerializedEventSha256).HasColumnName("serialized_event_sha256").HasColumnType("bytea").IsRequired();
        builder.HasIndex(x => x.CommunityParticipationId).HasDatabaseName("ix_community_receipts_participation_id");
        builder.HasIndex(x => x.VendorId).HasDatabaseName("ix_community_receipts_vendor_id");
    }
}
