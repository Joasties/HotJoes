using RabbitMQ.Client;

namespace HotJoes.Infrastructure.CommunityRelay;

public sealed class RabbitMqCommunityOutboxEventPublisher
    : ICommunityOutboxEventPublisher, IAsyncDisposable
{
    private readonly CommunityRabbitMqPublisherOptions options;
    private readonly IConnection connection;
    private readonly IChannel channel;

    private RabbitMqCommunityOutboxEventPublisher(
        CommunityRabbitMqPublisherOptions options, IConnection connection,
        IChannel channel)
    {
        this.options = options;
        this.connection = connection;
        this.channel = channel;
    }

    public static async Task<RabbitMqCommunityOutboxEventPublisher> CreateAsync(
        CommunityRabbitMqPublisherOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        IConnection? connection = null;
        IChannel? channel = null;
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = options.ConnectionUri,
                AutomaticRecoveryEnabled = false
            };
            connection = await factory.CreateConnectionAsync(cancellationToken);
            channel = await connection.CreateChannelAsync(
                new CreateChannelOptions(true, true), cancellationToken);
            await channel.ExchangeDeclareAsync(options.ExchangeName,
                options.ExchangeType, true, false,
                cancellationToken: cancellationToken);
            await channel.QueueDeclareAsync(options.QueueName, true, false,
                false, cancellationToken: cancellationToken);
            await channel.QueueBindAsync(options.QueueName,
                options.ExchangeName, options.RoutingKey,
                cancellationToken: cancellationToken);
            return new(options, connection, channel);
        }
        catch (OperationCanceledException)
        {
            await DisposeAsync(channel, connection);
            throw;
        }
        catch (Exception exception)
        {
            await DisposeAsync(channel, connection);
            throw new CommunityOutboxPublicationException(
                "RabbitMQ Community publisher initialization failed.", exception);
        }
    }

    public async Task<CommunityOutboxPublicationConfirmation> PublishAsync(
        CommunityOutboxPublication publication,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(publication);
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            MessageId = publication.EventId.ToString("D"),
            Persistent = true,
            Headers = Headers(publication)
        };
        try
        {
            await channel.BasicPublishAsync(options.ExchangeName,
                options.RoutingKey, true, properties,
                publication.SerializedEvent, cancellationToken);
            return CommunityOutboxPublicationConfirmation.Confirmed;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception exception)
        {
            throw new CommunityOutboxPublicationException(
                "RabbitMQ did not confirm Community publication.", exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await channel.DisposeAsync();
        await connection.DisposeAsync();
    }

    private static IDictionary<string, object?>? Headers(
        CommunityOutboxPublication publication)
    {
        if (publication.TraceParent is null) return null;
        var headers = new Dictionary<string, object?>
        {
            ["traceparent"] = publication.TraceParent
        };
        if (publication.TraceState is not null)
            headers["tracestate"] = publication.TraceState;
        return headers;
    }

    private static async Task DisposeAsync(IChannel? channel,
        IConnection? connection)
    {
        if (channel is not null) await channel.DisposeAsync();
        if (connection is not null) await connection.DisposeAsync();
    }
}
