using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityPersistenceDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<CommunityPersistenceDbContext>
{
    public CommunityPersistenceDbContext CreateDbContext(string[] args)
    {
        string? connectionString = Environment.GetEnvironmentVariable(
            "HOTJOES_COMMUNITY_MIGRATION_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "HOTJOES_COMMUNITY_MIGRATION_CONNECTION_STRING is required.");
        }

        DbContextOptions<CommunityPersistenceDbContext> options =
            new DbContextOptionsBuilder<CommunityPersistenceDbContext>()
                .UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "community"))
                .Options;

        return new CommunityPersistenceDbContext(options);
    }
}
