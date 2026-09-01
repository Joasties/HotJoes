using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class ComplianceReceiptDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<ComplianceReceiptDbContext>
{
    public ComplianceReceiptDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
            .UseNpgsql()
            .Options;
        return new ComplianceReceiptDbContext(options);
    }
}
