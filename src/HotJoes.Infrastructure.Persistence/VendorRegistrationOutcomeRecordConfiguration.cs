using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotJoes.Infrastructure.Persistence;

internal sealed class VendorRegistrationOutcomeRecordConfiguration
    : IEntityTypeConfiguration<VendorRegistrationOutcomeRecord>
{
    public void Configure(
        EntityTypeBuilder<VendorRegistrationOutcomeRecord> builder)
    {
        builder.ToTable(
            "vendor_registration_outcomes",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outcomes_fingerprint_version",
                    "\"fingerprint_version\" > 0");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outcomes_fingerprint_sha256",
                    "octet_length(\"semantic_fingerprint_sha256\") = 32");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registration_outcomes_result_vendor_state",
                    "\"result_vendor_state\" = 'pendingActivation'");
            });

        builder.HasKey(record => record.VendorId);

        builder.Property(record => record.VendorId)
            .HasColumnName("vendor_id")
            .HasColumnType("uuid");
        builder.Property(record => record.FingerprintVersion)
            .HasColumnName("fingerprint_version")
            .HasColumnType("smallint");
        builder.Property(record => record.SemanticFingerprintSha256)
            .HasColumnName("semantic_fingerprint_sha256")
            .HasColumnType("bytea")
            .IsRequired();
        builder.Property(record => record.ResultVendorState)
            .HasColumnName("result_vendor_state")
            .HasColumnType("character varying(32)")
            .HasMaxLength(32)
            .IsRequired();

        builder.HasOne<VendorRegistrationRecord>()
            .WithOne()
            .HasForeignKey<VendorRegistrationOutcomeRecord>(
                record => record.VendorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
