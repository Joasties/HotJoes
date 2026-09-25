using Microsoft.EntityFrameworkCore;
namespace HotJoes.Infrastructure.CommunityConsumer;
public sealed class CommunityReceiptDbContext : DbContext
{
    public CommunityReceiptDbContext(DbContextOptions<CommunityReceiptDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("community");
        builder.ApplyConfiguration(new CommunityReceiptRecordConfiguration());
    }
}
