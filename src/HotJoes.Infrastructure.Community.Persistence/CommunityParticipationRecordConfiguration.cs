using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotJoes.Infrastructure.Community.Persistence;

internal sealed class CommunityParticipationRecordConfiguration
    : IEntityTypeConfiguration<CommunityParticipationRecord>
{
    public void Configure(
        EntityTypeBuilder<CommunityParticipationRecord> builder)
    {
        builder.ToTable(
            "community_participations",
            "community",
            tableBuilder => tableBuilder.HasCheckConstraint(
                "ck_community_participations_contact_preference",
                "\"contact_preference\" IN ('email', 'sms', 'whatsApp')"));

        builder.HasKey(record => record.CommunityParticipationId);
        builder.Property(record => record.CommunityParticipationId)
            .HasColumnName("community_participation_id")
            .HasColumnType("uuid");
        builder.Property(record => record.VendorId)
            .HasColumnName("vendor_id")
            .HasColumnType("uuid");
        builder.Property(record => record.ContactPreference)
            .HasColumnName("contact_preference")
            .HasColumnType("character varying(16)")
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(record => record.JoinedAtUtc)
            .HasColumnName("joined_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(record => record.VendorId)
            .IsUnique()
            .HasDatabaseName("uq_community_participations_vendor_id");
    }
}
