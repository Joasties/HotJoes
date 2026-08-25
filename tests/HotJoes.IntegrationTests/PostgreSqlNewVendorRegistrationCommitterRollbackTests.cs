using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlNewVendorRegistrationCommitterRollbackTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlNewVendorRegistrationCommitterRollbackTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_OutboxWriteFails_RollsBackVendorOutcomeAndOutbox()
    {
        Guid existingVendorId =
            Guid.Parse("c522b668-154c-41da-822b-dfef165d0ffc");
        Guid attemptedVendorId =
            Guid.Parse("131a2b92-69c7-43d7-a75a-af43af70e612");
        Guid duplicateEventId =
            Guid.Parse("5d955e77-724d-4568-a057-e836881ffb92");

        await using (VendorRegistrationDbContext arrangeContext = CreateContext())
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Set<VendorRegistrationRecord>().Add(
                CreateExistingVendorRecord(existingVendorId));
            arrangeContext.Set<VendorRegistrationOutboxRecord>().Add(
                new VendorRegistrationOutboxRecord
                {
                    EventId = duplicateEventId,
                    VendorId = existingVendorId,
                    EventVersion = 1,
                    SerializedEvent = [1],
                    PublishedAtUtc = null
                });
            await arrangeContext.SaveChangesAsync();
        }

        NewVendorRegistrationCommit commit = CreateAttemptedCommit(
            attemptedVendorId,
            duplicateEventId);

        await using (VendorRegistrationDbContext actContext = CreateContext())
        {
            var committer = new PostgreSqlNewVendorRegistrationCommitter(
                actContext,
                new VendorRegisteredIntegrationEventSerializer());

            await Assert.ThrowsAsync<DbUpdateException>(
                () => committer.CommitAsync(commit, CancellationToken.None));
        }

        await using VendorRegistrationDbContext verifyContext = CreateContext();

        Assert.False(await verifyContext.Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == attemptedVendorId));
        Assert.False(await verifyContext.Set<VendorRegistrationOutcomeRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == attemptedVendorId));
        Assert.False(await verifyContext.Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == attemptedVendorId));

        VendorRegistrationOutboxRecord existingOutbox = await verifyContext
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.EventId == duplicateEventId);
        Assert.Equal(existingVendorId, existingOutbox.VendorId);
        Assert.Equal(new byte[] { 1 }, existingOutbox.SerializedEvent);
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static NewVendorRegistrationCommit CreateAttemptedCommit(
        Guid vendorId,
        Guid eventId)
    {
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        VendorAggregate vendor = CreateVendor(
            vendorId,
            command,
            addressValues);
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

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            "Rollback Test Kitchen",
            "Rollback Test Operator",
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
            "address-resolution-rollback-test",
            website: null,
            businessDescription: null,
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues CreateAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId("canonical-address-rollback-test"),
            new BusinessAddressSnapshot(
                "14 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null);
    }

    private static VendorAggregate CreateVendor(
        Guid vendorId,
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
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
            businessDescription: null,
            new DateTimeOffset(2026, 8, 25, 15, 0, 0, TimeSpan.Zero));
    }

    private static VendorRegistrationRecord CreateExistingVendorRecord(
        Guid vendorId)
    {
        return new VendorRegistrationRecord
        {
            VendorId = vendorId,
            VendorState = "pendingActivation",
            TradingPreference = "offline",
            RegisteredAtUtc = new DateTimeOffset(
                2026,
                8,
                25,
                14,
                30,
                0,
                TimeSpan.Zero),
            LegalOperatorType = "soleTrader",
            LegalOperatorName = "Existing Event Owner",
            NormalizedLegalOperatorName = "existing event owner",
            TradingName = "Existing Event Kitchen",
            NormalizedTradingName = "existing event kitchen",
            CompanyRegistrationNumber = null,
            ContactName = "Existing Contact",
            ContactEmail = "existing@example.test",
            ContactTelephone = "+44 20 7946 0999",
            CanonicalAddressId = "canonical-address-existing-event-owner",
            RecipientOrOrganisationName = null,
            AddressLine1 = "1 Existing Street",
            AddressLine2 = null,
            AddressLine3 = null,
            PostTown = "LONDON",
            Postcode = "AB1 2CD",
            County = null,
            FoodRegistrationAuthority = "Greenwich Borough Council",
            PrimaryTradingAuthority = null,
            TradingLocation = "kitchen",
            OpeningHoursStart = new TimeOnly(8, 0),
            OpeningHoursEnd = new TimeOnly(22, 0),
            ServiceIncludesHotFood = true,
            AlcoholService = false,
            Website = null,
            BusinessDescription = null
        };
    }
}
