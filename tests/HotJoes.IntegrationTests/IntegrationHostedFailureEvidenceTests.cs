using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

public sealed class IntegrationHostedFailureEvidenceTests
{
    [Fact]
    public void DeliberateHostedMigrationFailure_PreventsMerge()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=unused;Username=unused;" +
                    "Password=unused")
                .Options;
        using var context = new VendorRegistrationDbContext(options);

        Assert.Contains(
            "99999999999999_DeliberateMissingMigration",
            context.Database.GetMigrations());
    }
}
