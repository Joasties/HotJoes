using RabbitMQ.Client;

namespace HotJoes.Infrastructure.VendorRelay;

public sealed class RabbitMqOutboxEventPublisher
    : IOutboxEventPublisher,
      IAsyncDisposable
{
    private readonly RabbitMqPublisherOptions _options;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    private RabbitMqOutboxEventPublisher(
        RabbitMqPublisherOptions options,
        IConnection connection,
        IChannel channel)
    {
        _options = options;
        _connection = connection;
        _channel = channel;
    }

    public static async Task<RabbitMqOutboxEventPublisher> CreateAsync(
        RabbitMqPublisherOptions options,
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

            return new RabbitMqOutboxEventPublisher(
                options,
                connection,
                channel);
        }
        catch (OperationCanceledException)
        {
            await DisposeCreatedResourcesAsync(channel, connection);
            throw;
        }
        catch (Exception exception)
        {
            await DisposeCreatedResourcesAsync(channel, connection);
            throw new OutboxPublicationException(
                "RabbitMQ publisher initialization failed.",
                exception);
        }
    }

    public async Task<OutboxPublicationConfirmation> PublishAsync(
        OutboxPublication publication,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(publication);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            MessageId = publication.EventId.ToString("D"),
            Persistent = true,
            Headers = CreateTraceHeaders(publication)
        };

        try
        {
            await _channel.BasicPublishAsync(
                _options.ExchangeName,
                _options.RoutingKey,
                mandatory: true,
                properties,
                publication.SerializedEvent,
                cancellationToken);

            return OutboxPublicationConfirmation.Confirmed;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new OutboxPublicationException(
                "RabbitMQ did not confirm publication.",
                exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private static IDictionary<string, object?>? CreateTraceHeaders(
        OutboxPublication publication)
    {
        if (publication.TraceParent is null)
        {
            return null;
        }

        var headers = new Dictionary<string, object?>
        {
            ["traceparent"] = publication.TraceParent
        };

        if (publication.TraceState is not null)
        {
            headers["tracestate"] = publication.TraceState;
        }

        return headers;
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
