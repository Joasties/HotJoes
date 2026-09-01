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
                    "ck_vendor_registration_outbox_attempt_count",
                    "\"attempt_count\" >= 0");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outbox_claim",
                    "(\"claimed_by\" IS NULL AND " +
                    "\"claim_expires_at_utc\" IS NULL) OR " +
                    "(\"claimed_by\" IS NOT NULL AND " +
                    "\"claim_expires_at_utc\" IS NOT NULL)");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outbox_event_version",
                    "\"event_version\" > 0");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outbox_serialized_event",
                    "octet_length(\"serialized_event\") > 0");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outbox_stalled",
                    "NOT \"is_stalled\" OR " +
                    "\"next_attempt_at_utc\" IS NULL");
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
        builder.Property(record => record.TraceParent)
            .HasColumnName("trace_parent")
            .HasColumnType("character varying(55)")
            .HasMaxLength(55);
        builder.Property(record => record.TraceState)
            .HasColumnName("trace_state")
            .HasColumnType("character varying(512)")
            .HasMaxLength(512);
        builder.Property(record => record.AttemptCount)
            .HasColumnName("attempt_count")
            .HasColumnType("integer")
            .HasDefaultValue(0);
        builder.Property(record => record.NextAttemptAtUtc)
            .HasColumnName("next_attempt_at_utc")
            .HasColumnType("timestamp with time zone");
        builder.Property(record => record.ClaimedBy)
            .HasColumnName("claimed_by")
            .HasColumnType("uuid");
        builder.Property(record => record.ClaimExpiresAtUtc)
            .HasColumnName("claim_expires_at_utc")
            .HasColumnType("timestamp with time zone");
        builder.Property(record => record.LastAttemptAtUtc)
            .HasColumnName("last_attempt_at_utc")
            .HasColumnType("timestamp with time zone");
        builder.Property(record => record.LastFailureCategory)
            .HasColumnName("last_failure_category")
            .HasColumnType("character varying(64)")
            .HasMaxLength(64)
            .HasConversion<string>();
        builder.Property(record => record.IsStalled)
            .HasColumnName("is_stalled")
            .HasColumnType("boolean")
            .HasDefaultValue(false);
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

        builder.HasIndex(record => new
        {
            record.NextAttemptAtUtc,
            record.ClaimExpiresAtUtc,
            record.EventId
        })
            .HasFilter(
                "published_at_utc IS NULL AND is_stalled = FALSE")
            .HasDatabaseName("ix_vendor_registration_outbox_eligible");
    }
}
