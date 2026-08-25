using System.Text;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class RetrieveRegisteredVendorPostgreSqlTests
{
    private readonly PostgreSqlFixture _fixture;

    public RetrieveRegisteredVendorPostgreSqlTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RetrieveAsync_RepeatedlyReturnsAuthoritativePersistedDetailsWithoutSideEffects()
    {
        VendorAggregate registeredVendor = CreateVendor();
        Guid eventId = Guid.Parse("66576f21-2271-4d96-8bf7-25b17c11ddb5");
        byte[] fingerprint = Enumerable.Range(1, 32)
            .Select(value => (byte)value)
            .ToArray();
        byte[] serializedEvent = Encoding.UTF8.GetBytes(
            "{\"eventId\":\"66576f21-2271-4d96-8bf7-25b17c11ddb5\"}");

        await SeedCurrentPersistedStateAsync(
            registeredVendor,
            eventId,
            fingerprint,
            serializedEvent);
        DurableSnapshot before = await LoadDurableSnapshotAsync(
            registeredVendor.Id.Value);

        await using VendorRegistrationDbContext retrievalContext =
            CreateContext();
        var service = new RetrieveRegisteredVendorService(
            new PostgreSqlVendorRepository(retrievalContext),
            new RegisteredVendorDetailsMapper());

        RetrieveRegisteredVendorResult first = await service.RetrieveAsync(
            registeredVendor.Id,
            CancellationToken.None);
        RetrieveRegisteredVendorResult second = await service.RetrieveAsync(
            registeredVendor.Id,
            CancellationToken.None);

        RegisteredVendorDetails firstDetails =
            Assert.IsType<RetrieveRegisteredVendorResult.Found>(first).Details;
        RegisteredVendorDetails secondDetails =
            Assert.IsType<RetrieveRegisteredVendorResult.Found>(second).Details;

        AssertAuthoritativeDetails(firstDetails, registeredVendor.Id);
        AssertAuthoritativeDetails(secondDetails, registeredVendor.Id);
        DurableSnapshot after = await LoadDurableSnapshotAsync(
            registeredVendor.Id.Value);
        Assert.Equal(before, after);

        await using VendorRegistrationDbContext verificationContext =
            CreateContext();
        var repository = new PostgreSqlVendorRepository(verificationContext);
        VendorAggregate? rehydrated = await repository.FindAsync(
            registeredVendor.Id,
            CancellationToken.None);
        Assert.NotNull(rehydrated);
        Assert.Empty(rehydrated.DomainEvents);
    }

    private async Task SeedCurrentPersistedStateAsync(
        VendorAggregate vendor,
        Guid eventId,
        byte[] fingerprint,
        byte[] serializedEvent)
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new PostgreSqlVendorRepository(context);
        await repository.AddAsync(vendor, CancellationToken.None);

        VendorRegistrationRecord vendorRecord = Assert.Single(
            context.ChangeTracker
                .Entries<VendorRegistrationRecord>()
                .Select(entry => entry.Entity));
        vendorRecord.VendorState = "activated";
        vendorRecord.TradingPreference = "online";

        context.Set<VendorRegistrationOutcomeRecord>().Add(
            new VendorRegistrationOutcomeRecord
            {
                VendorId = vendor.Id.Value,
                FingerprintVersion = 1,
                SemanticFingerprintSha256 = fingerprint.ToArray(),
                ResultVendorState = "pendingActivation"
            });
        context.Set<VendorRegistrationOutboxRecord>().Add(
            new VendorRegistrationOutboxRecord
            {
                EventId = eventId,
                VendorId = vendor.Id.Value,
                EventVersion = 1,
                SerializedEvent = serializedEvent.ToArray(),
                PublishedAtUtc = null
            });

        await context.SaveChangesAsync();
    }

    private async Task<DurableSnapshot> LoadDurableSnapshotAsync(Guid vendorId)
    {
        await using VendorRegistrationDbContext context = CreateContext();
        VendorRegistrationRecord vendor = await context
            .Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendorId);
        VendorRegistrationOutcomeRecord outcome = await context
            .Set<VendorRegistrationOutcomeRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendorId);
        VendorRegistrationOutboxRecord outbox = await context
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendorId);

        return new DurableSnapshot(
            VendorCount: await context.Set<VendorRegistrationRecord>()
                .AsNoTracking()
                .CountAsync(record => record.VendorId == vendorId),
            OutcomeCount: await context.Set<VendorRegistrationOutcomeRecord>()
                .AsNoTracking()
                .CountAsync(record => record.VendorId == vendorId),
            OutboxCount: await context.Set<VendorRegistrationOutboxRecord>()
                .AsNoTracking()
                .CountAsync(record => record.VendorId == vendorId),
            vendor.VendorState,
            vendor.TradingPreference,
            vendor.TradingName,
            vendor.BusinessDescription,
            outcome.FingerprintVersion,
            Convert.ToHexString(outcome.SemanticFingerprintSha256),
            outcome.ResultVendorState,
            outbox.EventId,
            outbox.EventVersion,
            Convert.ToHexString(outbox.SerializedEvent),
            outbox.PublishedAtUtc);
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static void AssertAuthoritativeDetails(
        RegisteredVendorDetails details,
        VendorId vendorId)
    {
        Assert.Equal(vendorId, details.VendorId);
        Assert.Equal(
            new DateTimeOffset(2026, 8, 25, 22, 0, 0, TimeSpan.Zero),
            details.RegisteredAt);
        Assert.Equal(VendorState.Activated, details.VendorState);
        Assert.Equal(TradingPreference.Online, details.TradingPreference);
        Assert.Equal(LegalOperatorType.LimitedCompany, details.LegalOperatorType);
        Assert.Equal("PostgreSQL Retrieval Operator Ltd", details.LegalOperatorName);
        Assert.Equal("SC654321", details.CompanyRegistrationNumber);
        Assert.Equal("PostgreSQL Retrieval Stall", details.TradingName);
        Assert.Equal(
            TradingLocation.Stall,
            details.TradingCharacteristics.TradingLocation);
        Assert.Equal(
            new TimeOnly(10, 0),
            details.TradingCharacteristics.OpeningHours.StartTime);
        Assert.Equal(
            new TimeOnly(20, 0),
            details.TradingCharacteristics.OpeningHours.EndTime);
        Assert.True(details.TradingCharacteristics.ServiceIncludesHotFood);
        Assert.False(details.TradingCharacteristics.AlcoholService);
        Assert.Equal("Morgan Lee", details.ContactName);
        Assert.Equal("morgan@example.test", details.ContactEmail);
        Assert.Equal("+44 20 7946 0789", details.ContactTelephone);
        Assert.Equal(
            "canonical-address-postgresql-retrieval",
            details.CanonicalAddressId);
        Assert.Equal(
            "PostgreSQL Retrieval Foods Ltd",
            details.BusinessAddress.RecipientOrOrganisationName);
        Assert.Equal("36 Example Street", details.BusinessAddress.AddressLine1);
        Assert.Equal("Unit 8", details.BusinessAddress.AddressLine2);
        Assert.Equal("Example Market", details.BusinessAddress.AddressLine3);
        Assert.Equal("LONDON", details.BusinessAddress.PostTown);
        Assert.Equal("AB1 2CD", details.BusinessAddress.Postcode);
        Assert.Equal("Greater London", details.BusinessAddress.County);
        Assert.Equal(
            "Greenwich Borough Council",
            details.FoodRegistrationAuthority);
        Assert.Equal(
            "Greenwich Borough Council",
            details.PrimaryTradingAuthority);
        Assert.Equal(
            "https://retrieval.example.test/vendor",
            details.Website);
        Assert.Equal(
            "Authoritative persisted retrieval state.",
            details.BusinessDescription);
    }

    private static VendorAggregate CreateVendor()
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.LimitedCompany,
            new VendorName("PostgreSQL Retrieval Operator Ltd"),
            new VendorName("PostgreSQL Retrieval Stall"),
            new CompanyRegistrationNumber("SC654321"),
            new PrimaryContact(
                "Morgan Lee",
                new EmailAddress("morgan@example.test"),
                new TelephoneNumber("+44 20 7946 0789")),
            new CanonicalAddressId(
                "canonical-address-postgresql-retrieval"),
            new BusinessAddressSnapshot(
                "36 Example Street",
                "Unit 8",
                "Example Market",
                "LONDON",
                "AB1 2CD",
                "Greater London",
                "PostgreSQL Retrieval Foods Ltd"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"),
            new TradingCharacteristics(
                TradingLocation.Stall,
                new OpeningHours(
                    new TimeOnly(10, 0),
                    new TimeOnly(20, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(
                Guid.Parse("be61c317-d112-484e-b229-014fd73d962d")),
            information,
            new Uri("https://retrieval.example.test/vendor"),
            "Authoritative persisted retrieval state.",
            new DateTimeOffset(2026, 8, 25, 22, 0, 0, TimeSpan.Zero));
    }

    private sealed record DurableSnapshot(
        int VendorCount,
        int OutcomeCount,
        int OutboxCount,
        string VendorState,
        string TradingPreference,
        string TradingName,
        string? BusinessDescription,
        short FingerprintVersion,
        string FingerprintHex,
        string ResultVendorState,
        Guid EventId,
        int EventVersion,
        string SerializedEventHex,
        DateTimeOffset? PublishedAtUtc);
}
