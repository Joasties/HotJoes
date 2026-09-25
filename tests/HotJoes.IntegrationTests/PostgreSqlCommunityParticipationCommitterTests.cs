using HotJoes.Application.Community;
using HotJoes.Infrastructure.Community.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlCommunityParticipationCommitterTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlCommunityParticipationCommitterTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_FirstRequest_AtomicallyPersistsOriginalResultAndExactOutboxBytes()
    {
        Guid vendorId = Guid.NewGuid();
        Guid participationId = Guid.NewGuid();
        Guid eventId = Guid.NewGuid();
        DateTimeOffset joinedAt =
            new(2026, 9, 22, 11, 12, 13, TimeSpan.Zero);

        await using CommunityPersistenceDbContext context = CreateContext();
        await context.Database.MigrateAsync();
        PostgreSqlCommunityParticipationCommitter committer = CreateCommitter(
            context,
            participationId,
            eventId,
            joinedAt);

        CommunityParticipationCommitResult result = await committer.CommitAsync(
            new JoinCommunityRequest(vendorId, ContactPreference.WhatsApp));

        var recorded = Assert.IsType<
            CommunityParticipationCommitResult.FirstRecorded>(result);
        Assert.Equal(participationId, recorded.Participation.CommunityParticipationId);
        Assert.Equal(vendorId, recorded.Participation.VendorId);
        Assert.Equal(ContactPreference.WhatsApp, recorded.Participation.ContactPreference);
        Assert.Equal(joinedAt, recorded.Participation.JoinedAt);

        context.ChangeTracker.Clear();
        CommunityParticipationRecord persistedParticipation = await context
            .Set<CommunityParticipationRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendorId);
        CommunityParticipationOutboxRecord persistedOutbox = await context
            .Set<CommunityParticipationOutboxRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.CommunityParticipationId == participationId);

        Assert.Equal(participationId, persistedParticipation.CommunityParticipationId);
        Assert.Equal("whatsApp", persistedParticipation.ContactPreference);
        Assert.Equal(joinedAt, persistedParticipation.JoinedAtUtc);
        Assert.Equal(eventId, persistedOutbox.EventId);
        Assert.Equal(1, persistedOutbox.EventVersion);
        Assert.Null(persistedOutbox.PublishedAtUtc);

        CommunityParticipation expectedParticipation = new(
            participationId,
            vendorId,
            ContactPreference.WhatsApp,
            joinedAt);
        CommunityParticipationRecordedIntegrationEvent expectedEvent =
            new CommunityParticipationRecordedIntegrationEventMapper().Map(
                expectedParticipation,
                eventId,
                joinedAt);
        byte[] expectedBytes =
            new CommunityParticipationRecordedIntegrationEventSerializer()
                .Serialize(expectedEvent)
                .SerializedEvent.ToArray();
        Assert.Equal(expectedBytes, persistedOutbox.SerializedEvent);
    }

    [Fact]
    public async Task CommitAsync_EquivalentReplay_ReturnsOriginalAndCreatesNoSecondOutboxItem()
    {
        Guid vendorId = Guid.NewGuid();
        Guid originalParticipationId = Guid.NewGuid();
        Guid originalEventId = Guid.NewGuid();
        DateTimeOffset originalJoinedAt =
            new(2026, 9, 22, 12, 0, 0, TimeSpan.Zero);

        await using CommunityPersistenceDbContext firstContext = CreateContext();
        await firstContext.Database.MigrateAsync();
        CommunityParticipationCommitResult first = await CreateCommitter(
                firstContext,
                originalParticipationId,
                originalEventId,
                originalJoinedAt)
            .CommitAsync(new JoinCommunityRequest(
                vendorId,
                ContactPreference.Email));
        Assert.IsType<CommunityParticipationCommitResult.FirstRecorded>(first);

        await using CommunityPersistenceDbContext replayContext = CreateContext();
        CommunityParticipationCommitResult replay = await CreateCommitter(
                replayContext,
                Guid.NewGuid(),
                Guid.NewGuid(),
                originalJoinedAt.AddHours(4))
            .CommitAsync(new JoinCommunityRequest(
                vendorId,
                ContactPreference.Email));

        var alreadyRecorded = Assert.IsType<
            CommunityParticipationCommitResult.EquivalentReplay>(replay);
        Assert.Equal(
            originalParticipationId,
            alreadyRecorded.Participation.CommunityParticipationId);
        Assert.Equal(originalJoinedAt, alreadyRecorded.Participation.JoinedAt);
        Assert.Equal(
            1,
            await replayContext.Set<CommunityParticipationRecord>()
                .CountAsync(record => record.VendorId == vendorId));
        Assert.Equal(
            1,
            await replayContext.Set<CommunityParticipationOutboxRecord>()
                .CountAsync(record => record.CommunityParticipationId ==
                    originalParticipationId));
    }

    [Fact]
    public async Task CommitAsync_DifferentPreference_ReturnsConflictAndPreservesOriginal()
    {
        Guid vendorId = Guid.NewGuid();

        await using CommunityPersistenceDbContext firstContext = CreateContext();
        await firstContext.Database.MigrateAsync();
        await CreateCommitter(
                firstContext,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateTimeOffset(2026, 9, 22, 13, 0, 0, TimeSpan.Zero))
            .CommitAsync(new JoinCommunityRequest(
                vendorId,
                ContactPreference.Sms));

        await using CommunityPersistenceDbContext conflictContext = CreateContext();
        CommunityParticipationCommitResult conflict = await CreateCommitter(
                conflictContext,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateTimeOffset(2026, 9, 22, 14, 0, 0, TimeSpan.Zero))
            .CommitAsync(new JoinCommunityRequest(
                vendorId,
                ContactPreference.Email));

        Assert.IsType<CommunityParticipationCommitResult.PreferenceConflict>(
            conflict);
        CommunityParticipationRecord persisted = await conflictContext
            .Set<CommunityParticipationRecord>()
            .AsNoTracking()
            .SingleAsync(record => record.VendorId == vendorId);
        Assert.Equal("sms", persisted.ContactPreference);
        Assert.Equal(
            1,
            await conflictContext.Set<CommunityParticipationOutboxRecord>()
                .CountAsync(record => record.CommunityParticipationId ==
                    persisted.CommunityParticipationId));
    }

    [Fact]
    public async Task CommitAsync_OutboxInsertFails_RollsBackParticipationAndReturnsUnavailable()
    {
        Guid existingVendorId = Guid.NewGuid();
        Guid attemptedVendorId = Guid.NewGuid();
        Guid duplicateEventId = Guid.NewGuid();

        await using CommunityPersistenceDbContext firstContext = CreateContext();
        await firstContext.Database.MigrateAsync();
        await CreateCommitter(
                firstContext,
                Guid.NewGuid(),
                duplicateEventId,
                new DateTimeOffset(2026, 9, 22, 15, 0, 0, TimeSpan.Zero))
            .CommitAsync(new JoinCommunityRequest(
                existingVendorId,
                ContactPreference.Email));

        await using CommunityPersistenceDbContext failedContext = CreateContext();
        CommunityParticipationCommitResult result = await CreateCommitter(
                failedContext,
                Guid.NewGuid(),
                duplicateEventId,
                new DateTimeOffset(2026, 9, 22, 16, 0, 0, TimeSpan.Zero))
            .CommitAsync(new JoinCommunityRequest(
                attemptedVendorId,
                ContactPreference.Sms));

        Assert.IsType<CommunityParticipationCommitResult.PersistenceUnavailable>(
            result);
        Assert.False(await failedContext.Set<CommunityParticipationRecord>()
            .AsNoTracking()
            .AnyAsync(record => record.VendorId == attemptedVendorId));
        Assert.Equal(
            1,
            await failedContext.Set<CommunityParticipationOutboxRecord>()
                .AsNoTracking()
                .CountAsync(record => record.EventId == duplicateEventId));
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
            new FixedCommunityPersistenceIdentityGenerator(
                participationId,
                eventId),
            new FixedTimeProvider(joinedAt));
    }

    private sealed class FixedCommunityPersistenceIdentityGenerator
        : ICommunityPersistenceIdentityGenerator
    {
        private readonly Guid _participationId;
        private readonly Guid _eventId;

        public FixedCommunityPersistenceIdentityGenerator(
            Guid participationId,
            Guid eventId)
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
