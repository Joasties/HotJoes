using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityPersistenceDbContext : DbContext
{
    public CommunityPersistenceDbContext(
        DbContextOptions<CommunityPersistenceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("community");
        modelBuilder.ApplyConfiguration(
            new CommunityParticipationRecordConfiguration());
        modelBuilder.ApplyConfiguration(
            new CommunityParticipationOutboxRecordConfiguration());
    }
}
