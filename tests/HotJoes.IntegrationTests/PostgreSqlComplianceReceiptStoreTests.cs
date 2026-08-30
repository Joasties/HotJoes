using System.Security.Cryptography;
using HotJoes.Infrastructure.ComplianceConsumer;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(CompliancePostgreSqlCollection.Name)]
public sealed class PostgreSqlComplianceReceiptStoreTests
{
    private static readonly Guid EventId = Guid.Parse(
        "81d2a757-fefd-42c9-bd82-e3ebc3a09146");

    private static readonly DateTimeOffset ReceivedAtUtc = new(
        2026,
        8,
        28,
        18,
        0,
        0,
        TimeSpan.Zero);

    private static readonly byte[] SerializedEvent =
        "{\"eventId\":\"81d2a757-fefd-42c9-bd82-e3ebc3a09146\",\"eventType\":\"VendorRegistered\",\"eventVersion\":1,\"payload\":{}}"u8
            .ToArray();

    private readonly CompliancePostgreSqlFixture _fixture;

    public PostgreSqlComplianceReceiptStoreTests(
        CompliancePostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RecordAsync_FirstDelivery_PersistsExactReceiptAndSha256()
    {
        DbContextOptions<ComplianceReceiptDbContext> options = CreateOptions();
        await ResetSchemaAsync(options);

        await using var context = new ComplianceReceiptDbContext(options);
        var store = new PostgreSqlComplianceReceiptStore(context);

        ComplianceReceiptOutcome outcome = await store.RecordAsync(
            CreateCandidate(SerializedEvent, ReceivedAtUtc));

        Assert.Equal(ComplianceReceiptOutcome.Recorded, outcome);
        ComplianceReceiptRecord receipt = Assert.Single(
            await context.Set<ComplianceReceiptRecord>().ToListAsync());
        Assert.Equal(EventId, receipt.EventId);
        Assert.Equal("VendorRegistered", receipt.EventType);
        Assert.Equal(1, receipt.EventVersion);
        Assert.Equal(ReceivedAtUtc, receipt.ReceivedAtUtc);
        Assert.Equal(
            SHA256.HashData(SerializedEvent),
            receipt.SerializedEventSha256);
        Assert.Equal(32, receipt.SerializedEventSha256.Length);
    }

    [Fact]
    public async Task RecordAsync_EquivalentDuplicate_ReturnsExistingReceiptWithoutMutation()
    {
        DbContextOptions<ComplianceReceiptDbContext> options = CreateOptions();
        await ResetSchemaAsync(options);
        await RecordAsync(options, SerializedEvent, ReceivedAtUtc);

        ComplianceReceiptOutcome duplicate = await RecordAsync(
            options,
            SerializedEvent,
            ReceivedAtUtc.AddDays(1));

        Assert.Equal(
            ComplianceReceiptOutcome.EquivalentDuplicate,
            duplicate);
        await using var verificationContext =
            new ComplianceReceiptDbContext(options);
        ComplianceReceiptRecord receipt = Assert.Single(
            await verificationContext
                .Set<ComplianceReceiptRecord>()
                .ToListAsync());
        Assert.Equal(ReceivedAtUtc, receipt.ReceivedAtUtc);
        Assert.Equal(
            SHA256.HashData(SerializedEvent),
            receipt.SerializedEventSha256);
    }

    [Fact]
    public async Task RecordAsync_ConflictingBytes_PreservesOriginalReceipt()
    {
        DbContextOptions<ComplianceReceiptDbContext> options = CreateOptions();
        await ResetSchemaAsync(options);
        await RecordAsync(options, SerializedEvent, ReceivedAtUtc);
        byte[] conflictingBytes = [.. SerializedEvent, 0];

        ComplianceReceiptOutcome conflict = await RecordAsync(
            options,
            conflictingBytes,
            ReceivedAtUtc.AddMinutes(1));

        Assert.Equal(ComplianceReceiptOutcome.ConflictingBytes, conflict);
        await using var verificationContext =
            new ComplianceReceiptDbContext(options);
        ComplianceReceiptRecord receipt = Assert.Single(
            await verificationContext
                .Set<ComplianceReceiptRecord>()
                .ToListAsync());
        Assert.Equal(ReceivedAtUtc, receipt.ReceivedAtUtc);
        Assert.Equal(
            SHA256.HashData(SerializedEvent),
            receipt.SerializedEventSha256);
    }

    [Fact]
    public async Task RecordAsync_ConcurrentEquivalentDuplicates_ConvergeOnOneReceipt()
    {
        DbContextOptions<ComplianceReceiptDbContext> options = CreateOptions();
        await ResetSchemaAsync(options);

        Task<ComplianceReceiptOutcome> first = RecordAsync(
            options,
            SerializedEvent,
            ReceivedAtUtc);
        Task<ComplianceReceiptOutcome> second = RecordAsync(
            options,
            SerializedEvent,
            ReceivedAtUtc);

        ComplianceReceiptOutcome[] outcomes = await Task.WhenAll(
            first,
            second);

        Assert.Contains(ComplianceReceiptOutcome.Recorded, outcomes);
        Assert.Contains(
            ComplianceReceiptOutcome.EquivalentDuplicate,
            outcomes);
        await using var verificationContext =
            new ComplianceReceiptDbContext(options);
        Assert.Equal(1, await verificationContext
            .Set<ComplianceReceiptRecord>()
            .CountAsync());
    }

    private DbContextOptions<ComplianceReceiptDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;
    }

    private static ComplianceReceiptCandidate CreateCandidate(
        byte[] serializedEvent,
        DateTimeOffset receivedAtUtc)
    {
        return new ComplianceReceiptCandidate(
            EventId,
            "VendorRegistered",
            eventVersion: 1,
            receivedAtUtc,
            serializedEvent);
    }

    private static async Task<ComplianceReceiptOutcome> RecordAsync(
        DbContextOptions<ComplianceReceiptDbContext> options,
        byte[] serializedEvent,
        DateTimeOffset receivedAtUtc)
    {
        await using var context = new ComplianceReceiptDbContext(options);
        var store = new PostgreSqlComplianceReceiptStore(context);

        return await store.RecordAsync(
            CreateCandidate(serializedEvent, receivedAtUtc));
    }

    private static async Task ResetSchemaAsync(
        DbContextOptions<ComplianceReceiptDbContext> options)
    {
        await using var context = new ComplianceReceiptDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE compliance_vendor_registered_receipts;
            """);
    }
}
