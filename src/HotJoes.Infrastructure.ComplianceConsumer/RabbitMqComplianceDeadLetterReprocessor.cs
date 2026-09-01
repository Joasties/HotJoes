using RabbitMQ.Client;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class RabbitMqComplianceDeadLetterReprocessor
    : IAsyncDisposable
{
    private readonly RabbitMqRecoveryOptions _options;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    private RabbitMqComplianceDeadLetterReprocessor(
        RabbitMqRecoveryOptions options,
        IConnection connection,
        IChannel channel)
    {
        _options = options;
        _connection = connection;
        _channel = channel;
    }

    public static async Task<RabbitMqComplianceDeadLetterReprocessor>
        CreateAsync(
            RabbitMqRecoveryOptions options,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

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
                new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true),
                cancellationToken);

            await DeclareTopologyAsync(
                channel,
                options,
                cancellationToken);

            return new RabbitMqComplianceDeadLetterReprocessor(
                options,
                connection,
                channel);
        }
        catch
        {
            await DisposeCreatedResourcesAsync(channel, connection);
            throw;
        }
    }

    public async Task<ComplianceDeadLetterReprocessOutcome> RunOnceAsync(
        CancellationToken cancellationToken = default)
    {
        BasicGetResult? delivery = await _channel.BasicGetAsync(
            _options.DeadLetterQueueName,
            autoAck: false,
            cancellationToken);

        if (delivery is null)
        {
            return ComplianceDeadLetterReprocessOutcome.NoDelivery;
        }

        BasicProperties properties = CopyProperties(
            delivery.BasicProperties);

        await _channel.BasicPublishAsync(
            _options.PrimaryExchangeName,
            _options.PrimaryRoutingKey,
            mandatory: true,
            properties,
            delivery.Body,
            cancellationToken);
        await _channel.BasicAckAsync(
            delivery.DeliveryTag,
            multiple: false,
            cancellationToken);

        return ComplianceDeadLetterReprocessOutcome.Reprocessed;
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private static BasicProperties CopyProperties(
        IReadOnlyBasicProperties source)
    {
        return new BasicProperties
        {
            AppId = source.AppId,
            ClusterId = source.ClusterId,
            ContentEncoding = source.ContentEncoding,
            ContentType = source.ContentType,
            CorrelationId = source.CorrelationId,
            DeliveryMode = source.DeliveryMode,
            Expiration = source.Expiration,
            Headers = CopyHeaders(source.Headers),
            MessageId = source.MessageId,
            Priority = source.Priority,
            ReplyTo = source.ReplyTo,
            Timestamp = source.Timestamp,
            Type = source.Type,
            UserId = source.UserId
        };
    }

    private static Dictionary<string, object?>? CopyHeaders(
        IDictionary<string, object?>? headers)
    {
        if (headers is null)
        {
            return null;
        }

        return headers.ToDictionary(
            pair => pair.Key,
            pair => CopyHeaderValue(pair.Value),
            StringComparer.Ordinal);
    }

    private static object? CopyHeaderValue(object? value)
    {
        return value switch
        {
            byte[] bytes => bytes.ToArray(),
            _ => value
        };
    }

    private static async Task DeclareTopologyAsync(
        IChannel channel,
        RabbitMqRecoveryOptions options,
        CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            options.PrimaryExchangeName,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(
            options.DeadLetterExchangeName,
            ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(
            options.DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
        await channel.QueueBindAsync(
            options.DeadLetterQueueName,
            options.DeadLetterExchangeName,
            options.DeadLetterRoutingKey,
            cancellationToken: cancellationToken);
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken);
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
}
