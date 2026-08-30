using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotJoes.Infrastructure.ComplianceConsumer;

internal sealed class ComplianceReceiptRecordConfiguration
    : IEntityTypeConfiguration<ComplianceReceiptRecord>
{
    public void Configure(EntityTypeBuilder<ComplianceReceiptRecord> builder)
    {
        builder.ToTable(
            "compliance_vendor_registered_receipts",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_compliance_receipts_event_type",
                    "\"event_type\" = 'VendorRegistered'");
                tableBuilder.HasCheckConstraint(
                    "ck_compliance_receipts_event_version",
                    "\"event_version\" = 1");
                tableBuilder.HasCheckConstraint(
                    "ck_compliance_receipts_sha256",
                    "octet_length(\"serialized_event_sha256\") = 32");
            });

        builder.HasKey(record => record.EventId);

        builder.Property(record => record.EventId)
            .HasColumnName("event_id")
            .HasColumnType("uuid");
        builder.Property(record => record.EventType)
            .HasColumnName("event_type")
            .HasColumnType("character varying(64)")
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(record => record.EventVersion)
            .HasColumnName("event_version")
            .HasColumnType("integer");
        builder.Property(record => record.ReceivedAtUtc)
            .HasColumnName("received_at_utc")
            .HasColumnType("timestamp with time zone");
        builder.Property(record => record.SerializedEventSha256)
            .HasColumnName("serialized_event_sha256")
            .HasColumnType("bytea")
            .IsRequired();
    }
}
