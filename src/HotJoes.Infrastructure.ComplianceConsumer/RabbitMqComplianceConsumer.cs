using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class RabbitMqComplianceConsumer : IAsyncDisposable
{
    private const string TraceParentHeader = "traceparent";
    private const string TraceStateHeader = "tracestate";

    private static readonly ActivitySource ConsumerActivitySource = new(
        "HotJoes.Infrastructure.ComplianceConsumer");

    private const string AutomaticAttemptHeader =
        "x-hotjoes-automatic-attempt";
    private const string EventVersionHeader =
        "x-hotjoes-event-version";

    private readonly RabbitMqConsumerOptions _options;
    private readonly ComplianceDeliveryProcessor _processor;
    private readonly ComplianceDeliveryRecoveryHandler? _recovery;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IAsyncDisposable? _ownedResource;

    private RabbitMqComplianceConsumer(
        RabbitMqConsumerOptions options,
        ComplianceDeliveryProcessor processor,
        ComplianceDeliveryRecoveryHandler? recovery,
        IConnection connection,
        IChannel channel,
        IAsyncDisposable? ownedResource)
    {
        _options = options;
        _processor = processor;
        _recovery = recovery;
        _connection = connection;
        _channel = channel;
        _ownedResource = ownedResource;
    }

    public static Task<RabbitMqComplianceConsumer> CreateAsync(
        RabbitMqConsumerOptions options,
        ComplianceDeliveryProcessor processor,
        IAsyncDisposable? ownedResource = null,
        CancellationToken cancellationToken = default)
    {
        return CreateCoreAsync(
            options,
            processor,
            recovery: null,
            ownedResource,
            cancellationToken);
    }

    public static Task<RabbitMqComplianceConsumer> CreateAsync(
        RabbitMqConsumerOptions options,
        ComplianceDeliveryProcessor processor,
        ComplianceDeliveryRecoveryHandler recovery,
        IAsyncDisposable? ownedResource = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recovery);

        return CreateCoreAsync(
            options,
            processor,
            recovery,
            ownedResource,
            cancellationToken);
    }

    public async Task<ComplianceConsumerRunOutcome> RunOnceAsync(
        DateTimeOffset receivedAtUtc,
        CancellationToken cancellationToken = default)
    {
        BasicGetResult? delivery = await _channel.BasicGetAsync(
            _options.QueueName,
            autoAck: false,
            cancellationToken);

        if (delivery is null)
        {
            return ComplianceConsumerRunOutcome.NoDelivery;
        }

        using Activity? consumerActivity = StartConsumerActivity(
            delivery.BasicProperties.Headers);

        var acknowledgement = new RabbitMqDeliveryAcknowledgement(
            _channel,
            delivery.DeliveryTag);

        ComplianceDeliveryOutcome outcome;

        try
        {
            outcome = await _processor.ProcessAsync(
                delivery.Body,
                receivedAtUtc,
                acknowledgement,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch when (_recovery is not null)
        {
            return await RecoverAsync(
                delivery,
                acknowledgement,
                failureCategory: "receiptUnavailable",
                retryable: true,
                cancellationToken);
        }

        return outcome switch
        {
            ComplianceDeliveryOutcome.AcknowledgedNewReceipt =>
                ComplianceConsumerRunOutcome.AcknowledgedNewReceipt,
            ComplianceDeliveryOutcome.AcknowledgedEquivalentDuplicate =>
                ComplianceConsumerRunOutcome
                    .AcknowledgedEquivalentDuplicate,
            ComplianceDeliveryOutcome.InvalidContract =>
                await RecoverNonRetryableAsync(
                    delivery,
                    acknowledgement,
                    "invalidContract",
                    ComplianceConsumerRunOutcome.InvalidContract,
                    cancellationToken),
            ComplianceDeliveryOutcome.ConflictingBytes =>
                await RecoverNonRetryableAsync(
                    delivery,
                    acknowledgement,
                    "conflictingBytes",
                    ComplianceConsumerRunOutcome.ConflictingBytes,
                    cancellationToken),
            _ => throw new InvalidOperationException(
                $"Unsupported delivery outcome '{outcome}'.")
        };
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();

        if (_ownedResource is not null)
        {
            await _ownedResource.DisposeAsync();
        }
    }

    private static async Task<RabbitMqComplianceConsumer> CreateCoreAsync(
        RabbitMqConsumerOptions options,
        ComplianceDeliveryProcessor processor,
        ComplianceDeliveryRecoveryHandler? recovery,
        IAsyncDisposable? ownedResource,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(processor);

        IConnection? connection = null;
        IChannel? channel = null;

        try
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = options.ConnectionUri,
                AutomaticRecoveryEnabled = false
            };

            connection = await connectionFactory.CreateConnectionAsync(
                cancellationToken);
            channel = await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                options.ExchangeName,
                options.ExchangeType,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);
            await channel.QueueDeclareAsync(
                options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);
            await channel.QueueBindAsync(
                options.QueueName,
                options.ExchangeName,
                options.RoutingKey,
                cancellationToken: cancellationToken);
            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken);

            return new RabbitMqComplianceConsumer(
                options,
                processor,
                recovery,
                connection,
                channel,
                ownedResource);
        }
        catch
        {
            await DisposeCreatedResourcesAsync(channel, connection);
            throw;
        }
    }

    private async Task<ComplianceConsumerRunOutcome>
        RecoverNonRetryableAsync(
            BasicGetResult delivery,
            IComplianceDeliveryAcknowledgement acknowledgement,
            string failureCategory,
            ComplianceConsumerRunOutcome fallbackOutcome,
            CancellationToken cancellationToken)
    {
        if (_recovery is null)
        {
            return fallbackOutcome;
        }

        return await RecoverAsync(
            delivery,
            acknowledgement,
            failureCategory,
            retryable: false,
            cancellationToken);
    }

    private async Task<ComplianceConsumerRunOutcome> RecoverAsync(
        BasicGetResult delivery,
        IComplianceDeliveryAcknowledgement acknowledgement,
        string failureCategory,
        bool retryable,
        CancellationToken cancellationToken)
    {
        ComplianceDeliveryRecoveryHandler recovery = _recovery ??
            throw new InvalidOperationException(
                "Compliance delivery recovery is not configured.");
        Guid eventId = ReadEventId(delivery);
        int eventVersion = ReadEventVersion(delivery);
        int currentAttempt = ReadCurrentAttempt(delivery);
        ComplianceRecoveryRoute route = await recovery.RecoverAsync(
            eventId,
            eventVersion,
            delivery.Body,
            currentAttempt,
            failureCategory,
            retryable,
            acknowledgement,
            cancellationToken);

        return route == ComplianceRecoveryRoute.Retry
            ? ComplianceConsumerRunOutcome.Retried
            : ComplianceConsumerRunOutcome.DeadLettered;
    }

    private static Guid ReadEventId(BasicGetResult delivery)
    {
        string? messageId = delivery.BasicProperties.MessageId;

        if (messageId is not null &&
            Guid.TryParseExact(messageId, "D", out Guid eventId) &&
            eventId != Guid.Empty)
        {
            return eventId;
        }

        if (TryReadJsonIntOrString(
                delivery.Body,
                "eventId",
                out string? jsonEventId) &&
            Guid.TryParseExact(jsonEventId, "D", out eventId) &&
            eventId != Guid.Empty)
        {
            return eventId;
        }

        throw new InvalidOperationException(
            "Delivery has no stable EventId for recovery.");
    }

    private static int ReadEventVersion(BasicGetResult delivery)
    {
        if (TryReadPositiveHeader(
                delivery.BasicProperties.Headers,
                EventVersionHeader,
                out int headerVersion))
        {
            return headerVersion;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(delivery.Body);

            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty(
                    "eventVersion",
                    out JsonElement version) &&
                version.TryGetInt32(out int jsonVersion) &&
                jsonVersion > 0)
            {
                return jsonVersion;
            }
        }
        catch (JsonException)
        {
        }

        return 1;
    }

    private static int ReadCurrentAttempt(BasicGetResult delivery)
    {
        return TryReadPositiveHeader(
            delivery.BasicProperties.Headers,
            AutomaticAttemptHeader,
            out int attempt)
            ? attempt
            : 1;
    }

    private static Activity? StartConsumerActivity(
        IDictionary<string, object?>? headers)
    {
        string? traceParent = ReadUtf8Header(headers, TraceParentHeader);
        string? traceState = ReadUtf8Header(headers, TraceStateHeader);

        if (ActivityContext.TryParse(
            traceParent,
            traceState,
            isRemote: true,
            out ActivityContext parentContext))
        {
            return ConsumerActivitySource.StartActivity(
                "vendor registered consume",
                ActivityKind.Consumer,
                parentContext);
        }

        return ConsumerActivitySource.StartActivity(
            "vendor registered consume",
            ActivityKind.Consumer,
            default(ActivityContext));
    }

    private static string? ReadUtf8Header(
        IDictionary<string, object?>? headers,
        string headerName)
    {
        if (headers is null ||
            !headers.TryGetValue(headerName, out object? value))
        {
            return null;
        }

        return value switch
        {
            string text => text,
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            ReadOnlyMemory<byte> bytes => Encoding.UTF8.GetString(bytes.Span),
            _ => null
        };
    }

    private static bool TryReadPositiveHeader(
        IDictionary<string, object?>? headers,
        string headerName,
        out int value)
    {
        value = 0;

        if (headers is null ||
            !headers.TryGetValue(headerName, out object? rawValue) ||
            rawValue is null)
        {
            return false;
        }

        try
        {
            value = Convert.ToInt32(rawValue);
            return value > 0;
        }
        catch (Exception exception)
            when (exception is FormatException or
                InvalidCastException or
                OverflowException)
        {
            return false;
        }
    }

    private static bool TryReadJsonIntOrString(
        ReadOnlyMemory<byte> serializedEvent,
        string propertyName,
        out string? value)
    {
        value = null;

        try
        {
            using JsonDocument document = JsonDocument.Parse(serializedEvent);

            if (document.RootElement.ValueKind != JsonValueKind.Object ||
                !document.RootElement.TryGetProperty(
                    propertyName,
                    out JsonElement property) ||
                property.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = property.GetString();
            return value is not null;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static async Task DisposeCreatedResourcesAsync(
        IChannel? channel,
        IConnection? connection)
    {
        if (channel is not null)
        {
            await channel.DisposeAsync();
        }

        if (connection is not null)
        {
            await connection.DisposeAsync();
        }
    }

    private sealed class RabbitMqDeliveryAcknowledgement
        : IComplianceDeliveryAcknowledgement
    {
        private readonly IChannel _channel;
        private readonly ulong _deliveryTag;
        private bool _acknowledged;

        public RabbitMqDeliveryAcknowledgement(
            IChannel channel,
            ulong deliveryTag)
        {
            _channel = channel;
            _deliveryTag = deliveryTag;
        }

        public async Task AcknowledgeAsync(
            CancellationToken cancellationToken = default)
        {
            if (_acknowledged)
            {
                throw new InvalidOperationException(
                    "The RabbitMQ delivery has already been acknowledged.");
            }

            await _channel.BasicAckAsync(
                _deliveryTag,
                multiple: false,
                cancellationToken);
            _acknowledged = true;
        }
    }
}
