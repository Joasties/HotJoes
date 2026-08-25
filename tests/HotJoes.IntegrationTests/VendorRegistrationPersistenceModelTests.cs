using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HotJoes.IntegrationTests;

public sealed class VendorRegistrationPersistenceModelTests
{
    [Fact]
    public void Model_VendorRegistration_MapsApprovedColumnsAndCompositeIdentity()
    {
        using VendorRegistrationDbContext context = CreateContext();
        IEntityType entity = FindEntity(GetModel(context), "vendor_registrations");
        StoreObjectIdentifier table = StoreObjectIdentifier.Table(
            "vendor_registrations",
            null);

        AssertPrimaryKey(entity, table, "vendor_id");
        AssertColumns(
            entity,
            table,
            "address_line_1",
            "address_line_2",
            "address_line_3",
            "alcohol_service",
            "business_description",
            "canonical_address_id",
            "company_registration_number",
            "contact_email",
            "contact_name",
            "contact_telephone",
            "county",
            "food_registration_authority",
            "legal_operator_name",
            "legal_operator_type",
            "normalized_legal_operator_name",
            "normalized_trading_name",
            "opening_hours_end",
            "opening_hours_start",
            "post_town",
            "postcode",
            "primary_trading_authority",
            "recipient_or_organisation_name",
            "registered_at_utc",
            "service_includes_hot_food",
            "trading_location",
            "trading_name",
            "trading_preference",
            "vendor_id",
            "vendor_state",
            "website");

        AssertColumn(entity, table, "vendor_id", "uuid", nullable: false);
        AssertColumn(
            entity,
            table,
            "legal_operator_name",
            "character varying(160)",
            nullable: false,
            maximumLength: 160);
        AssertColumn(
            entity,
            table,
            "normalized_legal_operator_name",
            "character varying(160)",
            nullable: false,
            maximumLength: 160);
        AssertColumn(
            entity,
            table,
            "trading_name",
            "character varying(160)",
            nullable: false,
            maximumLength: 160);
        AssertColumn(
            entity,
            table,
            "normalized_trading_name",
            "character varying(160)",
            nullable: false,
            maximumLength: 160);
        AssertColumn(
            entity,
            table,
            "company_registration_number",
            "character varying(10)",
            nullable: true,
            maximumLength: 10);
        AssertColumn(
            entity,
            table,
            "contact_name",
            "character varying(100)",
            nullable: false,
            maximumLength: 100);
        AssertColumn(entity, table, "contact_email", "text", nullable: false);
        AssertColumn(entity, table, "contact_telephone", "text", nullable: false);
        AssertColumn(entity, table, "canonical_address_id", "text", nullable: false);
        AssertColumn(entity, table, "address_line_1", "text", nullable: false);
        AssertColumn(entity, table, "address_line_2", "text", nullable: true);
        AssertColumn(entity, table, "address_line_3", "text", nullable: true);
        AssertColumn(entity, table, "post_town", "text", nullable: false);
        AssertColumn(entity, table, "postcode", "text", nullable: false);
        AssertColumn(entity, table, "county", "text", nullable: true);
        AssertColumn(
            entity,
            table,
            "recipient_or_organisation_name",
            "text",
            nullable: true);
        AssertColumn(
            entity,
            table,
            "food_registration_authority",
            "text",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "primary_trading_authority",
            "text",
            nullable: true);
        AssertColumn(
            entity,
            table,
            "opening_hours_start",
            "time without time zone",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "opening_hours_end",
            "time without time zone",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "registered_at_utc",
            "timestamp with time zone",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "business_description",
            "character varying(2000)",
            nullable: true,
            maximumLength: 2000);
        AssertColumn(entity, table, "website", "text", nullable: true);

        IIndex identityIndex = Assert.Single(
            entity.GetIndexes(),
            index => index.GetDatabaseName() ==
                "uq_vendor_registrations_identity");
        Assert.True(identityIndex.IsUnique);
        Assert.Equal(
            new[]
            {
                "normalized_trading_name",
                "normalized_legal_operator_name",
                "canonical_address_id"
            },
            identityIndex.Properties.Select(
                property => property.GetColumnName(table)));

        AssertCheckConstraints(
            entity,
            "ck_vendor_registrations_company_registration_number",
            "ck_vendor_registrations_legal_operator_type",
            "ck_vendor_registrations_normalized_names",
            "ck_vendor_registrations_primary_trading_authority",
            "ck_vendor_registrations_trading_location",
            "ck_vendor_registrations_trading_preference",
            "ck_vendor_registrations_vendor_state");
    }

    [Fact]
    public void Model_RegistrationOutcome_MapsPermanentOneToOneResultAndDigest()
    {
        using VendorRegistrationDbContext context = CreateContext();
        IEntityType entity = FindEntity(
            GetModel(context),
            "vendor_registration_outcomes");
        StoreObjectIdentifier table = StoreObjectIdentifier.Table(
            "vendor_registration_outcomes",
            null);

        AssertPrimaryKey(entity, table, "vendor_id");
        AssertColumns(
            entity,
            table,
            "fingerprint_version",
            "result_vendor_state",
            "semantic_fingerprint_sha256",
            "vendor_id");
        AssertColumn(entity, table, "vendor_id", "uuid", nullable: false);
        AssertColumn(
            entity,
            table,
            "fingerprint_version",
            "smallint",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "semantic_fingerprint_sha256",
            "bytea",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "result_vendor_state",
            "character varying(32)",
            nullable: false,
            maximumLength: 32);
        AssertRestrictiveVendorRelationship(entity);
        AssertCheckConstraints(
            entity,
            "ck_vendor_registration_outcomes_fingerprint_sha256",
            "ck_vendor_registration_outcomes_fingerprint_version",
            "ck_vendor_registration_outcomes_result_vendor_state");
    }

    [Fact]
    public void Model_RegistrationOutbox_MapsImmutableBytesAndUnpublishedLookup()
    {
        using VendorRegistrationDbContext context = CreateContext();
        IEntityType entity = FindEntity(
            GetModel(context),
            "vendor_registration_outbox");
        StoreObjectIdentifier table = StoreObjectIdentifier.Table(
            "vendor_registration_outbox",
            null);

        AssertPrimaryKey(entity, table, "event_id");
        AssertColumns(
            entity,
            table,
            "event_id",
            "event_version",
            "published_at_utc",
            "serialized_event",
            "vendor_id");
        AssertColumn(entity, table, "event_id", "uuid", nullable: false);
        AssertColumn(entity, table, "vendor_id", "uuid", nullable: false);
        AssertColumn(entity, table, "event_version", "integer", nullable: false);
        AssertColumn(entity, table, "serialized_event", "bytea", nullable: false);
        AssertColumn(
            entity,
            table,
            "published_at_utc",
            "timestamp with time zone",
            nullable: true);
        AssertRestrictiveVendorRelationship(entity);

        IIndex vendorIndex = Assert.Single(
            entity.GetIndexes(),
            index => index.GetDatabaseName() ==
                "uq_vendor_registration_outbox_vendor_id");
        Assert.True(vendorIndex.IsUnique);
        Assert.Equal(
            new[] { "vendor_id" },
            vendorIndex.Properties.Select(
                property => property.GetColumnName(table)));

        IIndex unpublishedIndex = Assert.Single(
            entity.GetIndexes(),
            index => index.GetDatabaseName() ==
                "ix_vendor_registration_outbox_unpublished");
        Assert.False(unpublishedIndex.IsUnique);
        Assert.Equal("published_at_utc IS NULL", unpublishedIndex.GetFilter());
        Assert.Equal(
            new[] { "event_id" },
            unpublishedIndex.Properties.Select(
                property => property.GetColumnName(table)));

        AssertCheckConstraints(
            entity,
            "ck_vendor_registration_outbox_event_version",
            "ck_vendor_registration_outbox_serialized_event");
    }

    [Fact]
    public void Model_ContainsNoTransientOrReconstructionPersistenceColumns()
    {
        using VendorRegistrationDbContext context = CreateContext();
        string[] columns = context.Model
            .GetEntityTypes()
            .SelectMany(entity => entity.GetProperties())
            .Select(property => property.GetColumnName())
            .Where(column => column is not null)
            .Cast<string>()
            .ToArray();

        Assert.DoesNotContain("address_resolution_reference", columns);
        Assert.DoesNotContain("authorised_to_register_business", columns);
        Assert.DoesNotContain("information_accurate", columns);
        Assert.DoesNotContain("accept_hot_joes_platform_terms", columns);
        Assert.DoesNotContain("domain_event", columns);
        Assert.DoesNotContain("integration_event", columns);
        Assert.DoesNotContain("fingerprint_expires_at", columns);
    }

    private static VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=hotjoes_model_test;Username=test;Password=test")
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static IModel GetModel(VendorRegistrationDbContext context)
    {
        return context.GetService<IDesignTimeModel>().Model;
    }

    private static IEntityType FindEntity(IModel model, string tableName)
    {
        return Assert.Single(
            model.GetEntityTypes(),
            entity => entity.GetTableName() == tableName);
    }

    private static void AssertPrimaryKey(
        IEntityType entity,
        StoreObjectIdentifier table,
        params string[] expectedColumns)
    {
        IKey key = Assert.IsAssignableFrom<IKey>(entity.FindPrimaryKey());

        Assert.Equal(
            expectedColumns,
            key.Properties.Select(property => property.GetColumnName(table)));
    }

    private static void AssertColumns(
        IEntityType entity,
        StoreObjectIdentifier table,
        params string[] expectedColumns)
    {
        Assert.Equal(
            expectedColumns.Order(),
            entity.GetProperties()
                .Select(property => property.GetColumnName(table))
                .Where(column => column is not null)
                .Cast<string>()
                .Order());
    }

    private static void AssertColumn(
        IEntityType entity,
        StoreObjectIdentifier table,
        string columnName,
        string storeType,
        bool nullable,
        int? maximumLength = null)
    {
        IProperty property = Assert.Single(
            entity.GetProperties(),
            candidate => candidate.GetColumnName(table) == columnName);

        Assert.Equal(storeType, property.GetColumnType());
        Assert.Equal(nullable, property.IsNullable);
        Assert.Equal(maximumLength, property.GetMaxLength());
    }

    private static void AssertRestrictiveVendorRelationship(IEntityType entity)
    {
        IForeignKey foreignKey = Assert.Single(entity.GetForeignKeys());

        Assert.Equal(
            "vendor_registrations",
            foreignKey.PrincipalEntityType.GetTableName());
        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
        Assert.True(foreignKey.IsUnique);
        Assert.True(foreignKey.IsRequired);
    }

    private static void AssertCheckConstraints(
        IEntityType entity,
        params string[] expectedNames)
    {
        Assert.Equal(
            expectedNames.Order(),
            entity.GetCheckConstraints().Select(constraint => constraint.Name).Order());
    }
}
