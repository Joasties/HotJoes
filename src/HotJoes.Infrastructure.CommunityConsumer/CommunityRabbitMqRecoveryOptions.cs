namespace HotJoes.Infrastructure.CommunityConsumer;
public sealed class CommunityRabbitMqRecoveryOptions
{
    public CommunityRabbitMqRecoveryOptions(string connectionString,
        string primaryExchangeName, string primaryRoutingKey,
        string retryExchangeName, string retryQueueName, string retryRoutingKey,
        string deadLetterExchangeName, string deadLetterQueueName,
        string deadLetterRoutingKey, TimeSpan retryDelay)
    {
        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out Uri? uri) ||
            (uri.Scheme != "amqp" && uri.Scheme != "amqps")) throw new ArgumentException("AMQP URI required.", nameof(connectionString));
        if (retryDelay <= TimeSpan.Zero || retryDelay.TotalMilliseconds > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(retryDelay));
        ConnectionUri = uri; PrimaryExchangeName = R(primaryExchangeName); PrimaryRoutingKey = R(primaryRoutingKey);
        RetryExchangeName = R(retryExchangeName); RetryQueueName = R(retryQueueName); RetryRoutingKey = R(retryRoutingKey);
        DeadLetterExchangeName = R(deadLetterExchangeName); DeadLetterQueueName = R(deadLetterQueueName); DeadLetterRoutingKey = R(deadLetterRoutingKey); RetryDelay = retryDelay;
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
    private static string R(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Topology value is required.") : value;
}
