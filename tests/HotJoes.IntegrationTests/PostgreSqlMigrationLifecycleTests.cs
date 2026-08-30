using HotJoes.Infrastructure.ComplianceConsumer;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace HotJoes.IntegrationTests;

[Collection(MigrationPostgreSqlCollection.Name)]
public sealed class PostgreSqlMigrationLifecycleTests
{
    private const string VendorBaselineMigration =
        "20260817000100_InitialVendorRegistrationSchema";
    private const string VendorPrecedingMigration =
        "20260828000100_AddReliableOutboxRelayState";
    private const string VendorCurrentMigration =
        "20260829000100_AddOutboxTraceContext";
    private const string ComplianceCurrentMigration =
        "20260828000200_AddComplianceVendorRegisteredReceipts";

    private readonly MigrationPostgreSqlFixture _fixture;

    public PostgreSqlMigrationLifecycleTests(
        MigrationPostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MigrationChains_HaveStableReviewedOrder()
    {
        using VendorRegistrationDbContext vendorContext =
            CreateVendorContext();
        using ComplianceReceiptDbContext complianceContext =
            CreateComplianceContext();

        Assert.Equal(
            [
                VendorBaselineMigration,
                VendorPrecedingMigration,
                VendorCurrentMigration
            ],
            vendorContext.Database.GetMigrations().ToArray());
        Assert.Equal(
            [ComplianceCurrentMigration],
            complianceContext.Database.GetMigrations().ToArray());
    }

    [Fact]
    public async Task EmptyDatabase_AllMigrationsCreateCurrentSchema()
    {
        await ResetPublicSchemaAsync();

        await using (VendorRegistrationDbContext vendorContext =
            CreateVendorContext())
        {
            await vendorContext.Database.MigrateAsync();
            Assert.Empty(
                await vendorContext.Database.GetPendingMigrationsAsync());
            Assert.False(vendorContext.Database.HasPendingModelChanges());
        }

        await using (ComplianceReceiptDbContext complianceContext =
            CreateComplianceContext())
        {
            await complianceContext.Database.MigrateAsync();
            Assert.Empty(
                await complianceContext.Database.GetPendingMigrationsAsync());
            Assert.False(complianceContext.Database.HasPendingModelChanges());
        }

        Assert.True(await TableExistsAsync("vendor_registrations"));
        Assert.True(await TableExistsAsync("vendor_registration_outcomes"));
        Assert.True(await TableExistsAsync("vendor_registration_outbox"));
        Assert.True(await TableExistsAsync(
            "compliance_vendor_registered_receipts"));
        Assert.True(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "claim_expires_at_utc"));
        Assert.True(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "trace_parent"));
        Assert.True(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "trace_state"));
        Assert.True(await IndexExistsAsync(
            "ix_vendor_registration_outbox_eligible"));
    }

    [Fact]
    public async Task ImmediatelyPrecedingBaseline_UpgradesToCurrentSchema()
    {
        await ResetPublicSchemaAsync();

        await using (VendorRegistrationDbContext vendorContext =
            CreateVendorContext())
        {
            IMigrator migrator = vendorContext.GetService<IMigrator>();
            await migrator.MigrateAsync(VendorPrecedingMigration);
        }

        Assert.True(await TableExistsAsync("vendor_registrations"));
        Assert.True(await TableExistsAsync("vendor_registration_outcomes"));
        Assert.True(await TableExistsAsync("vendor_registration_outbox"));
        Assert.True(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "claim_expires_at_utc"));
        Assert.False(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "trace_parent"));
        Assert.False(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "trace_state"));
        Assert.False(await TableExistsAsync(
            "compliance_vendor_registered_receipts"));

        await using (VendorRegistrationDbContext vendorContext =
            CreateVendorContext())
        {
            await vendorContext.Database.MigrateAsync();
            Assert.Empty(
                await vendorContext.Database.GetPendingMigrationsAsync());
            Assert.False(vendorContext.Database.HasPendingModelChanges());
        }

        await using (ComplianceReceiptDbContext complianceContext =
            CreateComplianceContext())
        {
            await complianceContext.Database.MigrateAsync();
            Assert.Empty(
                await complianceContext.Database.GetPendingMigrationsAsync());
            Assert.False(complianceContext.Database.HasPendingModelChanges());
        }

        Assert.True(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "trace_parent"));
        Assert.True(await ColumnExistsAsync(
            "vendor_registration_outbox",
            "trace_state"));
        Assert.True(await IndexExistsAsync(
            "ix_vendor_registration_outbox_eligible"));
        Assert.True(await TableExistsAsync(
            "compliance_vendor_registered_receipts"));
    }

    private VendorRegistrationDbContext CreateVendorContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private ComplianceReceiptDbContext CreateComplianceContext()
    {
        DbContextOptions<ComplianceReceiptDbContext> options =
            new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new ComplianceReceiptDbContext(options);
    }

    private async Task ResetPublicSchemaAsync()
    {
        await using var connection = new NpgsqlConnection(
            _fixture.ConnectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText =
            "DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public;";
        await command.ExecuteNonQueryAsync();
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_name = @table_name);
            """;

        return await ExecuteExistsAsync(sql, ("table_name", tableName));
    }

    private async Task<bool> ColumnExistsAsync(
        string tableName,
        string columnName)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.columns
                WHERE table_schema = 'public'
                  AND table_name = @table_name
                  AND column_name = @column_name);
            """;

        return await ExecuteExistsAsync(
            sql,
            ("table_name", tableName),
            ("column_name", columnName));
    }

    private async Task<bool> IndexExistsAsync(string indexName)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM pg_indexes
                WHERE schemaname = 'public'
                  AND indexname = @index_name);
            """;

        return await ExecuteExistsAsync(sql, ("index_name", indexName));
    }

    private async Task<bool> ExecuteExistsAsync(
        string commandText,
        params (string Name, string Value)[] parameters)
    {
        await using var connection = new NpgsqlConnection(
            _fixture.ConnectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = commandText;

        foreach ((string name, string value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        object? result = await command.ExecuteScalarAsync();
        return Assert.IsType<bool>(result);
    }
}
