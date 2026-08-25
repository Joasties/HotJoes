using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.Persistence;

public sealed class VendorRegistrationDbContext : DbContext
{
    public VendorRegistrationDbContext(
        DbContextOptions<VendorRegistrationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(
            new VendorRegistrationRecordConfiguration());
        modelBuilder.ApplyConfiguration(
            new VendorRegistrationOutcomeRecordConfiguration());
        modelBuilder.ApplyConfiguration(
            new VendorRegistrationOutboxRecordConfiguration());
    }
}
