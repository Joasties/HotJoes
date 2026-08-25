using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class VendorRegistrationPostgreSqlRoundTripTests
{
    private readonly PostgreSqlFixture _fixture;

    public VendorRegistrationPostgreSqlRoundTripTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CompleteVendor_RoundTripsThroughRealPostgreSql()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        await using var context = new VendorRegistrationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        VendorAggregate original = CreateVendor();
        VendorRegistrationRecord record =
            VendorRegistrationRecordMapper.ToRecord(original);

        context.Set<VendorRegistrationRecord>().Add(record);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        VendorRegistrationRecord persisted = await context
            .Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .SingleAsync(candidate => candidate.VendorId == original.Id.Value);
        VendorAggregate rehydrated =
            VendorRegistrationRecordMapper.ToDomain(persisted);

        Assert.Equal(original.Id, rehydrated.Id);
        Assert.Equal(original.State, rehydrated.State);
        Assert.Equal(original.TradingPreference, rehydrated.TradingPreference);
        Assert.Equal(original.RegisteredAt, rehydrated.RegisteredAt);
        Assert.Equal(
            original.RegisteredInformation,
            rehydrated.RegisteredInformation);
        Assert.Equal(original.Website, rehydrated.Website);
        Assert.Equal(
            original.BusinessDescription,
            rehydrated.BusinessDescription);
        Assert.Empty(rehydrated.DomainEvents);
    }

    private static VendorAggregate CreateVendor()
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.LimitedCompany,
            new VendorName("Example Operator Ltd"),
            new VendorName("Example Kitchen"),
            new CompanyRegistrationNumber("SC123456"),
            new PrimaryContact(
                "Jordan Smith",
                new EmailAddress("jordan@example.test"),
                new TelephoneNumber("+44 20 7946 0123")),
            new CanonicalAddressId("address-postgresql-round-trip"),
            new BusinessAddressSnapshot(
                "10 Example Street",
                "Example Village",
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                "Greater London",
                "Example Foods"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(Guid.Parse("6e81f63c-9f92-4694-8461-1a760a50528c")),
            information,
            new Uri("https://example.test"),
            "Evening food delivery kitchen.",
            new DateTimeOffset(2026, 8, 17, 12, 0, 0, TimeSpan.Zero));
    }
}
