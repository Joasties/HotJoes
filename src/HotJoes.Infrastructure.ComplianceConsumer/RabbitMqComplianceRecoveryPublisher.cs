using RabbitMQ.Client;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class RabbitMqComplianceRecoveryPublisher
    : IComplianceRecoveryPublisher,
      IAsyncDisposable
{
    private const string AutomaticAttemptHeader =
        "x-hotjoes-automatic-attempt";
    private const string FailureCategoryHeader =
        "x-hotjoes-failure-category";
    private const string EventVersionHeader =
        "x-hotjoes-event-version";

    private readonly RabbitMqRecoveryOptions _options;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    private RabbitMqComplianceRecoveryPublisher(
        RabbitMqRecoveryOptions options,
        IConnection connection,
        IChannel channel)
    {
        _options = options;
        _connection = connection;
        _channel = channel;
    }

    public static async Task<RabbitMqComplianceRecoveryPublisher> CreateAsync(
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

            return new RabbitMqComplianceRecoveryPublisher(
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

    public async Task PublishAsync(
        ComplianceRecoveryRoute route,
        ComplianceRecoveryPublication publication,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(publication);

        (string exchange, string routingKey) = route switch
        {
            ComplianceRecoveryRoute.Retry =>
                (_options.RetryExchangeName, _options.RetryRoutingKey),
            ComplianceRecoveryRoute.DeadLetter =>
                (_options.DeadLetterExchangeName,
                    _options.DeadLetterRoutingKey),
            _ => throw new ArgumentOutOfRangeException(nameof(route))
        };

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            MessageId = publication.EventId.ToString("D"),
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                [AutomaticAttemptHeader] = publication.AutomaticAttempt,
                [FailureCategoryHeader] = publication.FailureCategory,
                [EventVersionHeader] = publication.EventVersion
            }
        };

        await _channel.BasicPublishAsync(
            exchange,
            routingKey,
            mandatory: true,
            properties,
            publication.SerializedEvent,
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
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
            options.RetryExchangeName,
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

        var retryArguments = new Dictionary<string, object?>
        {
            ["x-message-ttl"] = Convert.ToInt64(
                options.RetryDelay.TotalMilliseconds),
            ["x-dead-letter-exchange"] = options.PrimaryExchangeName,
            ["x-dead-letter-routing-key"] = options.PrimaryRoutingKey
        };
        await channel.QueueDeclareAsync(
            options.RetryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArguments,
            cancellationToken: cancellationToken);
        await channel.QueueBindAsync(
            options.RetryQueueName,
            options.RetryExchangeName,
            options.RetryRoutingKey,
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
