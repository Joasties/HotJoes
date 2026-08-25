using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class VendorRegistrationOutcomeAndOutboxPostgreSqlTests
{
    private const string FingerprintHex =
        "06136046449514b1f748178ae7b2a5f2ad6ebed357a6549d15efcfc60fd351be";

    private readonly PostgreSqlFixture _fixture;

    public VendorRegistrationOutcomeAndOutboxPostgreSqlTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RegistrationOutcomeAndSerializedOutboxItem_RoundTripUnchanged()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        await using var context = new VendorRegistrationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        VendorAggregate vendor = CreateVendor();
        Guid eventId = Guid.Parse("bb5c1dda-f6e5-4254-a5ad-4f4148fab2ab");
        DateTimeOffset occurredAt = new(
            2026,
            8,
            25,
            10,
            15,
            30,
            TimeSpan.Zero);
        VendorRegistered completedFact = Assert.IsType<VendorRegistered>(
            Assert.Single(vendor.DomainEvents));
        VendorRegisteredIntegrationEvent integrationEvent =
            new VendorRegisteredIntegrationEventMapper().Map(
                completedFact,
                vendor,
                eventId,
                occurredAt);
        SerializedIntegrationEvent serialized =
            new VendorRegisteredIntegrationEventSerializer().Serialize(
                integrationEvent);
        byte[] expectedFingerprint = Convert.FromHexString(FingerprintHex);
        byte[] expectedSerializedEvent = serialized.SerializedEvent.ToArray();

        context.Set<VendorRegistrationRecord>().Add(
            VendorRegistrationRecordMapper.ToRecord(vendor));
        context.Set<VendorRegistrationOutcomeRecord>().Add(
            new VendorRegistrationOutcomeRecord
            {
                VendorId = vendor.Id.Value,
                FingerprintVersion = 1,
                SemanticFingerprintSha256 = expectedFingerprint.ToArray(),
                ResultVendorState = "pendingActivation"
            });
        context.Set<VendorRegistrationOutboxRecord>().Add(
            new VendorRegistrationOutboxRecord
            {
                EventId = serialized.EventId,
                VendorId = vendor.Id.Value,
                EventVersion = serialized.EventVersion,
                SerializedEvent = expectedSerializedEvent.ToArray(),
                PublishedAtUtc = null
            });

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        VendorRegistrationOutcomeRecord persistedOutcome = await context
            .Set<VendorRegistrationOutcomeRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendor.Id.Value);
        VendorRegistrationOutboxRecord persistedOutbox = await context
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.EventId == eventId);

        Assert.Equal(vendor.Id.Value, persistedOutcome.VendorId);
        Assert.Equal((short)1, persistedOutcome.FingerprintVersion);
        Assert.Equal(
            expectedFingerprint,
            persistedOutcome.SemanticFingerprintSha256);
        Assert.Equal("pendingActivation", persistedOutcome.ResultVendorState);

        Assert.Equal(eventId, persistedOutbox.EventId);
        Assert.Equal(vendor.Id.Value, persistedOutbox.VendorId);
        Assert.Equal(1, persistedOutbox.EventVersion);
        Assert.Equal(expectedSerializedEvent, persistedOutbox.SerializedEvent);
        Assert.Null(persistedOutbox.PublishedAtUtc);
    }

    private static VendorAggregate CreateVendor()
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName("Morgan Lee"),
            new VendorName("Morgan's Evening Kitchen"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                "Morgan Lee",
                new EmailAddress("morgan@example.test"),
                new TelephoneNumber("+44 20 7946 0123")),
            new CanonicalAddressId("address-outcome-outbox-round-trip"),
            new BusinessAddressSnapshot(
                "12 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(Guid.Parse("8bd572a7-cf19-41e1-93ab-70478d6fa660")),
            information,
            website: null,
            businessDescription: null,
            new DateTimeOffset(2026, 8, 25, 10, 0, 0, TimeSpan.Zero));
    }
}
