using HotJoes.Infrastructure.ComplianceConsumer;
using HotJoes.Infrastructure.Community.Persistence;
using HotJoes.Infrastructure.CommunityConsumer;
using HotJoes.Infrastructure.Vendor.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Database.Migrations;

internal sealed class DatabaseMigrationOperation
{
    private const string CommunitySchema = "community";
    private const string MigrationsHistoryTable = "__EFMigrationsHistory";

    private readonly string _vendorConnectionString;
    private readonly string _complianceConnectionString;
    private readonly string _communityConnectionString;

    public DatabaseMigrationOperation(
        string vendorConnectionString,
        string complianceConnectionString,
        string communityConnectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(vendorConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(
            complianceConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(communityConnectionString);

        _vendorConnectionString = vendorConnectionString;
        _complianceConnectionString = complianceConnectionString;
        _communityConnectionString = communityConnectionString;
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

        DbContextOptions<CommunityPersistenceDbContext> communityOptions =
            new DbContextOptionsBuilder<CommunityPersistenceDbContext>()
                .UseNpgsql(
                    _communityConnectionString,
                    npgsql => npgsql.MigrationsHistoryTable(
                        MigrationsHistoryTable,
                        CommunitySchema))
                .Options;
        await using var communityContext =
            new CommunityPersistenceDbContext(communityOptions);
        await communityContext.Database.MigrateAsync(cancellationToken);

        DbContextOptions<CommunityReceiptDbContext> receiptOptions =
            new DbContextOptionsBuilder<CommunityReceiptDbContext>()
                .UseNpgsql(
                    _communityConnectionString,
                    npgsql => npgsql.MigrationsHistoryTable(
                        MigrationsHistoryTable,
                        CommunitySchema))
                .Options;
        await using var receiptContext =
            new CommunityReceiptDbContext(receiptOptions);
        await receiptContext.Database.MigrateAsync(cancellationToken);
    }
}
