using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotJoes.Infrastructure.Persistence;

internal sealed class VendorRegistrationOutboxRecordConfiguration
    : IEntityTypeConfiguration<VendorRegistrationOutboxRecord>
{
    public void Configure(
        EntityTypeBuilder<VendorRegistrationOutboxRecord> builder)
    {
        builder.ToTable(
            "vendor_registration_outbox",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outbox_event_version",
                    "\"event_version\" > 0");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outbox_serialized_event",
                    "octet_length(\"serialized_event\") > 0");
            });

        builder.HasKey(record => record.EventId);

        builder.Property(record => record.EventId)
            .HasColumnName("event_id")
            .HasColumnType("uuid");
        builder.Property(record => record.VendorId)
            .HasColumnName("vendor_id")
            .HasColumnType("uuid");
        builder.Property(record => record.EventVersion)
            .HasColumnName("event_version")
            .HasColumnType("integer");
        builder.Property(record => record.SerializedEvent)
            .HasColumnName("serialized_event")
            .HasColumnType("bytea")
            .IsRequired();
        builder.Property(record => record.PublishedAtUtc)
            .HasColumnName("published_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.HasOne<VendorRegistrationRecord>()
            .WithOne()
            .HasForeignKey<VendorRegistrationOutboxRecord>(
                record => record.VendorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(record => record.VendorId)
            .IsUnique()
            .HasDatabaseName("uq_vendor_registration_outbox_vendor_id");

        builder.HasIndex(record => record.EventId)
            .HasFilter("published_at_utc IS NULL")
            .HasDatabaseName("ix_vendor_registration_outbox_unpublished");
    }
}
