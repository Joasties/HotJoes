using System.Text;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using HotJoes.Infrastructure.VendorRelay;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(ReliablePublicationCollection.Name)]
public sealed class PostgreSqlRabbitMqOutboxRelayTests
{
    private static readonly DateTimeOffset RelayTime = new(
        2026,
        8,
        28,
        16,
        0,
        0,
        TimeSpan.Zero);

    private static readonly Guid FirstWorker = Guid.Parse(
        "f7f8b60d-b23d-46c2-9d4f-ce853c062414");

    private static readonly Guid RecoveryWorker = Guid.Parse(
        "83ec37b3-5792-4aa8-8663-3405c8a1df9f");

    private static readonly OutboxRelayRetryPolicy RetryPolicy = new(
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(4),
        automaticAttemptLimit: 3);

    private readonly PostgreSqlFixture _postgreSql;
    private readonly RabbitMqFixture _rabbitMq;

    public PostgreSqlRabbitMqOutboxRelayTests(
        PostgreSqlFixture postgreSql,
        RabbitMqFixture rabbitMq)
    {
        _postgreSql = postgreSql;
        _rabbitMq = rabbitMq;
    }

    [Fact]
    public async Task RunOnceAsync_ConfirmedPublication_CompletesOnlyAfterExactMessageIsAccepted()
    {
        DbContextOptions<VendorRegistrationDbContext> databaseOptions =
            CreateDatabaseOptions();
        SeededPublication seeded = await ResetSchemaAndSeedAsync(
            databaseOptions);
        await ChangePersistedVendorAfterEventWasStagedAsync(
            databaseOptions,
            seeded.VendorId);
        RabbitMqPublisherOptions publisherOptions = CreatePublisherOptions();

        await using var publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions);
        await using var context = new VendorRegistrationDbContext(
            databaseOptions);
        var store = new PostgreSqlOutboxRelayStore(context);
        var runner = CreateRunner(store, publisher, FirstWorker);

        OutboxRelayBatchResult result = await runner.RunOnceAsync(RelayTime);

        Assert.Equal(1, result.ClaimedCount);
        Assert.Equal(1, result.PublishedCount);
        Assert.Equal(0, result.RetryScheduledCount);

        VendorRegistrationOutboxRecord record = await context
            .Set<VendorRegistrationOutboxRecord>()
            .SingleAsync(item => item.EventId == seeded.EventId);
        Assert.Equal(RelayTime, record.PublishedAtUtc);
        Assert.Null(record.ClaimedBy);
        Assert.Null(record.ClaimExpiresAtUtc);

        BasicGetResult message = await GetMessageAsync(
            publisherOptions.QueueName);
        Assert.Equal(seeded.SerializedEvent, message.Body.ToArray());
        Assert.Equal(
            seeded.EventId.ToString("D"),
            message.BasicProperties.MessageId);
        Assert.Empty(await store.ClaimEligibleAsync(
            RecoveryWorker,
            RelayTime.AddDays(1),
            TimeSpan.FromMinutes(1),
            batchSize: 1));
    }

    [Fact]
    public async Task RunOnceAsync_BrokerFailure_RetriesOriginalWorkAfterBrokerRestart()
    {
        DbContextOptions<VendorRegistrationDbContext> databaseOptions =
            CreateDatabaseOptions();
        SeededPublication seeded = await ResetSchemaAndSeedAsync(
            databaseOptions);
        RabbitMqPublisherOptions publisherOptions = CreatePublisherOptions();
        await using var unavailablePublisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions);

        await _rabbitMq.StopAsync();

        try
        {
            await using var failedContext = new VendorRegistrationDbContext(
                databaseOptions);
            var failedStore = new PostgreSqlOutboxRelayStore(failedContext);
            var failedRunner = CreateRunner(
                failedStore,
                unavailablePublisher,
                FirstWorker);

            OutboxRelayBatchResult failed = await failedRunner.RunOnceAsync(
                RelayTime);

            Assert.Equal(1, failed.ClaimedCount);
            Assert.Equal(0, failed.PublishedCount);
            Assert.Equal(1, failed.RetryScheduledCount);

            VendorRegistrationOutboxRecord record = await failedContext
                .Set<VendorRegistrationOutboxRecord>()
                .SingleAsync(item => item.EventId == seeded.EventId);
            Assert.Null(record.PublishedAtUtc);
            Assert.Equal(1, record.AttemptCount);
            Assert.Equal(RelayTime.AddSeconds(1), record.NextAttemptAtUtc);
        }
        finally
        {
            await _rabbitMq.StartAsync();
        }

        publisherOptions = RebindToCurrentBroker(publisherOptions);
        await using var recoveryPublisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions);
        await using var recoveryContext = new VendorRegistrationDbContext(
            databaseOptions);
        var recoveryStore = new PostgreSqlOutboxRelayStore(recoveryContext);
        var recoveryRunner = CreateRunner(
            recoveryStore,
            recoveryPublisher,
            RecoveryWorker);

        OutboxRelayBatchResult recovered = await recoveryRunner.RunOnceAsync(
            RelayTime.AddSeconds(1));

        Assert.Equal(1, recovered.PublishedCount);
        Assert.Equal(
            seeded.SerializedEvent,
            (await GetMessageAsync(publisherOptions.QueueName)).Body.ToArray());
        Assert.Equal(1, await recoveryContext
            .Set<VendorRegistrationRecord>()
            .CountAsync());
        Assert.Equal(1, await recoveryContext
            .Set<VendorRegistrationOutboxRecord>()
            .CountAsync());
    }

    [Fact]
    public async Task RunOnceAsync_CompletionFailsAfterConfirmation_LeaseRecoveryRepublishesSameMessage()
    {
        DbContextOptions<VendorRegistrationDbContext> databaseOptions =
            CreateDatabaseOptions();
        SeededPublication seeded = await ResetSchemaAndSeedAsync(
            databaseOptions);
        RabbitMqPublisherOptions publisherOptions = CreatePublisherOptions();

        await using var firstPublisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions);
        await using (var firstContext = new VendorRegistrationDbContext(
            databaseOptions))
        {
            var innerStore = new PostgreSqlOutboxRelayStore(firstContext);
            var failingStore = new CompletionFailingRelayStore(innerStore);
            var firstRunner = CreateRunner(
                failingStore,
                firstPublisher,
                FirstWorker,
                leaseDuration: TimeSpan.FromSeconds(30));

            await Assert.ThrowsAsync<SimulatedCompletionException>(() =>
                firstRunner.RunOnceAsync(RelayTime));
        }

        BasicGetResult firstDelivery = await GetMessageAsync(
            publisherOptions.QueueName);

        await using var recoveryPublisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions);
        await using var recoveryContext = new VendorRegistrationDbContext(
            databaseOptions);
        var recoveryStore = new PostgreSqlOutboxRelayStore(recoveryContext);
        var recoveryRunner = CreateRunner(
            recoveryStore,
            recoveryPublisher,
            RecoveryWorker,
            leaseDuration: TimeSpan.FromSeconds(30));

        OutboxRelayBatchResult recovered = await recoveryRunner.RunOnceAsync(
            RelayTime.AddSeconds(30));
        BasicGetResult secondDelivery = await GetMessageAsync(
            publisherOptions.QueueName);

        Assert.Equal(1, recovered.PublishedCount);
        Assert.Equal(seeded.SerializedEvent, firstDelivery.Body.ToArray());
        Assert.Equal(seeded.SerializedEvent, secondDelivery.Body.ToArray());
        Assert.Equal(
            firstDelivery.BasicProperties.MessageId,
            secondDelivery.BasicProperties.MessageId);
        Assert.Equal(seeded.EventId.ToString("D"),
            secondDelivery.BasicProperties.MessageId);
        Assert.Equal(1, await recoveryContext
            .Set<VendorRegistrationRecord>()
            .CountAsync());
        Assert.Equal(1, await recoveryContext
            .Set<VendorRegistrationOutboxRecord>()
            .CountAsync());
        Assert.NotNull((await recoveryContext
            .Set<VendorRegistrationOutboxRecord>()
            .SingleAsync()).PublishedAtUtc);
    }

    private DbContextOptions<VendorRegistrationDbContext>
        CreateDatabaseOptions()
    {
        return new DbContextOptionsBuilder<VendorRegistrationDbContext>()
            .UseNpgsql(_postgreSql.ConnectionString)
            .Options;
    }

    private RabbitMqPublisherOptions CreatePublisherOptions()
    {
        string suffix = Guid.NewGuid().ToString("N");

        return new RabbitMqPublisherOptions(
            _rabbitMq.ConnectionString,
            $"hotjoes.vendor.registered.{suffix}",
            ExchangeType.Direct,
            $"hotjoes.compliance.vendor-registered.{suffix}",
            $"vendor.registered.{suffix}");
    }

    private RabbitMqPublisherOptions RebindToCurrentBroker(
        RabbitMqPublisherOptions previous)
    {
        return new RabbitMqPublisherOptions(
            _rabbitMq.ConnectionString,
            previous.ExchangeName,
            previous.ExchangeType,
            previous.QueueName,
            previous.RoutingKey);
    }

    private static VendorOutboxRelayRunner CreateRunner(
        IOutboxRelayClaimStore store,
        IOutboxEventPublisher publisher,
        Guid workerId,
        TimeSpan? leaseDuration = null)
    {
        return new VendorOutboxRelayRunner(
            store,
            publisher,
            workerId,
            leaseDuration ?? TimeSpan.FromMinutes(1),
            batchSize: 10,
            RetryPolicy);
    }

    private async Task<BasicGetResult> GetMessageAsync(string queueName)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMq.ConnectionString),
            AutomaticRecoveryEnabled = false
        };
        await using IConnection connection =
            await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        return Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(queueName, autoAck: true));
    }

    private static async Task ChangePersistedVendorAfterEventWasStagedAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        Guid vendorId)
    {
        await using var context = new VendorRegistrationDbContext(options);
        await context.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE vendor_registrations
            SET business_description = {"Changed after event staging"}
            WHERE vendor_id = {vendorId}
            """);
    }

    private static async Task<SeededPublication> ResetSchemaAndSeedAsync(
        DbContextOptions<VendorRegistrationDbContext> options)
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

        Guid vendorId = Guid.Parse(
            "19a412de-d83b-4838-801d-6ccce1abe336");
        Guid eventId = Guid.Parse(
            "7b85cf37-4797-438c-b4de-e424c969e28e");
        byte[] serializedEvent = Encoding.UTF8.GetBytes("""
            { "eventId": "7b85cf37-4797-438c-b4de-e424c969e28e", "eventType": "VendorRegistered", "eventVersion": 1, "payload": { "vendorId": "19a412de-d83b-4838-801d-6ccce1abe336", "businessDescription": "Original staged value" } }
            """);

        context.Set<VendorRegistrationRecord>().Add(
            VendorRegistrationRecordMapper.ToRecord(CreateVendor(vendorId)));
        context.Set<VendorRegistrationOutboxRecord>().Add(
            new VendorRegistrationOutboxRecord
            {
                EventId = eventId,
                VendorId = vendorId,
                EventVersion = 1,
                SerializedEvent = serializedEvent
            });
        await context.SaveChangesAsync();

        return new SeededPublication(
            vendorId,
            eventId,
            serializedEvent);
    }

    private static VendorAggregate CreateVendor(Guid vendorId)
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName("Relay Recovery Operator"),
            new VendorName("Relay Recovery Vendor"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                "Relay Recovery Contact",
                new EmailAddress("relay-recovery@example.test"),
                new TelephoneNumber("+442079460123")),
            new CanonicalAddressId("relay-recovery-address"),
            new BusinessAddressSnapshot(
                "1 Recovery Street",
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
            businessDescription: "Original persisted value",
            RelayTime);
    }

    private sealed record SeededPublication(
        Guid VendorId,
        Guid EventId,
        byte[] SerializedEvent);

    private sealed class CompletionFailingRelayStore
        : IOutboxRelayClaimStore
    {
        private readonly PostgreSqlOutboxRelayStore _inner;

        public CompletionFailingRelayStore(
            PostgreSqlOutboxRelayStore inner)
        {
            _inner = inner;
        }

        public Task<IReadOnlyList<OutboxRelayClaim>> ClaimEligibleAsync(
            Guid workerId,
            DateTimeOffset claimedAtUtc,
            TimeSpan leaseDuration,
            int batchSize,
            CancellationToken cancellationToken = default)
        {
            return _inner.ClaimEligibleAsync(
                workerId,
                claimedAtUtc,
                leaseDuration,
                batchSize,
                cancellationToken);
        }

        public Task MarkPublishedAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset publishedAtUtc,
            CancellationToken cancellationToken = default)
        {
            throw new SimulatedCompletionException();
        }

        public Task RecordFailureAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset failedAtUtc,
            OutboxRelayFailureCategory failureCategory,
            OutboxRelayRetryPolicy retryPolicy,
            CancellationToken cancellationToken = default)
        {
            return _inner.RecordFailureAsync(
                eventId,
                workerId,
                failedAtUtc,
                failureCategory,
                retryPolicy,
                cancellationToken);
        }
    }

    private sealed class SimulatedCompletionException : Exception
    {
    }
}
