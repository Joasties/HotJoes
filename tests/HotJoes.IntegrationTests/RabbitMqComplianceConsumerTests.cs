using System.Text;
using HotJoes.Infrastructure.ComplianceConsumer;
using HotJoes.Infrastructure.VendorRelay;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(ComplianceConsumerIntegrationCollection.Name)]
public sealed class RabbitMqComplianceConsumerTests
{
    private static readonly DateTimeOffset ReceivedAtUtc = new(
        2026,
        8,
        28,
        20,
        0,
        0,
        TimeSpan.Zero);

    private readonly RabbitMqFixture _rabbitMq;
    private readonly CompliancePostgreSqlFixture _postgreSql;

    public RabbitMqComplianceConsumerTests(
        RabbitMqFixture rabbitMq,
        CompliancePostgreSqlFixture postgreSql)
    {
        _rabbitMq = rabbitMq;
        _postgreSql = postgreSql;
    }

    [Fact]
    public async Task EquivalentDuplicateDeliveries_CreateOneReceiptAndAreBothAcknowledged()
    {
        DbContextOptions<ComplianceReceiptDbContext> databaseOptions =
            CreateDatabaseOptions();
        await ResetReceiptsAsync(databaseOptions);
        RabbitMqConsumerOptions brokerOptions = CreateBrokerOptions();
        Guid eventId = Guid.NewGuid();
        byte[] serializedEvent = CreateSerializedEvent(eventId);
        await PublishAsync(
            brokerOptions,
            eventId,
            serializedEvent,
            serializedEvent);

        await using (RabbitMqComplianceConsumer consumer =
            await CreateConsumerAsync(brokerOptions, databaseOptions))
        {
            Assert.Equal(
                ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
                await consumer.RunOnceAsync(ReceivedAtUtc));
            Assert.Equal(
                ComplianceConsumerRunOutcome
                    .AcknowledgedEquivalentDuplicate,
                await consumer.RunOnceAsync(ReceivedAtUtc.AddSeconds(1)));
            Assert.Equal(
                ComplianceConsumerRunOutcome.NoDelivery,
                await consumer.RunOnceAsync(ReceivedAtUtc.AddSeconds(2)));
        }

        await using var verificationContext =
            new ComplianceReceiptDbContext(databaseOptions);
        ComplianceReceiptRecord receipt = Assert.Single(
            await verificationContext
                .Set<ComplianceReceiptRecord>()
                .ToListAsync());
        Assert.Equal(eventId, receipt.EventId);
    }

    [Fact]
    public async Task ReorderedIndependentDeliveries_CreateIndependentReceiptsWithoutSequenceAssumption()
    {
        DbContextOptions<ComplianceReceiptDbContext> databaseOptions =
            CreateDatabaseOptions();
        await ResetReceiptsAsync(databaseOptions);
        RabbitMqConsumerOptions brokerOptions = CreateBrokerOptions();
        Guid earlierEventId = Guid.NewGuid();
        Guid laterEventId = Guid.NewGuid();
        byte[] earlierEvent = CreateSerializedEvent(earlierEventId);
        byte[] laterEvent = CreateSerializedEvent(laterEventId);

        await PublishAsync(
            brokerOptions,
            laterEventId,
            laterEvent,
            earlierEvent,
            earlierEventId);

        await using (RabbitMqComplianceConsumer consumer =
            await CreateConsumerAsync(brokerOptions, databaseOptions))
        {
            Assert.Equal(
                ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
                await consumer.RunOnceAsync(ReceivedAtUtc));
            Assert.Equal(
                ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
                await consumer.RunOnceAsync(ReceivedAtUtc.AddSeconds(1)));
            Assert.Equal(
                ComplianceConsumerRunOutcome.NoDelivery,
                await consumer.RunOnceAsync(ReceivedAtUtc.AddSeconds(2)));
        }

        await using var verificationContext =
            new ComplianceReceiptDbContext(databaseOptions);
        Guid[] receivedEventIds = await verificationContext
            .Set<ComplianceReceiptRecord>()
            .Select(receipt => receipt.EventId)
            .OrderBy(eventId => eventId)
            .ToArrayAsync();
        Assert.Equal(
            new[] { earlierEventId, laterEventId }.OrderBy(id => id),
            receivedEventIds);
    }

    [Fact]
    public async Task EquivalentDuplicateAfterConsumerRestart_UsesOriginalDurableReceipt()
    {
        DbContextOptions<ComplianceReceiptDbContext> databaseOptions =
            CreateDatabaseOptions();
        await ResetReceiptsAsync(databaseOptions);
        RabbitMqConsumerOptions brokerOptions = CreateBrokerOptions();
        Guid eventId = Guid.NewGuid();
        byte[] serializedEvent = CreateSerializedEvent(eventId);
        await PublishAsync(brokerOptions, eventId, serializedEvent);

        await using (RabbitMqComplianceConsumer firstConsumer =
            await CreateConsumerAsync(brokerOptions, databaseOptions))
        {
            Assert.Equal(
                ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
                await firstConsumer.RunOnceAsync(ReceivedAtUtc));
        }

        await PublishAsync(brokerOptions, eventId, serializedEvent);

        await using (RabbitMqComplianceConsumer restartedConsumer =
            await CreateConsumerAsync(brokerOptions, databaseOptions))
        {
            Assert.Equal(
                ComplianceConsumerRunOutcome
                    .AcknowledgedEquivalentDuplicate,
                await restartedConsumer.RunOnceAsync(
                    ReceivedAtUtc.AddMinutes(1)));
            Assert.Equal(
                ComplianceConsumerRunOutcome.NoDelivery,
                await restartedConsumer.RunOnceAsync(
                    ReceivedAtUtc.AddMinutes(1).AddSeconds(1)));
        }

        await using var verificationContext =
            new ComplianceReceiptDbContext(databaseOptions);
        ComplianceReceiptRecord receipt = Assert.Single(
            await verificationContext
                .Set<ComplianceReceiptRecord>()
                .ToListAsync());
        Assert.Equal(ReceivedAtUtc, receipt.ReceivedAtUtc);
    }

    [Fact]
    public async Task ReceiptFailure_LeavesDeliveryAvailableAfterConsumerRestart()
    {
        DbContextOptions<ComplianceReceiptDbContext> databaseOptions =
            CreateDatabaseOptions();
        await ResetReceiptsAsync(databaseOptions);
        RabbitMqConsumerOptions brokerOptions = CreateBrokerOptions();
        Guid eventId = Guid.NewGuid();
        byte[] serializedEvent = CreateSerializedEvent(eventId);
        await PublishAsync(brokerOptions, eventId, serializedEvent);

        var failingProcessor = new ComplianceDeliveryProcessor(
            new FailingReceiptStore());

        await using (RabbitMqComplianceConsumer failingConsumer =
            await RabbitMqComplianceConsumer.CreateAsync(
                brokerOptions,
                failingProcessor))
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                failingConsumer.RunOnceAsync(ReceivedAtUtc));
        }

        await using (RabbitMqComplianceConsumer restartedConsumer =
            await CreateConsumerAsync(brokerOptions, databaseOptions))
        {
            Assert.Equal(
                ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
                await restartedConsumer.RunOnceAsync(
                    ReceivedAtUtc.AddSeconds(1)));
            Assert.Equal(
                ComplianceConsumerRunOutcome.NoDelivery,
                await restartedConsumer.RunOnceAsync(
                    ReceivedAtUtc.AddSeconds(2)));
        }

        await using var verificationContext =
            new ComplianceReceiptDbContext(databaseOptions);
        ComplianceReceiptRecord receipt = Assert.Single(
            await verificationContext
                .Set<ComplianceReceiptRecord>()
                .ToListAsync());
        Assert.Equal(eventId, receipt.EventId);
    }

    private DbContextOptions<ComplianceReceiptDbContext>
        CreateDatabaseOptions()
    {
        return new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
            .UseNpgsql(_postgreSql.ConnectionString)
            .Options;
    }

    private RabbitMqConsumerOptions CreateBrokerOptions()
    {
        string suffix = Guid.NewGuid().ToString("N");
        return new RabbitMqConsumerOptions(
            _rabbitMq.ConnectionString,
            $"hotjoes.vendor.registered.{suffix}",
            ExchangeType.Direct,
            $"hotjoes.compliance.vendor-registered.{suffix}",
            $"vendor.registered.{suffix}");
    }

    private static async Task<RabbitMqComplianceConsumer>
        CreateConsumerAsync(
            RabbitMqConsumerOptions brokerOptions,
            DbContextOptions<ComplianceReceiptDbContext> databaseOptions)
    {
        var context = new ComplianceReceiptDbContext(databaseOptions);
        var store = new PostgreSqlComplianceReceiptStore(context);
        var processor = new ComplianceDeliveryProcessor(store);

        try
        {
            return await RabbitMqComplianceConsumer.CreateAsync(
                brokerOptions,
                processor,
                context);
        }
        catch
        {
            await context.DisposeAsync();
            throw;
        }
    }

    private static async Task PublishAsync(
        RabbitMqConsumerOptions options,
        Guid firstEventId,
        byte[] firstEvent,
        byte[]? secondEvent = null,
        Guid? secondEventId = null)
    {
        var publisherOptions = new RabbitMqPublisherOptions(
            options.ConnectionString,
            options.ExchangeName,
            options.ExchangeType,
            options.QueueName,
            options.RoutingKey);

        await using RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(
                publisherOptions);

        await publisher.PublishAsync(
            new OutboxPublication(firstEventId, 1, firstEvent));

        if (secondEvent is not null)
        {
            await publisher.PublishAsync(
                new OutboxPublication(
                    secondEventId ?? firstEventId,
                    1,
                    secondEvent));
        }
    }

    private static byte[] CreateSerializedEvent(Guid eventId)
    {
        return Encoding.UTF8.GetBytes($$"""
            {
              "eventId": "{{eventId:D}}",
              "eventType": "VendorRegistered",
              "eventVersion": 1,
              "occurredAt": "2026-08-28T18:00:00.0000000Z",
              "payload": {
                "vendorId": "4e512746-8714-4e31-8b29-e8a262dd54b2",
                "registeredAt": "2026-08-28T18:00:00.0000000Z",
                "vendorState": "pendingActivation",
                "tradingPreference": "online",
                "legalOperatorType": "soleTrader",
                "tradingCharacteristics": {
                  "tradingLocation": "restaurant",
                  "openingHours": {
                    "startTime": "09:00:00",
                    "endTime": "17:00:00"
                  },
                  "serviceIncludesHotFood": true,
                  "alcoholService": false
                },
                "businessAddress": {
                  "canonicalAddressId": "address-123",
                  "recipientOrOrganisationName": null,
                  "addressLine1": "2 High Street",
                  "addressLine2": null,
                  "addressLine3": null,
                  "postTown": "LEATHERHEAD",
                  "postcode": "KT22 7QS",
                  "county": null
                },
                "foodRegistrationAuthority": "Mole Valley District Council",
                "primaryTradingAuthority": null
              }
            }
            """);
    }

    private static async Task ResetReceiptsAsync(
        DbContextOptions<ComplianceReceiptDbContext> options)
    {
        await using var context = new ComplianceReceiptDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE compliance_vendor_registered_receipts;
            """);
    }

    private sealed class FailingReceiptStore : IComplianceReceiptStore
    {
        public Task<ComplianceReceiptOutcome> RecordAsync(
            ComplianceReceiptCandidate candidate,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Receipt persistence failed.");
        }
    }
}
