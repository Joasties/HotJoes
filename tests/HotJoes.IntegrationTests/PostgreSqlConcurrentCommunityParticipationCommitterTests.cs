using HotJoes.Application.Community;
using HotJoes.Infrastructure.Community.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlConcurrentCommunityParticipationCommitterTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlConcurrentCommunityParticipationCommitterTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_EquivalentConcurrentRequests_ConvergeOnOriginalResultAndOneEvent()
    {
        Guid vendorId = Guid.NewGuid();
        await EnsureSchemaExistsAsync();
        await using CommunityPersistenceDbContext firstContext = CreateContext();
        await using CommunityPersistenceDbContext secondContext = CreateContext();
        var release = new StartRelease(2);
        PostgreSqlCommunityParticipationCommitter first = CreateCommitter(
            firstContext,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTimeOffset(2026, 9, 22, 17, 0, 0, TimeSpan.Zero));
        PostgreSqlCommunityParticipationCommitter second = CreateCommitter(
            secondContext,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTimeOffset(2026, 9, 22, 18, 0, 0, TimeSpan.Zero));
        JoinCommunityRequest request = new(vendorId, ContactPreference.Email);

        Task<CommunityParticipationCommitResult> firstTask = StartAsync(
            release,
            first,
            request);
        Task<CommunityParticipationCommitResult> secondTask = StartAsync(
            release,
            second,
            request);
        CommunityParticipationCommitResult[] results = await Task.WhenAll(
            firstTask,
            secondTask);

        Assert.Single(results.OfType<
            CommunityParticipationCommitResult.FirstRecorded>());
        CommunityParticipation[] observed = results.Select(result => result switch
            {
                CommunityParticipationCommitResult.FirstRecorded recorded =>
                    recorded.Participation,
                CommunityParticipationCommitResult.EquivalentReplay replay =>
                    replay.Participation,
                _ => throw new Xunit.Sdk.XunitException(
                    $"Unexpected result {result.GetType().Name}.")
            })
            .ToArray();
        Assert.Equal(
            observed[0].CommunityParticipationId,
            observed[1].CommunityParticipationId);
        Assert.Equal(observed[0].JoinedAt, observed[1].JoinedAt);

        await using CommunityPersistenceDbContext verify = CreateContext();
        CommunityParticipationRecord participation = Assert.Single(
            await verify.Set<CommunityParticipationRecord>()
                .AsNoTracking()
                .Where(record => record.VendorId == vendorId)
                .ToArrayAsync());
        Assert.Equal(
            1,
            await verify.Set<CommunityParticipationOutboxRecord>()
                .AsNoTracking()
                .CountAsync(record => record.CommunityParticipationId ==
                    participation.CommunityParticipationId));
    }

    [Fact]
    public async Task CommitAsync_ConflictingConcurrentRequests_ConvergeOnOneRecordAndConflict()
    {
        Guid vendorId = Guid.NewGuid();
        await EnsureSchemaExistsAsync();
        await using CommunityPersistenceDbContext firstContext = CreateContext();
        await using CommunityPersistenceDbContext secondContext = CreateContext();
        var release = new StartRelease(2);

        Task<CommunityParticipationCommitResult> firstTask = StartAsync(
            release,
            CreateCommitter(
                firstContext,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateTimeOffset(2026, 9, 22, 19, 0, 0, TimeSpan.Zero)),
            new JoinCommunityRequest(vendorId, ContactPreference.Email));
        Task<CommunityParticipationCommitResult> secondTask = StartAsync(
            release,
            CreateCommitter(
                secondContext,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateTimeOffset(2026, 9, 22, 20, 0, 0, TimeSpan.Zero)),
            new JoinCommunityRequest(vendorId, ContactPreference.Sms));
        CommunityParticipationCommitResult[] results = await Task.WhenAll(
            firstTask,
            secondTask);

        Assert.Single(results.OfType<
            CommunityParticipationCommitResult.FirstRecorded>());
        Assert.Single(results.OfType<
            CommunityParticipationCommitResult.PreferenceConflict>());

        await using CommunityPersistenceDbContext verify = CreateContext();
        CommunityParticipationRecord participation = Assert.Single(
            await verify.Set<CommunityParticipationRecord>()
                .AsNoTracking()
                .Where(record => record.VendorId == vendorId)
                .ToArrayAsync());
        Assert.Contains(
            participation.ContactPreference,
            new[] { "email", "sms" });
        Assert.Equal(
            1,
            await verify.Set<CommunityParticipationOutboxRecord>()
                .AsNoTracking()
                .CountAsync(record => record.CommunityParticipationId ==
                    participation.CommunityParticipationId));
    }

    private static async Task<CommunityParticipationCommitResult> StartAsync(
        StartRelease release,
        PostgreSqlCommunityParticipationCommitter committer,
        JoinCommunityRequest request)
    {
        await release.WaitAsync();
        return await committer.CommitAsync(request);
    }

    private async Task EnsureSchemaExistsAsync()
    {
        await using CommunityPersistenceDbContext context = CreateContext();
        await context.Database.MigrateAsync();
    }

    private CommunityPersistenceDbContext CreateContext()
    {
        DbContextOptions<CommunityPersistenceDbContext> options =
            new DbContextOptionsBuilder<CommunityPersistenceDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;
        return new CommunityPersistenceDbContext(options);
    }

    private static PostgreSqlCommunityParticipationCommitter CreateCommitter(
        CommunityPersistenceDbContext context,
        Guid participationId,
        Guid eventId,
        DateTimeOffset joinedAt)
    {
        return new PostgreSqlCommunityParticipationCommitter(
            context,
            new CommunityParticipationRecordedIntegrationEventMapper(),
            new CommunityParticipationRecordedIntegrationEventSerializer(),
            new FixedIdentityGenerator(participationId, eventId),
            new FixedTimeProvider(joinedAt));
    }

    private sealed class StartRelease
    {
        private readonly int _participantCount;
        private int _arrived;
        private readonly TaskCompletionSource _released =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public StartRelease(int participantCount)
        {
            _participantCount = participantCount;
        }

        public async Task WaitAsync()
        {
            if (Interlocked.Increment(ref _arrived) == _participantCount)
            {
                _released.SetResult();
            }

            await _released.Task;
        }
    }

    private sealed class FixedIdentityGenerator
        : ICommunityPersistenceIdentityGenerator
    {
        private readonly Guid _participationId;
        private readonly Guid _eventId;

        public FixedIdentityGenerator(Guid participationId, Guid eventId)
        {
            _participationId = participationId;
            _eventId = eventId;
        }

        public Guid NewCommunityParticipationId() => _participationId;

        public Guid NewEventId() => _eventId;
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
