using HotJoes.Infrastructure.Community.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HotJoes.IntegrationTests;

[Collection(MigrationPostgreSqlCollection.Name)]
public sealed class PostgreSqlCommunityMigrationLifecycleTests
{
    private const string InitialCommunityMigration =
        "20260922121609_InitialCommunityParticipationSchema";

    private readonly MigrationPostgreSqlFixture _fixture;

    public PostgreSqlCommunityMigrationLifecycleTests(
        MigrationPostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MigrationChain_HasStableReviewedOrder()
    {
        using CommunityPersistenceDbContext context = CreateContext();

        Assert.Equal(
            [InitialCommunityMigration],
            context.Database.GetMigrations().ToArray());
    }

    [Fact]
    public async Task EmptyDatabase_MigrationCreatesCommunityOwnedSchemaAndConstraints()
    {
        await ResetCommunitySchemaAsync();

        await using CommunityPersistenceDbContext context = CreateContext();
        await context.Database.MigrateAsync();

        Assert.Empty(await context.Database.GetPendingMigrationsAsync());
        Assert.False(context.Database.HasPendingModelChanges());
        Assert.True(await SchemaExistsAsync("community"));
        Assert.True(await TableExistsAsync(
            "community",
            "community_participations"));
        Assert.True(await TableExistsAsync(
            "community",
            "community_participation_outbox"));
        Assert.True(await IndexExistsAsync(
            "community",
            "uq_community_participations_vendor_id"));
        Assert.True(await IndexExistsAsync(
            "community",
            "uq_community_participation_outbox_participation_id"));
    }

    [Fact]
    public async Task CurrentMigration_ReexecutionIsSafeAndLeavesNoPendingWork()
    {
        await ResetCommunitySchemaAsync();

        await using CommunityPersistenceDbContext first = CreateContext();
        await first.Database.MigrateAsync();
        await using CommunityPersistenceDbContext second = CreateContext();
        await second.Database.MigrateAsync();

        Assert.Empty(await second.Database.GetPendingMigrationsAsync());
        Assert.False(second.Database.HasPendingModelChanges());
        Assert.True(await TableExistsAsync(
            "community",
            "community_participations"));
        Assert.True(await TableExistsAsync(
            "community",
            "community_participation_outbox"));
    }

    private CommunityPersistenceDbContext CreateContext()
    {
        DbContextOptions<CommunityPersistenceDbContext> options =
            new DbContextOptionsBuilder<CommunityPersistenceDbContext>()
                .UseNpgsql(
                    _fixture.ConnectionString,
                    npgsql => npgsql.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "community"))
                .Options;
        return new CommunityPersistenceDbContext(options);
    }

    private async Task ResetCommunitySchemaAsync()
    {
        await using var connection = new NpgsqlConnection(
            _fixture.ConnectionString);
        await connection.OpenAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText =
            "DROP SCHEMA IF EXISTS community CASCADE;";
        await command.ExecuteNonQueryAsync();
    }

    private Task<bool> SchemaExistsAsync(string schemaName) =>
        ExecuteExistsAsync(
            "SELECT EXISTS (SELECT 1 FROM information_schema.schemata " +
            "WHERE schema_name = @schema_name);",
            ("schema_name", schemaName));

    private Task<bool> TableExistsAsync(string schemaName, string tableName) =>
        ExecuteExistsAsync(
            "SELECT EXISTS (SELECT 1 FROM information_schema.tables " +
            "WHERE table_schema = @schema_name AND table_name = @object_name);",
            ("schema_name", schemaName),
            ("object_name", tableName));

    private Task<bool> IndexExistsAsync(string schemaName, string indexName) =>
        ExecuteExistsAsync(
            "SELECT EXISTS (SELECT 1 FROM pg_indexes " +
            "WHERE schemaname = @schema_name AND indexname = @object_name);",
            ("schema_name", schemaName),
            ("object_name", indexName));

    private async Task<bool> ExecuteExistsAsync(
        string sql,
        params (string Name, string Value)[] parameters)
    {
        await using var connection = new NpgsqlConnection(
            _fixture.ConnectionString);
        await connection.OpenAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;

        foreach ((string name, string value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        return (bool)(await command.ExecuteScalarAsync())!;
    }
}
