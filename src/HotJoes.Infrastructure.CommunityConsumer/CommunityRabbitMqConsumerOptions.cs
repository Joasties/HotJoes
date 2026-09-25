namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed class CommunityRabbitMqConsumerOptions
{
    public CommunityRabbitMqConsumerOptions(string connectionString, string exchangeName,
        string exchangeType, string queueName, string routingKey)
    {
        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out Uri? uri) ||
            (uri.Scheme != "amqp" && uri.Scheme != "amqps")) throw new ArgumentException("RabbitMQ connection string must be an AMQP URI.", nameof(connectionString));
        ConnectionString = connectionString; ConnectionUri = uri;
        ExchangeName = Require(exchangeName); ExchangeType = Require(exchangeType);
        QueueName = Require(queueName); RoutingKey = Require(routingKey);
    }
    public string ConnectionString { get; }
    public Uri ConnectionUri { get; }
    public string ExchangeName { get; }
    public string ExchangeType { get; }
    public string QueueName { get; }
    public string RoutingKey { get; }
    private static string Require(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("RabbitMQ topology value must not be empty.") : value;
}
