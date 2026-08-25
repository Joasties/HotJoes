using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlVendorRepositoryTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlVendorRepositoryTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ThenCallerCommits_RoundTripsCompleteVendorAggregate()
    {
        VendorAggregate original = CreateVendor(
            Guid.Parse("29e3ae4c-d422-4b1a-81ed-39ac975bc3e2"),
            "repository-add");
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new PostgreSqlVendorRepository(context);

        await repository.AddAsync(original, CancellationToken.None);

        await using (VendorRegistrationDbContext uncommittedContext =
            CreateContext())
        {
            Assert.False(await uncommittedContext
                .Set<VendorRegistrationRecord>()
                .AsNoTracking()
                .AnyAsync(record => record.VendorId == original.Id.Value));
        }

        await context.SaveChangesAsync();

        await using VendorRegistrationDbContext retrievalContext =
            CreateContext();
        var retrievalRepository = new PostgreSqlVendorRepository(
            retrievalContext);
        VendorAggregate? rehydrated = await retrievalRepository.FindAsync(
            original.Id,
            CancellationToken.None);

        Assert.NotNull(rehydrated);
        AssertEquivalentAggregate(original, rehydrated);
        Assert.Empty(rehydrated.DomainEvents);
    }

    [Fact]
    public async Task FindAsync_KnownVendorId_ReturnsAggregateRootWithoutMutation()
    {
        VendorAggregate original = CreateVendor(
            Guid.Parse("af0f68c0-7458-4c3f-88a6-361524647162"),
            "repository-find");
        await SeedAsync(original);
        await using VendorRegistrationDbContext context = CreateContext();
        var repository = new PostgreSqlVendorRepository(context);
        int recordCountBefore = await CountVendorRecordsAsync(context);

        VendorAggregate? found = await repository.FindAsync(
            original.Id,
            CancellationToken.None);

        Assert.NotNull(found);
        Assert.IsType<VendorAggregate>(found);
        AssertEquivalentAggregate(original, found);
        Assert.Empty(found.DomainEvents);
        Assert.Equal(recordCountBefore, await CountVendorRecordsAsync(context));
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task FindAsync_UnknownVendorId_ReturnsNull()
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new PostgreSqlVendorRepository(context);

        VendorAggregate? result = await repository.FindAsync(
            new VendorId(
                Guid.Parse("30141e19-5045-46c1-951d-d41a6f746834")),
            CancellationToken.None);

        Assert.Null(result);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    private async Task SeedAsync(VendorAggregate vendor)
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        context.Set<VendorRegistrationRecord>().Add(
            VendorRegistrationRecordMapper.ToRecord(vendor));
        await context.SaveChangesAsync();
    }

    private static Task<int> CountVendorRecordsAsync(
        VendorRegistrationDbContext context)
    {
        return context.Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .CountAsync();
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static void AssertEquivalentAggregate(
        VendorAggregate expected,
        VendorAggregate actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.State, actual.State);
        Assert.Equal(expected.TradingPreference, actual.TradingPreference);
        Assert.Equal(expected.RegisteredAt, actual.RegisteredAt);
        Assert.Equal(
            expected.RegisteredInformation,
            actual.RegisteredInformation);
        Assert.Equal(expected.Website, actual.Website);
        Assert.Equal(
            expected.BusinessDescription,
            actual.BusinessDescription);
    }

    private static VendorAggregate CreateVendor(Guid vendorId, string suffix)
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.LimitedCompany,
            new VendorName($"Repository Operator {suffix} Ltd"),
            new VendorName($"Repository Kitchen {suffix}"),
            new CompanyRegistrationNumber("SC123456"),
            new PrimaryContact(
                "Jordan Smith",
                new EmailAddress("jordan@example.test"),
                new TelephoneNumber("+44 20 7946 0123")),
            new CanonicalAddressId($"canonical-address-{suffix}"),
            new BusinessAddressSnapshot(
                "24 Example Street",
                "Example Village",
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                "Greater London",
                "Repository Foods"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(vendorId),
            information,
            new Uri("https://repository.example.test"),
            "Repository persistence test Vendor.",
            new DateTimeOffset(2026, 8, 25, 19, 0, 0, TimeSpan.Zero));
    }
}
