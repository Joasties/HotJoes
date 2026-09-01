using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class ComplianceReceiptDbContext : DbContext
{
    public ComplianceReceiptDbContext(
        DbContextOptions<ComplianceReceiptDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(
            new ComplianceReceiptRecordConfiguration());
    }
}
