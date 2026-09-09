using HotJoes.Infrastructure.ComplianceConsumer;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Database.Migrations;

internal sealed class DatabaseMigrationOperation
{
    private readonly string _vendorConnectionString;
    private readonly string _complianceConnectionString;

    public DatabaseMigrationOperation(
        string vendorConnectionString,
        string complianceConnectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vendorConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(
            complianceConnectionString);

        _vendorConnectionString = vendorConnectionString;
        _complianceConnectionString = complianceConnectionString;
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        DbContextOptions<VendorRegistrationDbContext> vendorOptions =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_vendorConnectionString)
                .Options;
        await using var vendorContext =
            new VendorRegistrationDbContext(vendorOptions);
        await vendorContext.Database.MigrateAsync(cancellationToken);

        DbContextOptions<ComplianceReceiptDbContext> complianceOptions =
            new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
                .UseNpgsql(_complianceConnectionString)
                .Options;
        await using var complianceContext =
            new ComplianceReceiptDbContext(complianceOptions);
        await complianceContext.Database.MigrateAsync(cancellationToken);
    }
}
