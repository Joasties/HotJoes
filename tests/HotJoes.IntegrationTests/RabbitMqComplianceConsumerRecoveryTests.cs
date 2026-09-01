using System.Security.Cryptography;
using System.Text;
using HotJoes.Infrastructure.ComplianceConsumer;
using HotJoes.Infrastructure.VendorRelay;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(ComplianceConsumerIntegrationCollection.Name)]
public sealed class RabbitMqComplianceConsumerRecoveryTests
{
    private static readonly DateTimeOffset ReceivedAtUtc = new(
        2026,
        8,
        28,
        21,
        0,
        0,
        TimeSpan.Zero);

    private readonly RabbitMqFixture _rabbitMq;
    private readonly CompliancePostgreSqlFixture _postgreSql;

    public RabbitMqComplianceConsumerRecoveryTests(
        RabbitMqFixture rabbitMq,
        CompliancePostgreSqlFixture postgreSql)
    {
        _rabbitMq = rabbitMq;
        _postgreSql = postgreSql;
    }

    [Fact]
    public async Task ReceiptFailure_RetriesToInjectedLimitThenDeadLettersExactMessage()
    {
        RecoveryTopology topology = CreateTopology();
        Guid eventId = Guid.NewGuid();
        byte[] serializedEvent = CreateSerializedEvent(eventId);
        await PublishAsync(topology, eventId, serializedEvent);

        await using RabbitMqComplianceRecoveryPublisher recoveryPublisher =
            await RabbitMqComplianceRecoveryPublisher.CreateAsync(
                topology.RecoveryOptions);
        var recovery = new ComplianceDeliveryRecoveryHandler(
            new ComplianceConsumerRetryPolicy(
                maximumAutomaticAttempts: 2,
                retryDelay: topology.RecoveryOptions.RetryDelay),
            new ComplianceRecoveryDispatcher(recoveryPublisher));
        var processor = new ComplianceDeliveryProcessor(
            new FailingReceiptStore());

        await using (RabbitMqComplianceConsumer consumer =
            await RabbitMqComplianceConsumer.CreateAsync(
                topology.ConsumerOptions,
                processor,
                recovery))
        {
            Assert.Equal(
                ComplianceConsumerRunOutcome.Retried,
                await WaitForNonEmptyRunAsync(consumer, ReceivedAtUtc));
            Assert.Equal(
                ComplianceConsumerRunOutcome.DeadLettered,
                await WaitForNonEmptyRunAsync(
                    consumer,
                    ReceivedAtUtc.AddSeconds(1)));
        }

        BasicGetResult deadLetter = await GetMessageAsync(
            topology.RecoveryOptions.DeadLetterQueueName);
        AssertRecoveryMessage(
            deadLetter,
            eventId,
            serializedEvent,
            automaticAttempt: 2,
            failureCategory: "receiptUnavailable");
        Assert.Null(await TryGetMessageAsync(topology.ConsumerOptions.QueueName));
    }

    [Fact]
    public async Task InvalidContract_DeadLettersImmediatelyWithoutReceipt()
    {
        DbContextOptions<ComplianceReceiptDbContext> databaseOptions =
            CreateDatabaseOptions();
        await ResetReceiptsAsync(databaseOptions);
        RecoveryTopology topology = CreateTopology();
        Guid eventId = Guid.NewGuid();
        byte[] invalidEvent = "{\"eventType\":\"VendorRegistered\"}"u8
            .ToArray();
        await PublishAsync(topology, eventId, invalidEvent);

        await using RabbitMqComplianceRecoveryPublisher recoveryPublisher =
            await RabbitMqComplianceRecoveryPublisher.CreateAsync(
                topology.RecoveryOptions);
        await using RabbitMqComplianceConsumer consumer =
            await CreatePostgreSqlConsumerAsync(
                topology,
                databaseOptions,
                recoveryPublisher,
                maximumAutomaticAttempts: 3);

        Assert.Equal(
            ComplianceConsumerRunOutcome.DeadLettered,
            await WaitForNonEmptyRunAsync(consumer, ReceivedAtUtc));

        BasicGetResult deadLetter = await GetMessageAsync(
            topology.RecoveryOptions.DeadLetterQueueName);
        AssertRecoveryMessage(
            deadLetter,
            eventId,
            invalidEvent,
            automaticAttempt: 1,
            failureCategory: "invalidContract");
        await using var verificationContext =
            new ComplianceReceiptDbContext(databaseOptions);
        Assert.Equal(
            0,
            await verificationContext
                .Set<ComplianceReceiptRecord>()
                .CountAsync());
    }

    [Fact]
    public async Task ConflictingBytes_PreserveOriginalReceiptAndDeadLetterConflict()
    {
        DbContextOptions<ComplianceReceiptDbContext> databaseOptions =
            CreateDatabaseOptions();
        await ResetReceiptsAsync(databaseOptions);
        RecoveryTopology topology = CreateTopology();
        Guid eventId = Guid.NewGuid();
        byte[] originalEvent = CreateSerializedEvent(eventId);
        byte[] conflictingEvent = CreateSerializedEvent(
            eventId,
            foodRegistrationAuthority: "Different Council");
        await PublishAsync(
            topology,
            eventId,
            originalEvent,
            conflictingEvent);

        await using RabbitMqComplianceRecoveryPublisher recoveryPublisher =
            await RabbitMqComplianceRecoveryPublisher.CreateAsync(
                topology.RecoveryOptions);
        await using RabbitMqComplianceConsumer consumer =
            await CreatePostgreSqlConsumerAsync(
                topology,
                databaseOptions,
                recoveryPublisher,
                maximumAutomaticAttempts: 3);

        Assert.Equal(
            ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
            await WaitForNonEmptyRunAsync(consumer, ReceivedAtUtc));
        Assert.Equal(
            ComplianceConsumerRunOutcome.DeadLettered,
            await WaitForNonEmptyRunAsync(
                consumer,
                ReceivedAtUtc.AddSeconds(1)));

        BasicGetResult deadLetter = await GetMessageAsync(
            topology.RecoveryOptions.DeadLetterQueueName);
        AssertRecoveryMessage(
            deadLetter,
            eventId,
            conflictingEvent,
            automaticAttempt: 1,
            failureCategory: "conflictingBytes");

        await using var verificationContext =
            new ComplianceReceiptDbContext(databaseOptions);
        ComplianceReceiptRecord receipt = Assert.Single(
            await verificationContext
                .Set<ComplianceReceiptRecord>()
                .ToListAsync());
        Assert.Equal(eventId, receipt.EventId);
        Assert.Equal(
            SHA256.HashData(originalEvent),
            receipt.SerializedEventSha256);
    }

    private async Task<RabbitMqComplianceConsumer>
        CreatePostgreSqlConsumerAsync(
            RecoveryTopology topology,
            DbContextOptions<ComplianceReceiptDbContext> databaseOptions,
            RabbitMqComplianceRecoveryPublisher recoveryPublisher,
            int maximumAutomaticAttempts)
    {
        var context = new ComplianceReceiptDbContext(databaseOptions);
        var processor = new ComplianceDeliveryProcessor(
            new PostgreSqlComplianceReceiptStore(context));
        var recovery = new ComplianceDeliveryRecoveryHandler(
            new ComplianceConsumerRetryPolicy(
                maximumAutomaticAttempts,
                topology.RecoveryOptions.RetryDelay),
            new ComplianceRecoveryDispatcher(recoveryPublisher));

        try
        {
            return await RabbitMqComplianceConsumer.CreateAsync(
                topology.ConsumerOptions,
                processor,
                recovery,
                context);
        }
        catch
        {
            await context.DisposeAsync();
            throw;
        }
    }

    private RecoveryTopology CreateTopology()
    {
        string suffix = Guid.NewGuid().ToString("N");
        string exchangeName = $"hotjoes.vendor.registered.{suffix}";
        string routingKey = $"vendor.registered.{suffix}";

        return new RecoveryTopology(
            new RabbitMqConsumerOptions(
                _rabbitMq.ConnectionString,
                exchangeName,
                ExchangeType.Direct,
                $"hotjoes.compliance.vendor-registered.{suffix}",
                routingKey),
            new RabbitMqRecoveryOptions(
                _rabbitMq.ConnectionString,
                exchangeName,
                routingKey,
                $"hotjoes.compliance.retry.{suffix}",
                $"hotjoes.compliance.retry.{suffix}",
                $"compliance.retry.{suffix}",
                $"hotjoes.compliance.dead-letter.{suffix}",
                $"hotjoes.compliance.dead-letter.{suffix}",
                $"compliance.dead-letter.{suffix}",
                TimeSpan.FromMilliseconds(250)));
    }

    private async Task PublishAsync(
        RecoveryTopology topology,
        Guid eventId,
        byte[] firstEvent,
        byte[]? secondEvent = null)
    {
        var publisherOptions = new RabbitMqPublisherOptions(
            _rabbitMq.ConnectionString,
            topology.ConsumerOptions.ExchangeName,
            topology.ConsumerOptions.ExchangeType,
            topology.ConsumerOptions.QueueName,
            topology.ConsumerOptions.RoutingKey);
        await using RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions);
        await publisher.PublishAsync(
            new OutboxPublication(eventId, 1, firstEvent));

        if (secondEvent is not null)
        {
            await publisher.PublishAsync(
                new OutboxPublication(eventId, 1, secondEvent));
        }
    }

    private async Task<BasicGetResult> GetMessageAsync(string queueName)
    {
        using var timeout = new CancellationTokenSource(
            TimeSpan.FromSeconds(10));

        while (true)
        {
            BasicGetResult? message = await TryGetMessageAsync(
                queueName,
                timeout.Token);

            if (message is not null)
            {
                return message;
            }

            await Task.Delay(50, timeout.Token);
        }
    }

    private async Task<BasicGetResult?> TryGetMessageAsync(
        string queueName,
        CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMq.ConnectionString),
            AutomaticRecoveryEnabled = false
        };
        await using IConnection connection =
            await factory.CreateConnectionAsync(cancellationToken);
        await using IChannel channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);
        return await channel.BasicGetAsync(
            queueName,
            autoAck: true,
            cancellationToken);
    }

    private static async Task<ComplianceConsumerRunOutcome>
        WaitForNonEmptyRunAsync(
            RabbitMqComplianceConsumer consumer,
            DateTimeOffset receivedAtUtc)
    {
        using var timeout = new CancellationTokenSource(
            TimeSpan.FromSeconds(10));

        while (true)
        {
            ComplianceConsumerRunOutcome outcome = await consumer.RunOnceAsync(
                receivedAtUtc,
                timeout.Token);

            if (outcome != ComplianceConsumerRunOutcome.NoDelivery)
            {
                return outcome;
            }

            await Task.Delay(50, timeout.Token);
        }
    }

    private DbContextOptions<ComplianceReceiptDbContext>
        CreateDatabaseOptions()
    {
        return new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
            .UseNpgsql(_postgreSql.ConnectionString)
            .Options;
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

    private static byte[] CreateSerializedEvent(
        Guid eventId,
        string foodRegistrationAuthority =
            "Mole Valley District Council")
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
                "foodRegistrationAuthority": "{{foodRegistrationAuthority}}",
                "primaryTradingAuthority": null
              }
            }
            """);
    }

    private static void AssertRecoveryMessage(
        BasicGetResult message,
        Guid expectedEventId,
        byte[] expectedBytes,
        int automaticAttempt,
        string failureCategory)
    {
        Assert.Equal(expectedBytes, message.Body.ToArray());
        Assert.Equal(
            expectedEventId.ToString("D"),
            message.BasicProperties.MessageId);
        Assert.True(message.BasicProperties.Persistent);
        Assert.NotNull(message.BasicProperties.Headers);
        Assert.Equal(
            automaticAttempt,
            Convert.ToInt32(
                message.BasicProperties.Headers[
                    "x-hotjoes-automatic-attempt"]));
        Assert.Equal(
            failureCategory,
            Encoding.UTF8.GetString(
                Assert.IsType<byte[]>(
                    message.BasicProperties.Headers[
                        "x-hotjoes-failure-category"])));
        Assert.Equal(
            1,
            Convert.ToInt32(
                message.BasicProperties.Headers[
                    "x-hotjoes-event-version"]));
    }

    private sealed record RecoveryTopology(
        RabbitMqConsumerOptions ConsumerOptions,
        RabbitMqRecoveryOptions RecoveryOptions);

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
