using RabbitMQ.Client;
namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed class RabbitMqCommunityRecoveryPublisher : ICommunityRecoveryPublisher, IAsyncDisposable
{
    private readonly CommunityRabbitMqRecoveryOptions options;
    private readonly IConnection connection; private readonly IChannel channel;
    private RabbitMqCommunityRecoveryPublisher(CommunityRabbitMqRecoveryOptions options, IConnection connection, IChannel channel)
    { this.options = options; this.connection = connection; this.channel = channel; }
    public static async Task<RabbitMqCommunityRecoveryPublisher> CreateAsync(CommunityRabbitMqRecoveryOptions options, CancellationToken cancellationToken = default)
    {
        IConnection? connection = null; IChannel? channel = null;
        try
        {
            var factory = new ConnectionFactory { Uri = options.ConnectionUri, AutomaticRecoveryEnabled = false };
            connection = await factory.CreateConnectionAsync(cancellationToken);
            channel = await connection.CreateChannelAsync(new CreateChannelOptions(true, true), cancellationToken);
            await Declare(channel, options, cancellationToken);
            return new(options, connection, channel);
        }
        catch { if (channel is not null) await channel.DisposeAsync(); if (connection is not null) await connection.DisposeAsync(); throw; }
    }
    public async Task PublishAsync(CommunityRecoveryRoute route, CommunityRecoveryPublication publication, CancellationToken cancellationToken = default)
    {
        (string exchange, string key) = route == CommunityRecoveryRoute.Retry
            ? (options.RetryExchangeName, options.RetryRoutingKey)
            : route == CommunityRecoveryRoute.DeadLetter
                ? (options.DeadLetterExchangeName, options.DeadLetterRoutingKey)
                : throw new ArgumentOutOfRangeException(nameof(route));
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            MessageId = publication.EventId.ToString("D"),
            Persistent = true,
            Headers = new Dictionary<string, object?> { ["x-hotjoes-automatic-attempt"] = publication.AutomaticAttempt, ["x-hotjoes-failure-category"] = publication.FailureCategory, ["x-hotjoes-event-version"] = publication.EventVersion }
        };
        await channel.BasicPublishAsync(exchange, key, true, properties, publication.SerializedEvent, cancellationToken);
    }
    public async ValueTask DisposeAsync() { await channel.DisposeAsync(); await connection.DisposeAsync(); }
    private static async Task Declare(IChannel channel, CommunityRabbitMqRecoveryOptions o, CancellationToken token)
    {
        await channel.ExchangeDeclareAsync(o.PrimaryExchangeName, ExchangeType.Direct, true, false, cancellationToken: token);
        await channel.ExchangeDeclareAsync(o.RetryExchangeName, ExchangeType.Direct, true, false, cancellationToken: token);
        await channel.ExchangeDeclareAsync(o.DeadLetterExchangeName, ExchangeType.Direct, true, false, cancellationToken: token);
        var retryArgs = new Dictionary<string, object?> { ["x-message-ttl"] = Convert.ToInt64(o.RetryDelay.TotalMilliseconds), ["x-dead-letter-exchange"] = o.PrimaryExchangeName, ["x-dead-letter-routing-key"] = o.PrimaryRoutingKey };
        await channel.QueueDeclareAsync(
            o.RetryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArgs,
            cancellationToken: token);
        await channel.QueueBindAsync(o.RetryQueueName, o.RetryExchangeName, o.RetryRoutingKey, cancellationToken: token);
        await channel.QueueDeclareAsync(o.DeadLetterQueueName, true, false, false, cancellationToken: token);
        await channel.QueueBindAsync(o.DeadLetterQueueName, o.DeadLetterExchangeName, o.DeadLetterRoutingKey, cancellationToken: token);
    }
}
