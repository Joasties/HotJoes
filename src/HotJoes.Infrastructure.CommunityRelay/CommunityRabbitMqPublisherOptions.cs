namespace HotJoes.Infrastructure.CommunityRelay;

public sealed class CommunityRabbitMqPublisherOptions
{
    public CommunityRabbitMqPublisherOptions(string connectionString,
        string exchangeName, string exchangeType, string queueName,
        string routingKey)
    {
        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out Uri? uri) ||
            (uri.Scheme != "amqp" && uri.Scheme != "amqps"))
            throw new ArgumentException("RabbitMQ connection string must be an AMQP URI.", nameof(connectionString));
        ConnectionUri = uri;
        ExchangeName = Require(exchangeName, nameof(exchangeName));
        ExchangeType = Require(exchangeType, nameof(exchangeType));
        QueueName = Require(queueName, nameof(queueName));
        RoutingKey = Require(routingKey, nameof(routingKey));
    }

    public Uri ConnectionUri { get; }
    public string ExchangeName { get; }
    public string ExchangeType { get; }
    public string QueueName { get; }
    public string RoutingKey { get; }

    private static string Require(string value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("RabbitMQ topology value must not be empty.", name)
            : value;
}
