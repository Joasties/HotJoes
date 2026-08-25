using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotJoes.Infrastructure.Persistence;

internal sealed class VendorRegistrationRecordConfiguration
    : IEntityTypeConfiguration<VendorRegistrationRecord>
{
    public void Configure(EntityTypeBuilder<VendorRegistrationRecord> builder)
    {
        builder.ToTable(
            "vendor_registrations",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registrations_vendor_state",
                    "\"vendor_state\" IN ('pendingActivation', 'activated', 'suspended', 'deactivated')");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registrations_trading_preference",
                    "\"trading_preference\" IN ('offline', 'online')");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registrations_legal_operator_type",
                    "\"legal_operator_type\" IN ('soleTrader', 'generalPartnership', 'limitedCompany', 'limitedLiabilityPartnership', 'charitableCommunityGroup', 'charitableIncorporatedOrganisation')");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registrations_trading_location",
                    "\"trading_location\" IN ('restaurant', 'stall', 'kitchen')");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registrations_company_registration_number",
                    "((\"legal_operator_type\" IN ('limitedCompany', 'limitedLiabilityPartnership', 'charitableIncorporatedOrganisation') AND \"company_registration_number\" IS NOT NULL) OR (\"legal_operator_type\" NOT IN ('limitedCompany', 'limitedLiabilityPartnership', 'charitableIncorporatedOrganisation') AND \"company_registration_number\" IS NULL))");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registrations_primary_trading_authority",
                    "((\"trading_location\" = 'stall' AND \"primary_trading_authority\" IS NOT NULL) OR (\"trading_location\" <> 'stall' AND \"primary_trading_authority\" IS NULL))");
                tableBuilder.HasCheckConstraint(
                    "ck_vendor_registrations_normalized_names",
                    "length(\"normalized_trading_name\") > 0 AND \"normalized_trading_name\" = lower(btrim(\"normalized_trading_name\")) AND length(\"normalized_legal_operator_name\") > 0 AND \"normalized_legal_operator_name\" = lower(btrim(\"normalized_legal_operator_name\"))");
            });

        builder.HasKey(record => record.VendorId);

        builder.Property(record => record.VendorId)
            .HasColumnName("vendor_id")
            .HasColumnType("uuid");
        builder.Property(record => record.VendorState)
            .HasColumnName("vendor_state")
            .HasColumnType("character varying(32)")
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(record => record.TradingPreference)
            .HasColumnName("trading_preference")
            .HasColumnType("character varying(16)")
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(record => record.RegisteredAtUtc)
            .HasColumnName("registered_at_utc")
            .HasColumnType("timestamp with time zone");
        builder.Property(record => record.LegalOperatorType)
            .HasColumnName("legal_operator_type")
            .HasColumnType("character varying(64)")
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(record => record.LegalOperatorName)
            .HasColumnName("legal_operator_name")
            .HasColumnType("character varying(160)")
            .HasMaxLength(160)
            .IsRequired();
        builder.Property(record => record.NormalizedLegalOperatorName)
            .HasColumnName("normalized_legal_operator_name")
            .HasColumnType("character varying(160)")
            .HasMaxLength(160)
            .IsRequired();
        builder.Property(record => record.TradingName)
            .HasColumnName("trading_name")
            .HasColumnType("character varying(160)")
            .HasMaxLength(160)
            .IsRequired();
        builder.Property(record => record.NormalizedTradingName)
            .HasColumnName("normalized_trading_name")
            .HasColumnType("character varying(160)")
            .HasMaxLength(160)
            .IsRequired();
        builder.Property(record => record.CompanyRegistrationNumber)
            .HasColumnName("company_registration_number")
            .HasColumnType("character varying(10)")
            .HasMaxLength(10);
        builder.Property(record => record.ContactName)
            .HasColumnName("contact_name")
            .HasColumnType("character varying(100)")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(record => record.ContactEmail)
            .HasColumnName("contact_email")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(record => record.ContactTelephone)
            .HasColumnName("contact_telephone")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(record => record.CanonicalAddressId)
            .HasColumnName("canonical_address_id")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(record => record.RecipientOrOrganisationName)
            .HasColumnName("recipient_or_organisation_name")
            .HasColumnType("text");
        builder.Property(record => record.AddressLine1)
            .HasColumnName("address_line_1")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(record => record.AddressLine2)
            .HasColumnName("address_line_2")
            .HasColumnType("text");
        builder.Property(record => record.AddressLine3)
            .HasColumnName("address_line_3")
            .HasColumnType("text");
        builder.Property(record => record.PostTown)
            .HasColumnName("post_town")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(record => record.Postcode)
            .HasColumnName("postcode")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(record => record.County)
            .HasColumnName("county")
            .HasColumnType("text");
        builder.Property(record => record.FoodRegistrationAuthority)
            .HasColumnName("food_registration_authority")
            .HasColumnType("text")
            .IsRequired();
        builder.Property(record => record.PrimaryTradingAuthority)
            .HasColumnName("primary_trading_authority")
            .HasColumnType("text");
        builder.Property(record => record.TradingLocation)
            .HasColumnName("trading_location")
            .HasColumnType("character varying(16)")
            .HasMaxLength(16)
            .IsRequired();
        builder.Property(record => record.OpeningHoursStart)
            .HasColumnName("opening_hours_start")
            .HasColumnType("time without time zone");
        builder.Property(record => record.OpeningHoursEnd)
            .HasColumnName("opening_hours_end")
            .HasColumnType("time without time zone");
        builder.Property(record => record.ServiceIncludesHotFood)
            .HasColumnName("service_includes_hot_food")
            .HasColumnType("boolean");
        builder.Property(record => record.AlcoholService)
            .HasColumnName("alcohol_service")
            .HasColumnType("boolean");
        builder.Property(record => record.Website)
            .HasColumnName("website")
            .HasColumnType("text");
        builder.Property(record => record.BusinessDescription)
            .HasColumnName("business_description")
            .HasColumnType("character varying(2000)")
            .HasMaxLength(2000);

        builder.HasIndex(
                record => new
                {
                    record.NormalizedTradingName,
                    record.NormalizedLegalOperatorName,
                    record.CanonicalAddressId
                })
            .IsUnique()
            .HasDatabaseName("uq_vendor_registrations_identity");
    }
}
