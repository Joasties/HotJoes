using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlOutboxRelayRetryTests
{
    private static readonly DateTimeOffset AttemptTime = new(
        2026,
        8,
        28,
        12,
        0,
        0,
        TimeSpan.Zero);

    private static readonly Guid WorkerId = Guid.Parse(
        "8dd895d4-8ac9-4477-b458-f22971f80e9c");

    private static readonly OutboxRelayRetryPolicy RetryPolicy = new(
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(25),
        automaticAttemptLimit: 3);

    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlOutboxRelayRetryTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task FailedAttempts_ScheduleConfiguredBoundedExponentialBackoff()
    {
        DbContextOptions<VendorRegistrationDbContext> options = CreateOptions();
        Guid eventId = await ResetSchemaAndSeedAsync(options, itemCount: 1);

        await ClaimSingleAsync(options, AttemptTime);
        await RecordFailureAsync(options, eventId, AttemptTime);

        await AssertOperationalStateAsync(
            options,
            eventId,
            attemptCount: 1,
            nextAttemptAtUtc: AttemptTime.AddSeconds(10),
            lastAttemptAtUtc: AttemptTime,
            isStalled: false);
        Assert.Empty(await ClaimAsync(options, AttemptTime.AddSeconds(9), 1));

        await ClaimSingleAsync(options, AttemptTime.AddSeconds(10));
        await RecordFailureAsync(
            options,
            eventId,
            AttemptTime.AddSeconds(10));

        await AssertOperationalStateAsync(
            options,
            eventId,
            attemptCount: 2,
            nextAttemptAtUtc: AttemptTime.AddSeconds(30),
            lastAttemptAtUtc: AttemptTime.AddSeconds(10),
            isStalled: false);

        await ClaimSingleAsync(options, AttemptTime.AddSeconds(30));
        await RecordFailureAsync(
            options,
            eventId,
            AttemptTime.AddSeconds(30));

        await AssertOperationalStateAsync(
            options,
            eventId,
            attemptCount: 3,
            nextAttemptAtUtc: null,
            lastAttemptAtUtc: AttemptTime.AddSeconds(30),
            isStalled: true);
    }

    [Fact]
    public async Task ExhaustedItem_RemainsDurableAndDoesNotBlockUnrelatedWork()
    {
        DbContextOptions<VendorRegistrationDbContext> options = CreateOptions();
        Guid stalledEventId = await ResetSchemaAndSeedAsync(options, itemCount: 2);

        await ExhaustAttemptsAsync(options, stalledEventId);

        OutboxRelayClaim available = Assert.Single(
            await ClaimAsync(options, AttemptTime.AddDays(1), batchSize: 2));

        Assert.NotEqual(stalledEventId, available.EventId);

        await using var verificationContext =
            new VendorRegistrationDbContext(options);
        VendorRegistrationOutboxRecord stalled = await verificationContext
            .Set<VendorRegistrationOutboxRecord>()
            .SingleAsync(record => record.EventId == stalledEventId);

        Assert.True(stalled.IsStalled);
        Assert.Equal(RetryPolicy.AutomaticAttemptLimit, stalled.AttemptCount);
        Assert.Equal(2, await verificationContext
            .Set<VendorRegistrationOutboxRecord>()
            .CountAsync());
    }

    [Fact]
    public async Task ExplicitAdministrativeRequeue_ResumesStalledImmutableWork()
    {
        DbContextOptions<VendorRegistrationDbContext> options = CreateOptions();
        Guid eventId = await ResetSchemaAndSeedAsync(options, itemCount: 1);
        await ExhaustAttemptsAsync(options, eventId);

        Assert.Empty(await ClaimAsync(options, AttemptTime.AddDays(1), 1));

        byte[] originalBytes;
        await using (var beforeContext = new VendorRegistrationDbContext(options))
        {
            originalBytes = (await beforeContext
                .Set<VendorRegistrationOutboxRecord>()
                .SingleAsync(record => record.EventId == eventId))
                .SerializedEvent
                .ToArray();
        }

        DateTimeOffset requeuedAt = AttemptTime.AddDays(1);
        await using (var requeueContext = new VendorRegistrationDbContext(options))
        {
            var store = new PostgreSqlOutboxRelayStore(requeueContext);
            Assert.True(await store.RequeueStalledAsync(eventId, requeuedAt));
        }

        OutboxRelayClaim requeued = Assert.Single(
            await ClaimAsync(options, requeuedAt, 1));

        Assert.Equal(eventId, requeued.EventId);
        Assert.Equal(originalBytes, requeued.SerializedEvent.ToArray());

        await using var verificationContext =
            new VendorRegistrationDbContext(options);
        VendorRegistrationOutboxRecord record = await verificationContext
            .Set<VendorRegistrationOutboxRecord>()
            .SingleAsync(item => item.EventId == eventId);

        Assert.False(record.IsStalled);
        Assert.Equal(0, record.AttemptCount);
        Assert.Null(record.NextAttemptAtUtc);
        Assert.Equal(AttemptTime.AddSeconds(30), record.LastAttemptAtUtc);
        Assert.Equal(
            OutboxRelayFailureCategory.PublicationFailed,
            record.LastFailureCategory);
        Assert.Equal(WorkerId, record.ClaimedBy);
        Assert.NotNull(record.ClaimExpiresAtUtc);
    }

    [Fact]
    public async Task Requeue_NonStalledItem_DoesNotChangeOperationalState()
    {
        DbContextOptions<VendorRegistrationDbContext> options = CreateOptions();
        Guid eventId = await ResetSchemaAndSeedAsync(options, itemCount: 1);

        await using var context = new VendorRegistrationDbContext(options);
        var store = new PostgreSqlOutboxRelayStore(context);

        Assert.False(await store.RequeueStalledAsync(eventId, AttemptTime));

        VendorRegistrationOutboxRecord record = await context
            .Set<VendorRegistrationOutboxRecord>()
            .SingleAsync(item => item.EventId == eventId);
        Assert.Equal(0, record.AttemptCount);
        Assert.Null(record.NextAttemptAtUtc);
        Assert.Null(record.LastAttemptAtUtc);
        Assert.Null(record.LastFailureCategory);
        Assert.False(record.IsStalled);
    }

    private DbContextOptions<VendorRegistrationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<VendorRegistrationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;
    }

    private static async Task ExhaustAttemptsAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        Guid eventId)
    {
        DateTimeOffset attemptAt = AttemptTime;

        for (int attempt = 1;
             attempt <= RetryPolicy.AutomaticAttemptLimit;
             attempt++)
        {
            await ClaimSingleAsync(options, attemptAt);
            await RecordFailureAsync(options, eventId, attemptAt);

            if (attempt < RetryPolicy.AutomaticAttemptLimit)
            {
                attemptAt += RetryPolicy.DelayForAttempt(attempt);
            }
        }
    }

    private static async Task RecordFailureAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        Guid eventId,
        DateTimeOffset failedAtUtc)
    {
        await using var context = new VendorRegistrationDbContext(options);
        var store = new PostgreSqlOutboxRelayStore(context);

        await store.RecordFailureAsync(
            eventId,
            WorkerId,
            failedAtUtc,
            OutboxRelayFailureCategory.PublicationFailed,
            RetryPolicy);
    }

    private static async Task<OutboxRelayClaim> ClaimSingleAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        DateTimeOffset claimedAtUtc)
    {
        return Assert.Single(await ClaimAsync(options, claimedAtUtc, 1));
    }

    private static async Task<IReadOnlyList<OutboxRelayClaim>> ClaimAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        DateTimeOffset claimedAtUtc,
        int batchSize)
    {
        await using var context = new VendorRegistrationDbContext(options);
        var store = new PostgreSqlOutboxRelayStore(context);

        return await store.ClaimEligibleAsync(
            WorkerId,
            claimedAtUtc,
            TimeSpan.FromMinutes(2),
            batchSize);
    }

    private static async Task AssertOperationalStateAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        Guid eventId,
        int attemptCount,
        DateTimeOffset? nextAttemptAtUtc,
        DateTimeOffset lastAttemptAtUtc,
        bool isStalled)
    {
        await using var context = new VendorRegistrationDbContext(options);
        VendorRegistrationOutboxRecord record = await context
            .Set<VendorRegistrationOutboxRecord>()
            .SingleAsync(item => item.EventId == eventId);

        Assert.Equal(attemptCount, record.AttemptCount);
        Assert.Equal(nextAttemptAtUtc, record.NextAttemptAtUtc);
        Assert.Equal(lastAttemptAtUtc, record.LastAttemptAtUtc);
        Assert.Equal(
            OutboxRelayFailureCategory.PublicationFailed,
            record.LastFailureCategory);
        Assert.Equal(isStalled, record.IsStalled);
        Assert.Null(record.ClaimedBy);
        Assert.Null(record.ClaimExpiresAtUtc);
        Assert.Null(record.PublishedAtUtc);
    }

    private static async Task<Guid> ResetSchemaAndSeedAsync(
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

        Guid firstEventId = Guid.Empty;

        for (int index = 0; index < itemCount; index++)
        {
            Guid vendorId = Guid.Parse(
                $"00000000-0000-0000-0000-{index + 101:D12}");
            Guid eventId = Guid.Parse(
                $"10000000-0000-0000-0000-{index + 101:D12}");
            firstEventId = firstEventId == Guid.Empty
                ? eventId
                : firstEventId;

            context.Set<VendorRegistrationRecord>().Add(
                VendorRegistrationRecordMapper.ToRecord(
                    CreateVendor(vendorId, index)));
            context.Set<VendorRegistrationOutboxRecord>().Add(
                new VendorRegistrationOutboxRecord
                {
                    EventId = eventId,
                    VendorId = vendorId,
                    EventVersion = 1,
                    SerializedEvent = [9, 8, 7, checked((byte)(index + 1))]
                });
        }

        await context.SaveChangesAsync();
        return firstEventId;
    }

    private static VendorAggregate CreateVendor(Guid vendorId, int index)
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName($"Retry Operator {index}"),
            new VendorName($"Retry Vendor {index}"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                $"Retry Contact {index}",
                new EmailAddress($"retry{index}@example.test"),
                new TelephoneNumber("+442079460123")),
            new CanonicalAddressId($"retry-address-{index}"),
            new BusinessAddressSnapshot(
                $"{index + 1} Retry Street",
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
            AttemptTime.AddMinutes(index));
    }
}
