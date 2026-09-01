namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class RabbitMqConsumerOptions
{
    public RabbitMqConsumerOptions(
        string connectionString,
        string exchangeName,
        string exchangeType,
        string queueName,
        string routingKey)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "RabbitMQ connection string must not be empty.",
                nameof(connectionString));
        }

        if (!Uri.TryCreate(
                connectionString,
                UriKind.Absolute,
                out Uri? connectionUri) ||
            (connectionUri.Scheme != "amqp" &&
                connectionUri.Scheme != "amqps"))
        {
            throw new ArgumentException(
                "RabbitMQ connection string must be an AMQP URI.",
                nameof(connectionString));
        }

        ConnectionString = connectionString;
        ConnectionUri = connectionUri;
        ExchangeName = RequireValue(exchangeName, nameof(exchangeName));
        ExchangeType = RequireValue(exchangeType, nameof(exchangeType));
        QueueName = RequireValue(queueName, nameof(queueName));
        RoutingKey = RequireValue(routingKey, nameof(routingKey));
    }

    public string ConnectionString { get; }

    public Uri ConnectionUri { get; }

    public string ExchangeName { get; }

    public string ExchangeType { get; }

    public string QueueName { get; }

    public string RoutingKey { get; }

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "RabbitMQ topology value must not be empty.",
                parameterName);
        }

        return value;
    }
}
