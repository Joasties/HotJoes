using HotJoes.Infrastructure.Community.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlCommunityOutboxRelayStoreTests
{
    private const string TraceParent =
        "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
    private const string TraceState = "community=hotjoes";
    private static readonly DateTimeOffset ClaimTime = new(
        2026, 9, 23, 13, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture fixture;

    public PostgreSqlCommunityOutboxRelayStoreTests(PostgreSqlFixture fixture) =>
        this.fixture = fixture;

    [Fact]
    public async Task AI_EVT_005_ConcurrentWorkersClaimDisjointBoundedBatches()
    {
        await ResetAndSeedAsync(itemCount: 4);
        await using CommunityPersistenceDbContext firstContext = CreateContext();
        await using CommunityPersistenceDbContext secondContext = CreateContext();
        var firstStore = new PostgreSqlCommunityOutboxRelayStore(firstContext);
        var secondStore = new PostgreSqlCommunityOutboxRelayStore(secondContext);

        Task<IReadOnlyList<CommunityOutboxRelayClaim>> firstClaim =
            firstStore.ClaimEligibleAsync(
                Guid.Parse("589f23d9-e588-4d1b-8ab0-d4018f60f515"),
                ClaimTime,
                TimeSpan.FromMinutes(2),
                batchSize: 2);
        Task<IReadOnlyList<CommunityOutboxRelayClaim>> secondClaim =
            secondStore.ClaimEligibleAsync(
                Guid.Parse("5205e3f7-a78c-4f5d-b5a3-805c6281cd9f"),
                ClaimTime,
                TimeSpan.FromMinutes(2),
                batchSize: 2);

        await Task.WhenAll(firstClaim, secondClaim);

        CommunityOutboxRelayClaim[] first = (await firstClaim).ToArray();
        CommunityOutboxRelayClaim[] second = (await secondClaim).ToArray();
        Assert.Equal(2, first.Length);
        Assert.Equal(2, second.Length);
        Assert.Empty(first.Select(item => item.EventId)
            .Intersect(second.Select(item => item.EventId)));
        Assert.Equal(4, first.Concat(second)
            .Select(item => item.EventId).Distinct().Count());
    }

    [Fact]
    public async Task AI_EVT_005_ExpiredLeasePreservesImmutableEventAndTraceContext()
    {
        await ResetAndSeedAsync(itemCount: 1);
        Guid firstWorker = Guid.Parse("33fd3933-9ab8-42bb-9352-b32aa52a04c7");
        Guid recoveryWorker = Guid.Parse("2d64640d-0d3b-43a3-a695-73bd613a852e");

        CommunityOutboxRelayClaim original;
        await using (CommunityPersistenceDbContext context = CreateContext())
        {
            original = Assert.Single(await
                new PostgreSqlCommunityOutboxRelayStore(context)
                    .ClaimEligibleAsync(
                        firstWorker,
                        ClaimTime,
                        TimeSpan.FromMinutes(2),
                        batchSize: 1));
        }

        await using (CommunityPersistenceDbContext context = CreateContext())
        {
            Assert.Empty(await new PostgreSqlCommunityOutboxRelayStore(context)
                .ClaimEligibleAsync(
                    recoveryWorker,
                    ClaimTime.AddMinutes(1),
                    TimeSpan.FromMinutes(2),
                    batchSize: 1));
        }

        CommunityOutboxRelayClaim recovered;
        await using (CommunityPersistenceDbContext context = CreateContext())
        {
            recovered = Assert.Single(await
                new PostgreSqlCommunityOutboxRelayStore(context)
                    .ClaimEligibleAsync(
                        recoveryWorker,
                        ClaimTime.AddMinutes(2),
                        TimeSpan.FromMinutes(2),
                        batchSize: 1));
        }

        Assert.Equal(original.EventId, recovered.EventId);
        Assert.Equal(original.EventVersion, recovered.EventVersion);
        Assert.Equal(
            original.SerializedEvent.ToArray(),
            recovered.SerializedEvent.ToArray());
        Assert.Equal(TraceParent, recovered.TraceParent);
        Assert.Equal(TraceState, recovered.TraceState);
    }

    [Fact]
    public async Task AI_EVT_005_FailureBackoffStallRequeueAndPublicationRemainDurable()
    {
        await ResetAndSeedAsync(itemCount: 1);
        Guid workerId = Guid.Parse("92707e68-657d-4ae2-8ef2-31f98db951e2");
        var policy = new CommunityOutboxRelayRetryPolicy(
            TimeSpan.FromSeconds(10),
            TimeSpan.FromMinutes(1),
            automaticAttemptLimit: 2);

        Guid eventId;
        await using (CommunityPersistenceDbContext context = CreateContext())
        {
            var store = new PostgreSqlCommunityOutboxRelayStore(context);
            eventId = Assert.Single(await store.ClaimEligibleAsync(
                workerId, ClaimTime, TimeSpan.FromMinutes(2), 1)).EventId;
            await store.RecordFailureAsync(
                eventId,
                workerId,
                ClaimTime,
                CommunityOutboxRelayFailureCategory.PublicationFailed,
                policy);
        }

        await using (CommunityPersistenceDbContext context = CreateContext())
        {
            var store = new PostgreSqlCommunityOutboxRelayStore(context);
            CommunityOutboxRelayClaim retry = Assert.Single(
                await store.ClaimEligibleAsync(
                    workerId,
                    ClaimTime.AddSeconds(10),
                    TimeSpan.FromMinutes(2),
                    1));
            await store.RecordFailureAsync(
                retry.EventId,
                workerId,
                ClaimTime.AddSeconds(10),
                CommunityOutboxRelayFailureCategory.PublicationFailed,
                policy);
            Assert.Empty(await store.ClaimEligibleAsync(
                workerId,
                ClaimTime.AddHours(1),
                TimeSpan.FromMinutes(2),
                1));
            Assert.True(await store.RequeueStalledAsync(
                eventId,
                ClaimTime.AddHours(1)));
        }

        await using (CommunityPersistenceDbContext context = CreateContext())
        {
            var store = new PostgreSqlCommunityOutboxRelayStore(context);
            Assert.Single(await store.ClaimEligibleAsync(
                workerId,
                ClaimTime.AddHours(1),
                TimeSpan.FromMinutes(2),
                1));
            await store.MarkPublishedAsync(
                eventId,
                workerId,
                ClaimTime.AddHours(1));
            Assert.Empty(await store.ClaimEligibleAsync(
                workerId,
                ClaimTime.AddHours(2),
                TimeSpan.FromMinutes(2),
                1));
        }
    }

    private CommunityPersistenceDbContext CreateContext()
    {
        DbContextOptions<CommunityPersistenceDbContext> options =
            new DbContextOptionsBuilder<CommunityPersistenceDbContext>()
                .UseNpgsql(fixture.ConnectionString)
                .Options;
        return new CommunityPersistenceDbContext(options);
    }

    private async Task ResetAndSeedAsync(int itemCount)
    {
        await using CommunityPersistenceDbContext context = CreateContext();
        await context.Database.MigrateAsync();
        await context.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                community.community_participation_outbox,
                community.community_participations
            CASCADE;
            """);

        for (int index = 0; index < itemCount; index++)
        {
            Guid participationId = Guid.Parse(
                $"20000000-0000-0000-0000-{index + 1:D12}");
            Guid vendorId = Guid.Parse(
                $"30000000-0000-0000-0000-{index + 1:D12}");
            Guid eventId = Guid.Parse(
                $"40000000-0000-0000-0000-{index + 1:D12}");
            context.Set<CommunityParticipationRecord>().Add(new()
            {
                CommunityParticipationId = participationId,
                VendorId = vendorId,
                ContactPreference = "email",
                JoinedAtUtc = ClaimTime.AddMinutes(index)
            });
            context.Set<CommunityParticipationOutboxRecord>().Add(new()
            {
                EventId = eventId,
                CommunityParticipationId = participationId,
                EventVersion = 1,
                SerializedEvent = [10, 20, 30, checked((byte)(index + 1))],
                TraceParent = TraceParent,
                TraceState = TraceState
            });
        }

        await context.SaveChangesAsync();
    }
}
