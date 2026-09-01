using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.ComplianceConsumer;
using HotJoes.Infrastructure.Persistence;
using HotJoes.Infrastructure.VendorRelay;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(TracePropagationIntegrationCollection.Name)]
public sealed class PostgreSqlRabbitMqComplianceTraceTests
{
    private const string TraceParent =
        "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
    private const string TraceState = "vendor=hotjoes";

    private static readonly DateTimeOffset ObservedAtUtc = new(
        2026,
        8,
        29,
        16,
        0,
        0,
        TimeSpan.Zero);

    private readonly PostgreSqlFixture _vendorPostgreSql;
    private readonly CompliancePostgreSqlFixture _compliancePostgreSql;
    private readonly RabbitMqFixture _rabbitMq;

    public PostgreSqlRabbitMqComplianceTraceTests(
        PostgreSqlFixture vendorPostgreSql,
        CompliancePostgreSqlFixture compliancePostgreSql,
        RabbitMqFixture rabbitMq)
    {
        _vendorPostgreSql = vendorPostgreSql;
        _compliancePostgreSql = compliancePostgreSql;
        _rabbitMq = rabbitMq;
    }

    [Fact]
    public async Task PersistedTraceContext_ContinuesThroughRelayAndConsumerReceipt()
    {
        DbContextOptions<VendorRegistrationDbContext> vendorOptions =
            CreateVendorOptions();
        DbContextOptions<ComplianceReceiptDbContext> complianceOptions =
            CreateComplianceOptions();
        await ResetDatabasesAsync(vendorOptions, complianceOptions);
        Guid eventId = Guid.NewGuid();
        byte[] serializedEvent = CreateSerializedEvent(eventId);
        await SeedOutboxAsync(
            vendorOptions,
            eventId,
            serializedEvent,
            TraceParent,
            TraceState);
        (RabbitMqPublisherOptions publisherOptions,
            RabbitMqConsumerOptions consumerOptions) = CreateBrokerOptions();
        using var activityObserver = new TraceActivityObserver();

        await using (RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions))
        await using (var vendorContext =
            new VendorRegistrationDbContext(vendorOptions))
        {
            var store = new PostgreSqlOutboxRelayStore(vendorContext);
            var runner = new VendorOutboxRelayRunner(
                store,
                publisher,
                Guid.NewGuid(),
                TimeSpan.FromMinutes(1),
                batchSize: 1,
                new OutboxRelayRetryPolicy(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(4),
                    automaticAttemptLimit: 3));

            OutboxRelayBatchResult relayResult = await runner.RunOnceAsync(
                ObservedAtUtc);
            Assert.Equal(1, relayResult.PublishedCount);
        }

        await using var complianceContext =
            new ComplianceReceiptDbContext(complianceOptions);
        var observingStore = new ActivityObservingReceiptStore(
            new PostgreSqlComplianceReceiptStore(complianceContext));
        var processor = new ComplianceDeliveryProcessor(observingStore);
        await using RabbitMqComplianceConsumer consumer =
            await RabbitMqComplianceConsumer.CreateAsync(
                consumerOptions,
                processor);

        ComplianceConsumerRunOutcome outcome = await consumer.RunOnceAsync(
            ObservedAtUtc.AddSeconds(1));

        Assert.Equal(
            ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
            outcome);
        ActivityContext.TryParse(
            TraceParent,
            TraceState,
            isRemote: true,
            out ActivityContext originatingContext);
        Assert.Equal(ActivityKind.Consumer, observingStore.ActivityKind);
        Assert.Equal(originatingContext.TraceId, observingStore.TraceId);
        Assert.NotNull(activityObserver.ProducerSpanId);
        Assert.Equal(
            activityObserver.ProducerSpanId,
            observingStore.ParentSpanId);
        Assert.NotEqual(
            originatingContext.SpanId,
            observingStore.ParentSpanId);
        Assert.Equal(TraceState, observingStore.TraceState);

        ComplianceReceiptRecord receipt = Assert.Single(
            await complianceContext
                .Set<ComplianceReceiptRecord>()
                .AsNoTracking()
                .ToListAsync());
        Assert.Equal(eventId, receipt.EventId);
        Assert.Equal(
            SHA256.HashData(serializedEvent),
            receipt.SerializedEventSha256);
    }

    [Fact]
    public async Task MissingAndMalformedTraceHeaders_CreateReceiptsWithoutRejectingEvents()
    {
        DbContextOptions<ComplianceReceiptDbContext> complianceOptions =
            CreateComplianceOptions();
        await ResetComplianceDatabaseAsync(complianceOptions);
        (RabbitMqPublisherOptions publisherOptions,
            RabbitMqConsumerOptions consumerOptions) = CreateBrokerOptions();
        Guid missingContextEventId = Guid.NewGuid();
        Guid malformedContextEventId = Guid.NewGuid();
        byte[] missingContextEvent = CreateSerializedEvent(
            missingContextEventId);
        byte[] malformedContextEvent = CreateSerializedEvent(
            malformedContextEventId);

        await using (RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(publisherOptions))
        {
            await publisher.PublishAsync(new OutboxPublication(
                missingContextEventId,
                1,
                missingContextEvent));
        }
        await PublishMalformedContextAsync(
            publisherOptions,
            malformedContextEventId,
            malformedContextEvent);

        using var activityObserver = new TraceActivityObserver();
        await using var complianceContext =
            new ComplianceReceiptDbContext(complianceOptions);
        var observingStore = new ActivityObservingReceiptStore(
            new PostgreSqlComplianceReceiptStore(complianceContext));
        var processor = new ComplianceDeliveryProcessor(observingStore);
        await using RabbitMqComplianceConsumer consumer =
            await RabbitMqComplianceConsumer.CreateAsync(
                consumerOptions,
                processor);

        Assert.Equal(
            ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
            await consumer.RunOnceAsync(ObservedAtUtc));
        Assert.Equal(
            ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
            await consumer.RunOnceAsync(ObservedAtUtc.AddSeconds(1)));
        Assert.Equal(2, observingStore.ObservedContexts.Count);
        Assert.All(
            observingStore.ObservedContexts,
            context => Assert.NotEqual(default, context.TraceId));
        Assert.Equal(
            2,
            await complianceContext.Set<ComplianceReceiptRecord>().CountAsync());
    }

    private DbContextOptions<VendorRegistrationDbContext>
        CreateVendorOptions()
    {
        return new DbContextOptionsBuilder<VendorRegistrationDbContext>()
            .UseNpgsql(_vendorPostgreSql.ConnectionString)
            .Options;
    }

    private DbContextOptions<ComplianceReceiptDbContext>
        CreateComplianceOptions()
    {
        return new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
            .UseNpgsql(_compliancePostgreSql.ConnectionString)
            .Options;
    }

    private (RabbitMqPublisherOptions Publisher, RabbitMqConsumerOptions Consumer)
        CreateBrokerOptions()
    {
        string suffix = Guid.NewGuid().ToString("N");
        string exchange = $"hotjoes.vendor.registered.{suffix}";
        string queue = $"hotjoes.compliance.vendor-registered.{suffix}";
        string routingKey = $"vendor.registered.{suffix}";

        return (
            new RabbitMqPublisherOptions(
                _rabbitMq.ConnectionString,
                exchange,
                ExchangeType.Direct,
                queue,
                routingKey),
            new RabbitMqConsumerOptions(
                _rabbitMq.ConnectionString,
                exchange,
                ExchangeType.Direct,
                queue,
                routingKey));
    }

    private static async Task ResetDatabasesAsync(
        DbContextOptions<VendorRegistrationDbContext> vendorOptions,
        DbContextOptions<ComplianceReceiptDbContext> complianceOptions)
    {
        await using var vendorContext =
            new VendorRegistrationDbContext(vendorOptions);
        await vendorContext.Database.EnsureCreatedAsync();
        await vendorContext.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                vendor_registration_outbox,
                vendor_registration_outcomes,
                vendor_registrations
            RESTART IDENTITY CASCADE;
            """);
        await ResetComplianceDatabaseAsync(complianceOptions);
    }

    private static async Task ResetComplianceDatabaseAsync(
        DbContextOptions<ComplianceReceiptDbContext> options)
    {
        await using var context = new ComplianceReceiptDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE compliance_vendor_registered_receipts;");
    }

    private static async Task SeedOutboxAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        Guid eventId,
        byte[] serializedEvent,
        string? traceParent,
        string? traceState)
    {
        await using var context = new VendorRegistrationDbContext(options);
        Guid vendorId = Guid.NewGuid();
        context.Set<VendorRegistrationRecord>().Add(
            VendorRegistrationRecordMapper.ToRecord(CreateVendor(vendorId)));
        context.Set<VendorRegistrationOutboxRecord>().Add(
            new VendorRegistrationOutboxRecord
            {
                EventId = eventId,
                VendorId = vendorId,
                EventVersion = 1,
                SerializedEvent = serializedEvent,
                TraceParent = traceParent,
                TraceState = traceState
            });
        await context.SaveChangesAsync();
    }

    private static async Task PublishMalformedContextAsync(
        RabbitMqPublisherOptions options,
        Guid eventId,
        byte[] serializedEvent)
    {
        var factory = new ConnectionFactory
        {
            Uri = options.ConnectionUri,
            AutomaticRecoveryEnabled = false
        };
        await using IConnection connection =
            await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            MessageId = eventId.ToString("D"),
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                ["traceparent"] = "not-a-w3c-trace-parent",
                ["tracestate"] = "invalid"
            }
        };

        await channel.BasicPublishAsync(
            options.ExchangeName,
            options.RoutingKey,
            mandatory: true,
            properties,
            serializedEvent);
    }

    private static byte[] CreateSerializedEvent(Guid eventId)
    {
        return Encoding.UTF8.GetBytes($$"""
            {
              "eventId": "{{eventId:D}}",
              "eventType": "VendorRegistered",
              "eventVersion": 1,
              "occurredAt": "2026-08-29T15:00:00.0000000Z",
              "payload": {
                "vendorId": "4e512746-8714-4e31-8b29-e8a262dd54b2",
                "registeredAt": "2026-08-29T15:00:00.0000000Z",
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

    private static VendorAggregate CreateVendor(Guid vendorId)
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName($"Trace Operator {vendorId:N}"),
            new VendorName($"Trace Vendor {vendorId:N}"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                "Trace Contact",
                new EmailAddress("trace@example.test"),
                new TelephoneNumber("+442079460123")),
            new CanonicalAddressId($"trace-address-{vendorId:N}"),
            new BusinessAddressSnapshot(
                "1 Trace Street",
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
                new OpeningHours(
                    new TimeOnly(9, 0),
                    new TimeOnly(17, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(vendorId),
            information,
            website: null,
            businessDescription: null,
            ObservedAtUtc);
    }

    private sealed class ActivityObservingReceiptStore
        : IComplianceReceiptStore
    {
        private readonly IComplianceReceiptStore _inner;

        public ActivityObservingReceiptStore(IComplianceReceiptStore inner)
        {
            _inner = inner;
        }

        public List<ActivityContext> ObservedContexts { get; } = [];

        public ActivityKind? ActivityKind { get; private set; }

        public ActivityTraceId? TraceId { get; private set; }

        public ActivitySpanId? ParentSpanId { get; private set; }

        public string? TraceState { get; private set; }

        public Task<ComplianceReceiptOutcome> RecordAsync(
            ComplianceReceiptCandidate candidate,
            CancellationToken cancellationToken = default)
        {
            Activity? activity = Activity.Current;
            Assert.NotNull(activity);
            ObservedContexts.Add(activity.Context);
            ActivityKind = activity.Kind;
            TraceId = activity.TraceId;
            ParentSpanId = activity.ParentSpanId;
            TraceState = activity.TraceStateString;
            return _inner.RecordAsync(candidate, cancellationToken);
        }
    }

    private sealed class TraceActivityObserver : IDisposable
    {
        private readonly ActivityListener _listener;

        public TraceActivityObserver()
        {
            _listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = static (
                    ref ActivityCreationOptions<ActivityContext> _) =>
                        ActivitySamplingResult.AllDataAndRecorded,
                SampleUsingParentId = static (
                    ref ActivityCreationOptions<string> _) =>
                        ActivitySamplingResult.AllDataAndRecorded,
                ActivityStarted = activity =>
                {
                    if (activity.Kind == ActivityKind.Producer)
                    {
                        ProducerSpanId = activity.SpanId;
                    }
                }
            };
            ActivitySource.AddActivityListener(_listener);
        }

        public ActivitySpanId? ProducerSpanId { get; private set; }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}
