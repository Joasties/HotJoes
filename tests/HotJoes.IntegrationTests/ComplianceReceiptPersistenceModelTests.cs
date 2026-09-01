using HotJoes.Infrastructure.ComplianceConsumer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HotJoes.IntegrationTests;

public sealed class ComplianceReceiptPersistenceModelTests
{
    [Fact]
    public void Model_MapsExactDurableReceiptWithoutPayloadOrBusinessState()
    {
        using ComplianceReceiptDbContext context = CreateContext();
        IModel model = context.GetService<IDesignTimeModel>().Model;
        IEntityType entity = Assert.Single(
            model.GetEntityTypes(),
            candidate => candidate.GetTableName() ==
                "compliance_vendor_registered_receipts");
        StoreObjectIdentifier table = StoreObjectIdentifier.Table(
            "compliance_vendor_registered_receipts",
            null);

        IKey primaryKey = Assert.IsAssignableFrom<IKey>(
            entity.FindPrimaryKey());
        Assert.Equal(
            new[] { "event_id" },
            primaryKey.Properties.Select(
                property => property.GetColumnName(table)));

        Assert.Equal(
            new[]
            {
                "event_id",
                "event_type",
                "event_version",
                "received_at_utc",
                "serialized_event_sha256"
            },
            entity.GetProperties()
                .Select(property => property.GetColumnName(table))
                .Order(StringComparer.Ordinal));

        AssertColumn(entity, table, "event_id", "uuid", nullable: false);
        AssertColumn(
            entity,
            table,
            "event_type",
            "character varying(64)",
            nullable: false,
            maximumLength: 64);
        AssertColumn(
            entity,
            table,
            "event_version",
            "integer",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "received_at_utc",
            "timestamp with time zone",
            nullable: false);
        AssertColumn(
            entity,
            table,
            "serialized_event_sha256",
            "bytea",
            nullable: false);

        Assert.Equal(
            new[]
            {
                "ck_compliance_receipts_event_type",
                "ck_compliance_receipts_event_version",
                "ck_compliance_receipts_sha256"
            },
            entity.GetCheckConstraints()
                .Select(constraint => constraint.Name)
                .Order(StringComparer.Ordinal));

        string[] prohibitedColumns =
        [
            "serialized_event",
            "vendor_id",
            "vendor_state",
            "pending_activation",
            "contact_email",
            "contact_telephone",
            "address_line_1",
            "postcode"
        ];

        Assert.All(
            prohibitedColumns,
            column => Assert.DoesNotContain(
                column,
                entity.GetProperties().Select(
                    property => property.GetColumnName(table))));
    }

    private static ComplianceReceiptDbContext CreateContext()
    {
        DbContextOptions<ComplianceReceiptDbContext> options =
            new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=hotjoes_compliance_model_test;Username=test;Password=test")
                .Options;

        return new ComplianceReceiptDbContext(options);
    }

    private static void AssertColumn(
        IEntityType entity,
        StoreObjectIdentifier table,
        string columnName,
        string columnType,
        bool nullable,
        int? maximumLength = null)
    {
        IProperty property = Assert.Single(
            entity.GetProperties(),
            candidate => candidate.GetColumnName(table) == columnName);

        Assert.Equal(columnType, property.GetColumnType());
        Assert.Equal(nullable, property.IsNullable);
        Assert.Equal(maximumLength, property.GetMaxLength());
    }
}
