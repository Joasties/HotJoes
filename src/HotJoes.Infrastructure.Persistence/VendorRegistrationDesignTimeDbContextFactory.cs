using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotJoes.Infrastructure.Persistence;

public sealed class VendorRegistrationDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<VendorRegistrationDbContext>
{
    public VendorRegistrationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<VendorRegistrationDbContext>()
            .UseNpgsql()
            .Options;
        return new VendorRegistrationDbContext(options);
    }
}
