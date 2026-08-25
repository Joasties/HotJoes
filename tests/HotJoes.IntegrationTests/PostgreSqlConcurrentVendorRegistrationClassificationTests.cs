using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlConcurrentVendorRegistrationClassificationTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlConcurrentVendorRegistrationClassificationTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_CompositeIdentityAlreadyCommitted_ThrowsRaceSignalAndRollsBack()
    {
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        NewVendorRegistrationCommit winner = CreateCommit(
            command,
            addressValues,
            Guid.Parse("a4903aef-6ba2-42b2-8295-9fe8ad6cfb48"),
            Guid.Parse("906d45f5-7b89-44d0-b180-1c4974288e8e"));
        NewVendorRegistrationCommit loser = CreateCommit(
            command,
            addressValues,
            Guid.Parse("ba5c719f-c22a-4cb6-933a-e2a4bc9feb90"),
            Guid.Parse("b7eeae14-aa51-4dc0-8e8c-54bbce3422b4"));

        await using (VendorRegistrationDbContext winnerContext = CreateContext())
        {
            await winnerContext.Database.EnsureCreatedAsync();
            var committer = new PostgreSqlNewVendorRegistrationCommitter(
                winnerContext,
                new VendorRegisteredIntegrationEventSerializer());
            await committer.CommitAsync(winner, CancellationToken.None);
        }

        await using (VendorRegistrationDbContext loserContext = CreateContext())
        {
            var committer = new PostgreSqlNewVendorRegistrationCommitter(
                loserContext,
                new VendorRegisteredIntegrationEventSerializer());

            await Assert.ThrowsAsync<ConcurrentVendorRegistrationException>(
                () => committer.CommitAsync(loser, CancellationToken.None));
        }

        await using VendorRegistrationDbContext verifyContext = CreateContext();
        Guid winnerId = winner.Vendor.Id.Value;
        Guid loserId = loser.Vendor.Id.Value;

        Assert.True(await verifyContext.Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == winnerId));
        Assert.True(await verifyContext.Set<VendorRegistrationOutcomeRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == winnerId));
        Assert.True(await verifyContext.Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == winnerId));

        Assert.False(await verifyContext.Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == loserId));
        Assert.False(await verifyContext.Set<VendorRegistrationOutcomeRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == loserId));
        Assert.False(await verifyContext.Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == loserId));

        Assert.Equal(
            1,
            await verifyContext.Set<VendorRegistrationRecord>()
                .AsNoTracking()
                .CountAsync(record =>
                    record.NormalizedTradingName ==
                        winner.Identity.NormalizedTradingName.ToLowerInvariant()
                    && record.NormalizedLegalOperatorName ==
                        winner.Identity.NormalizedLegalOperatorName.ToLowerInvariant()
                    && record.CanonicalAddressId ==
                        winner.Identity.CanonicalAddressId.Value));
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static NewVendorRegistrationCommit CreateCommit(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        Guid vendorId,
        Guid eventId)
    {
        VendorAggregate vendor = CreateVendor(
            command,
            addressValues,
            vendorId);
        VendorRegistered completedFact = Assert.IsType<VendorRegistered>(
            Assert.Single(vendor.DomainEvents));
        VendorRegisteredIntegrationEvent integrationEvent =
            new VendorRegisteredIntegrationEventMapper().Map(
                completedFact,
                vendor,
                eventId,
                vendor.RegisteredAt);
        var originalResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(vendor.Id));

        return new NewVendorRegistrationCommit(
            vendor,
            VendorRegistrationIdentity.Create(command, addressValues),
            RegistrationSemanticFingerprint.Create(command, addressValues),
            originalResult,
            integrationEvent);
    }

    private static VendorAggregate CreateVendor(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        Guid vendorId)
    {
        var information = new VendorRegistrationInformation(
            command.LegalOperatorType,
            new VendorName(command.LegalOperatorName),
            new VendorName(command.TradingName),
            companyRegistrationNumber: null,
            new PrimaryContact(
                command.ContactName,
                new EmailAddress(command.ContactEmail),
                new TelephoneNumber(command.ContactTelephone)),
            addressValues.CanonicalAddressId,
            addressValues.BusinessAddressSnapshot,
            addressValues.FoodRegistrationAuthority,
            addressValues.PrimaryTradingAuthority,
            new TradingCharacteristics(
                command.TradingLocation,
                new OpeningHours(
                    command.OpeningHoursStartTime,
                    command.OpeningHoursEndTime),
                command.ServiceIncludesHotFood,
                command.AlcoholService));

        return VendorAggregate.Register(
            new VendorId(vendorId),
            information,
            website: null,
            businessDescription: command.BusinessDescription,
            new DateTimeOffset(2026, 8, 25, 18, 0, 0, TimeSpan.Zero));
    }

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            "Concurrent Classification Kitchen",
            "Concurrent Classification Operator",
            LegalOperatorType.SoleTrader,
            companyRegistrationNumber: null,
            TradingLocation.Kitchen,
            new TimeOnly(17, 0),
            new TimeOnly(2, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Jamie Taylor",
            "jamie@example.test",
            "+44 20 7946 0123",
            "address-reference-concurrent-classification",
            website: null,
            businessDescription: "Concurrent classification registration.",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues CreateAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId(
                "canonical-address-concurrent-classification"),
            new BusinessAddressSnapshot(
                "16 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null);
    }
}
