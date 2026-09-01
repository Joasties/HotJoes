using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlOutboxRelayClaimTests
{
    private const string TraceParent =
        "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
    private const string TraceState = "vendor=hotjoes";

    private static readonly DateTimeOffset ClaimTime = new(
        2026,
        8,
        28,
        10,
        0,
        0,
        TimeSpan.Zero);

    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlOutboxRelayClaimTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ConcurrentWorkers_ClaimDisjointBoundedBatches()
    {
        DbContextOptions<VendorRegistrationDbContext> options = CreateOptions();
        await ResetSchemaAndSeedAsync(options, itemCount: 4);

        await using var firstContext = new VendorRegistrationDbContext(options);
        await using var secondContext = new VendorRegistrationDbContext(options);
        var firstStore = new PostgreSqlOutboxRelayStore(firstContext);
        var secondStore = new PostgreSqlOutboxRelayStore(secondContext);

        Task<IReadOnlyList<OutboxRelayClaim>> firstClaim =
            firstStore.ClaimEligibleAsync(
                Guid.Parse("0e946953-30aa-4399-a28b-4a77106238ca"),
                ClaimTime,
                TimeSpan.FromMinutes(2),
                batchSize: 2);
        Task<IReadOnlyList<OutboxRelayClaim>> secondClaim =
            secondStore.ClaimEligibleAsync(
                Guid.Parse("c601f7ee-f6ba-4737-a207-8013544749f9"),
                ClaimTime,
                TimeSpan.FromMinutes(2),
                batchSize: 2);

        await Task.WhenAll(firstClaim, secondClaim);

        OutboxRelayClaim[] first = (await firstClaim).ToArray();
        OutboxRelayClaim[] second = (await secondClaim).ToArray();

        Assert.Equal(2, first.Length);
        Assert.Equal(2, second.Length);
        Assert.Empty(first.Select(claim => claim.EventId)
            .Intersect(second.Select(claim => claim.EventId)));
        Assert.Equal(
            4,
            first.Concat(second).Select(claim => claim.EventId).Distinct().Count());
    }

    [Fact]
    public async Task ExpiredLease_BecomesClaimableWithoutChangingStoredEvent()
    {
        DbContextOptions<VendorRegistrationDbContext> options = CreateOptions();
        await ResetSchemaAndSeedAsync(options, itemCount: 1);
        Guid firstWorker = Guid.Parse("c39af7b8-57d7-481c-b507-e3d2df2d99a3");
        Guid recoveryWorker = Guid.Parse("abdaf825-89cb-42ea-a3ce-a7a84130fb4f");

        OutboxRelayClaim original;
        await using (var context = new VendorRegistrationDbContext(options))
        {
            var store = new PostgreSqlOutboxRelayStore(context);
            original = Assert.Single(await store.ClaimEligibleAsync(
                firstWorker,
                ClaimTime,
                TimeSpan.FromMinutes(2),
                batchSize: 1));
        }

        await using (var context = new VendorRegistrationDbContext(options))
        {
            var store = new PostgreSqlOutboxRelayStore(context);
            Assert.Empty(await store.ClaimEligibleAsync(
                recoveryWorker,
                ClaimTime.AddMinutes(1),
                TimeSpan.FromMinutes(2),
                batchSize: 1));
        }

        OutboxRelayClaim recovered;
        await using (var context = new VendorRegistrationDbContext(options))
        {
            var store = new PostgreSqlOutboxRelayStore(context);
            recovered = Assert.Single(await store.ClaimEligibleAsync(
                recoveryWorker,
                ClaimTime.AddMinutes(2),
                TimeSpan.FromMinutes(2),
                batchSize: 1));
        }

        Assert.Equal(original.EventId, recovered.EventId);
        Assert.Equal(original.EventVersion, recovered.EventVersion);
        Assert.Equal(original.SerializedEvent.ToArray(), recovered.SerializedEvent.ToArray());
        Assert.Equal(TraceParent, original.TraceParent);
        Assert.Equal(TraceState, original.TraceState);
        Assert.Equal(original.TraceParent, recovered.TraceParent);
        Assert.Equal(original.TraceState, recovered.TraceState);
    }

    private DbContextOptions<VendorRegistrationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<VendorRegistrationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;
    }

    private static async Task ResetSchemaAndSeedAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        int itemCount)
    {
        await using var context = new VendorRegistrationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                vendor_registration_outbox,
                vendor_registration_outcomes,
                vendor_registrations
            RESTART IDENTITY CASCADE;
            """);

        for (int index = 0; index < itemCount; index++)
        {
            Guid vendorId = Guid.Parse($"00000000-0000-0000-0000-{index + 1:D12}");
            Guid eventId = Guid.Parse($"10000000-0000-0000-0000-{index + 1:D12}");
            VendorAggregate vendor = CreateVendor(vendorId, index);

            context.Set<VendorRegistrationRecord>().Add(
                VendorRegistrationRecordMapper.ToRecord(vendor));
            context.Set<VendorRegistrationOutboxRecord>().Add(
                new VendorRegistrationOutboxRecord
                {
                    EventId = eventId,
                    VendorId = vendorId,
                    EventVersion = 1,
                    SerializedEvent = [1, 2, 3, checked((byte)(index + 1))],
                    TraceParent = TraceParent,
                    TraceState = TraceState,
                    PublishedAtUtc = null
                });
        }

        await context.SaveChangesAsync();
    }

    private static VendorAggregate CreateVendor(Guid vendorId, int index)
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName($"Relay Operator {index}"),
            new VendorName($"Relay Vendor {index}"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                $"Relay Contact {index}",
                new EmailAddress($"relay{index}@example.test"),
                new TelephoneNumber("+442079460123")),
            new CanonicalAddressId($"relay-address-{index}"),
            new BusinessAddressSnapshot(
                $"{index + 1} Relay Street",
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
                new OpeningHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(vendorId),
            information,
            website: null,
            businessDescription: null,
            ClaimTime.AddMinutes(index));
    }
}
