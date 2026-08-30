namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class RabbitMqRecoveryOptions
{
    public RabbitMqRecoveryOptions(
        string connectionString,
        string primaryExchangeName,
        string primaryRoutingKey,
        string retryExchangeName,
        string retryQueueName,
        string retryRoutingKey,
        string deadLetterExchangeName,
        string deadLetterQueueName,
        string deadLetterRoutingKey,
        TimeSpan retryDelay)
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

        if (retryDelay <= TimeSpan.Zero ||
            retryDelay.TotalMilliseconds > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(retryDelay));
        }

        ConnectionUri = connectionUri;
        PrimaryExchangeName = RequireValue(
            primaryExchangeName,
            nameof(primaryExchangeName));
        PrimaryRoutingKey = RequireValue(
            primaryRoutingKey,
            nameof(primaryRoutingKey));
        RetryExchangeName = RequireValue(
            retryExchangeName,
            nameof(retryExchangeName));
        RetryQueueName = RequireValue(
            retryQueueName,
            nameof(retryQueueName));
        RetryRoutingKey = RequireValue(
            retryRoutingKey,
            nameof(retryRoutingKey));
        DeadLetterExchangeName = RequireValue(
            deadLetterExchangeName,
            nameof(deadLetterExchangeName));
        DeadLetterQueueName = RequireValue(
            deadLetterQueueName,
            nameof(deadLetterQueueName));
        DeadLetterRoutingKey = RequireValue(
            deadLetterRoutingKey,
            nameof(deadLetterRoutingKey));
        RetryDelay = retryDelay;
    }

    public Uri ConnectionUri { get; }

    public string PrimaryExchangeName { get; }

    public string PrimaryRoutingKey { get; }

    public string RetryExchangeName { get; }

    public string RetryQueueName { get; }

    public string RetryRoutingKey { get; }

    public string DeadLetterExchangeName { get; }

    public string DeadLetterQueueName { get; }

    public string DeadLetterRoutingKey { get; }

    public TimeSpan RetryDelay { get; }

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
