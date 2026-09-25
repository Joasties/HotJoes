using System.Security.Cryptography;
using HotJoes.Infrastructure.CommunityConsumer;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlCommunityReceiptStoreTests
{
    private static readonly Guid EventId = Guid.Parse("7bb04edf-b6b0-4e52-8600-c9ac9b71cc36");
    private static readonly Guid ParticipationId = Guid.Parse("b590cd67-9bf2-46fd-b4aa-02f25f404275");
    private static readonly Guid VendorId = Guid.Parse("7ecf7027-d8f8-4bfb-8559-c65896e52e48");
    private static readonly DateTimeOffset ReceivedAt = new(2026, 9, 23, 18, 0, 0, TimeSpan.Zero);
    private static readonly byte[] Bytes = "{\"eventId\":\"7bb04edf-b6b0-4e52-8600-c9ac9b71cc36\"}"u8.ToArray();
    private readonly PostgreSqlFixture fixture;

    public PostgreSqlCommunityReceiptStoreTests(PostgreSqlFixture fixture) =>
        this.fixture = fixture;

    [Fact]
    public async Task VR_COMMUNITY_020_FirstReceiptStoresProcessingEvidenceAndHash()
    {
        DbContextOptions<CommunityReceiptDbContext> options = Options();
        await ResetAsync(options);
        await using var context = new CommunityReceiptDbContext(options);
        var outcome = await new PostgreSqlCommunityReceiptStore(context)
            .ClassifyAsync(Candidate(Bytes, ReceivedAt));
        Assert.Equal(CommunityReceiptOutcome.Recorded, outcome);
        CommunityReceiptRecord receipt = Assert.Single(await context
            .Set<CommunityReceiptRecord>().ToListAsync());
        Assert.Equal(EventId, receipt.EventId);
        Assert.Equal(ParticipationId, receipt.CommunityParticipationId);
        Assert.Equal(VendorId, receipt.VendorId);
        Assert.Equal(ReceivedAt, receipt.ReceivedAtUtc);
        Assert.Equal(SHA256.HashData(Bytes), receipt.SerializedEventSha256);
    }

    [Fact]
    public async Task VR_COMMUNITY_020_EquivalentAndConflictingRedeliveryUseOriginalReceipt()
    {
        DbContextOptions<CommunityReceiptDbContext> options = Options();
        await ResetAsync(options);
        Assert.Equal(CommunityReceiptOutcome.Recorded,
            await RecordAsync(options, Bytes, ReceivedAt));
        Assert.Equal(CommunityReceiptOutcome.EquivalentDuplicate,
            await RecordAsync(options, Bytes, ReceivedAt.AddDays(1)));
        byte[] conflict = [.. Bytes, 0];
        Assert.Equal(CommunityReceiptOutcome.ConflictingBytes,
            await RecordAsync(options, conflict, ReceivedAt.AddDays(2)));
        await using var verification = new CommunityReceiptDbContext(options);
        CommunityReceiptRecord receipt = Assert.Single(await verification
            .Set<CommunityReceiptRecord>().ToListAsync());
        Assert.Equal(ReceivedAt, receipt.ReceivedAtUtc);
        Assert.Equal(SHA256.HashData(Bytes), receipt.SerializedEventSha256);
    }

    [Fact]
    public async Task VR_COMMUNITY_020_ConcurrentEquivalentDeliveriesConvergeOnOneReceipt()
    {
        DbContextOptions<CommunityReceiptDbContext> options = Options();
        await ResetAsync(options);
        CommunityReceiptOutcome[] outcomes = await Task.WhenAll(
            RecordAsync(options, Bytes, ReceivedAt),
            RecordAsync(options, Bytes, ReceivedAt));
        Assert.Contains(CommunityReceiptOutcome.Recorded, outcomes);
        Assert.Contains(CommunityReceiptOutcome.EquivalentDuplicate, outcomes);
        await using var verification = new CommunityReceiptDbContext(options);
        Assert.Equal(1, await verification.Set<CommunityReceiptRecord>().CountAsync());
    }

    private DbContextOptions<CommunityReceiptDbContext> Options() =>
        new DbContextOptionsBuilder<CommunityReceiptDbContext>()
            .UseNpgsql(fixture.ConnectionString).Options;

    private static CommunityReceiptCandidate Candidate(byte[] bytes,
        DateTimeOffset receivedAt) => new(EventId,
            "CommunityParticipationRecorded", 1, ParticipationId, VendorId,
            receivedAt, bytes);

    private static async Task<CommunityReceiptOutcome> RecordAsync(
        DbContextOptions<CommunityReceiptDbContext> options, byte[] bytes,
        DateTimeOffset receivedAt)
    {
        await using var context = new CommunityReceiptDbContext(options);
        return await new PostgreSqlCommunityReceiptStore(context)
            .ClassifyAsync(Candidate(bytes, receivedAt));
    }

    private static async Task ResetAsync(
        DbContextOptions<CommunityReceiptDbContext> options)
    {
        await using var context = new CommunityReceiptDbContext(options);
        await context.Database.MigrateAsync();
        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE community.community_participation_receipts;");
    }
}
