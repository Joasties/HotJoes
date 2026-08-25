using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlNewVendorRegistrationCommitterTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlNewVendorRegistrationCommitterTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_CompleteCommit_PersistsVendorOutcomeAndExactOutboxItem()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        await using var context = new VendorRegistrationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        VendorAggregate vendor = CreateVendor(command, addressValues);
        var originalResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(vendor.Id));
        VendorRegistered completedFact = Assert.IsType<VendorRegistered>(
            Assert.Single(vendor.DomainEvents));
        VendorRegisteredIntegrationEvent integrationEvent =
            new VendorRegisteredIntegrationEventMapper().Map(
                completedFact,
                vendor,
                Guid.Parse("63c8bc6d-ff4d-42f8-8eb2-d69955fe89e4"),
                vendor.RegisteredAt);
        RegistrationSemanticFingerprint fingerprint =
            RegistrationSemanticFingerprint.Create(command, addressValues);
        var commit = new NewVendorRegistrationCommit(
            vendor,
            VendorRegistrationIdentity.Create(command, addressValues),
            fingerprint,
            originalResult,
            integrationEvent);
        var serializer = new VendorRegisteredIntegrationEventSerializer();
        byte[] expectedEventBytes = serializer
            .Serialize(integrationEvent)
            .SerializedEvent
            .ToArray();
        INewVendorRegistrationCommitter committer =
            new PostgreSqlNewVendorRegistrationCommitter(
                context,
                serializer);

        await committer.CommitAsync(commit, CancellationToken.None);
        context.ChangeTracker.Clear();

        VendorRegistrationRecord persistedVendor = await context
            .Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendor.Id.Value);
        VendorRegistrationOutcomeRecord persistedOutcome = await context
            .Set<VendorRegistrationOutcomeRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendor.Id.Value);
        VendorRegistrationOutboxRecord persistedOutbox = await context
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendor.Id.Value);

        Assert.Equal(vendor.Id.Value, persistedVendor.VendorId);
        Assert.Equal(
            commit.Identity.NormalizedTradingName.ToLowerInvariant(),
            persistedVendor.NormalizedTradingName);
        Assert.Equal(
            commit.Identity.NormalizedLegalOperatorName.ToLowerInvariant(),
            persistedVendor.NormalizedLegalOperatorName);
        Assert.Equal(
            commit.Identity.CanonicalAddressId.Value,
            persistedVendor.CanonicalAddressId);

        Assert.Equal(vendor.Id.Value, persistedOutcome.VendorId);
        Assert.Equal(fingerprint.Version, persistedOutcome.FingerprintVersion);
        Assert.Equal(
            Convert.FromHexString(fingerprint.Sha256Digest),
            persistedOutcome.SemanticFingerprintSha256);
        Assert.Equal("pendingActivation", persistedOutcome.ResultVendorState);

        Assert.Equal(integrationEvent.EventId, persistedOutbox.EventId);
        Assert.Equal(vendor.Id.Value, persistedOutbox.VendorId);
        Assert.Equal(integrationEvent.EventVersion, persistedOutbox.EventVersion);
        Assert.Equal(expectedEventBytes, persistedOutbox.SerializedEvent);
        Assert.Null(persistedOutbox.PublishedAtUtc);

        Assert.Equal(
            1,
            await context.Set<VendorRegistrationRecord>()
                .CountAsync(record => record.VendorId == vendor.Id.Value));
        Assert.Equal(
            1,
            await context.Set<VendorRegistrationOutcomeRecord>()
                .CountAsync(record => record.VendorId == vendor.Id.Value));
        Assert.Equal(
            1,
            await context.Set<VendorRegistrationOutboxRecord>()
                .CountAsync(record => record.VendorId == vendor.Id.Value));
    }

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            "Atomic Commit Kitchen",
            "Atomic Commit Operator Ltd",
            LegalOperatorType.LimitedCompany,
            "SC123456",
            TradingLocation.Stall,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Alex Morgan",
            "alex@example.test",
            "+44 20 7946 0123",
            "address-resolution-atomic-commit",
            "https://atomic-commit.example.test",
            "Hot food market stall.",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues CreateAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId("canonical-address-atomic-commit"),
            new BusinessAddressSnapshot(
                "2 High Street",
                "Market Square",
                "Stall 7",
                "GREENWICH",
                "SE10 8AA",
                "Greater London",
                "Atomic Commit Kitchen"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
    }

    private static VendorAggregate CreateVendor(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
    {
        var information = new VendorRegistrationInformation(
            command.LegalOperatorType,
            new VendorName(command.LegalOperatorName),
            new VendorName(command.TradingName),
            new CompanyRegistrationNumber(command.CompanyRegistrationNumber!),
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
            new VendorId(Guid.Parse("d4fa7633-0433-41e4-8e9b-b77b47004289")),
            information,
            new Uri(command.Website!),
            command.BusinessDescription,
            new DateTimeOffset(2026, 8, 25, 14, 0, 0, TimeSpan.Zero));
    }
}
