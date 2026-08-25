using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class VendorRegistrationPostgreSqlOptionalValueTests
{
    private readonly PostgreSqlFixture _fixture;

    public VendorRegistrationPostgreSqlOptionalValueTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task VendorWithNoOptionalValues_RoundTripsNullsThroughPostgreSql()
    {
        VendorAggregate original = CreateVendor(
            vendorId: "a345c2a8-21bd-461c-b04d-ad72ba848e82",
            legalOperatorName: "Jordan Smith",
            tradingName: "Jordan's Kitchen",
            canonicalAddressId: "address-postgresql-null-optionals",
            tradingLocation: TradingLocation.Kitchen,
            primaryTradingAuthority: null,
            addressLine2: null,
            addressLine3: null,
            county: null,
            recipientOrOrganisationName: null,
            website: null,
            businessDescription: null);

        VendorRegistrationRecord persisted = await SaveAndReloadAsync(original);
        VendorAggregate rehydrated =
            VendorRegistrationRecordMapper.ToDomain(persisted);

        Assert.Null(persisted.CompanyRegistrationNumber);
        Assert.Null(persisted.RecipientOrOrganisationName);
        Assert.Null(persisted.AddressLine2);
        Assert.Null(persisted.AddressLine3);
        Assert.Null(persisted.County);
        Assert.Null(persisted.PrimaryTradingAuthority);
        Assert.Null(persisted.Website);
        Assert.Null(persisted.BusinessDescription);
        Assert.Equal(original.RegisteredInformation, rehydrated.RegisteredInformation);
        Assert.Null(rehydrated.Website);
        Assert.Null(rehydrated.BusinessDescription);
        Assert.Empty(rehydrated.DomainEvents);
    }

    [Fact]
    public async Task StallVendor_RoundTripsRequiredPrimaryTradingAuthorityThroughPostgreSql()
    {
        VendorAggregate original = CreateVendor(
            vendorId: "89c013ae-f0c2-4b41-8b0a-25b5df6bf595",
            legalOperatorName: "Taylor Green",
            tradingName: "Green Market Foods",
            canonicalAddressId: "address-postgresql-stall-authority",
            tradingLocation: TradingLocation.Stall,
            primaryTradingAuthority:
                new PrimaryTradingAuthority("Greenwich Borough Council"),
            addressLine2: "Market Square",
            addressLine3: "Stall 14",
            county: "Greater London",
            recipientOrOrganisationName: "Green Market Foods",
            website: new Uri("https://green-market.example.test"),
            businessDescription: "Hot food market stall.");

        VendorRegistrationRecord persisted = await SaveAndReloadAsync(original);
        VendorAggregate rehydrated =
            VendorRegistrationRecordMapper.ToDomain(persisted);

        Assert.Equal("stall", persisted.TradingLocation);
        Assert.Equal(
            "Greenwich Borough Council",
            persisted.PrimaryTradingAuthority);
        Assert.Equal(
            TradingLocation.Stall,
            rehydrated.RegisteredInformation.TradingCharacteristics.TradingLocation);
        Assert.Equal(
            new PrimaryTradingAuthority("Greenwich Borough Council"),
            rehydrated.RegisteredInformation.PrimaryTradingAuthority);
        Assert.Equal(original.RegisteredInformation, rehydrated.RegisteredInformation);
        Assert.Equal(original.Website, rehydrated.Website);
        Assert.Equal(original.BusinessDescription, rehydrated.BusinessDescription);
        Assert.Empty(rehydrated.DomainEvents);
    }

    private async Task<VendorRegistrationRecord> SaveAndReloadAsync(
        VendorAggregate vendor)
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        await using var context = new VendorRegistrationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.Set<VendorRegistrationRecord>().Add(
            VendorRegistrationRecordMapper.ToRecord(vendor));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return await context
            .Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendor.Id.Value);
    }

    private static VendorAggregate CreateVendor(
        string vendorId,
        string legalOperatorName,
        string tradingName,
        string canonicalAddressId,
        TradingLocation tradingLocation,
        PrimaryTradingAuthority? primaryTradingAuthority,
        string? addressLine2,
        string? addressLine3,
        string? county,
        string? recipientOrOrganisationName,
        Uri? website,
        string? businessDescription)
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName(legalOperatorName),
            new VendorName(tradingName),
            companyRegistrationNumber: null,
            new PrimaryContact(
                "Primary Contact",
                new EmailAddress("contact@example.test"),
                new TelephoneNumber("+44 20 7946 0123")),
            new CanonicalAddressId(canonicalAddressId),
            new BusinessAddressSnapshot(
                "10 Example Street",
                addressLine2,
                addressLine3,
                "LONDON",
                "AB1 2CD",
                county,
                recipientOrOrganisationName),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority,
            new TradingCharacteristics(
                tradingLocation,
                new OpeningHours(new TimeOnly(8, 30), new TimeOnly(22, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(Guid.Parse(vendorId)),
            information,
            website,
            businessDescription,
            new DateTimeOffset(2026, 8, 25, 9, 0, 0, TimeSpan.Zero));
    }
}
