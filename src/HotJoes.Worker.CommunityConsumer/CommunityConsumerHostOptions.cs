namespace HotJoes.Worker.CommunityConsumer;
public sealed class CommunityConsumerHostOptions
{
    private const string P = "HotJoes__CommunityConsumer__";
    private CommunityConsumerHostOptions(string database, string rabbitMq,
        string primaryExchange, string primaryType, string primaryQueue, string primaryKey,
        string retryExchange, string retryQueue, string retryKey,
        string deadExchange, string deadQueue, string deadKey,
        TimeSpan poll, TimeSpan retry, int attempts)
    { CommunityDatabase = database; RabbitMq = rabbitMq; PrimaryExchangeName = primaryExchange; PrimaryExchangeType = primaryType; PrimaryQueueName = primaryQueue; PrimaryRoutingKey = primaryKey; RetryExchangeName = retryExchange; RetryQueueName = retryQueue; RetryRoutingKey = retryKey; DeadLetterExchangeName = deadExchange; DeadLetterQueueName = deadQueue; DeadLetterRoutingKey = deadKey; PollInterval = poll; RetryDelay = retry; MaximumAutomaticAttempts = attempts; }
    public string CommunityDatabase { get; } public string RabbitMq { get; }
    public string PrimaryExchangeName { get; } public string PrimaryExchangeType { get; } public string PrimaryQueueName { get; } public string PrimaryRoutingKey { get; }
    public string RetryExchangeName { get; } public string RetryQueueName { get; } public string RetryRoutingKey { get; }
    public string DeadLetterExchangeName { get; } public string DeadLetterQueueName { get; } public string DeadLetterRoutingKey { get; }
    public TimeSpan PollInterval { get; } public TimeSpan RetryDelay { get; } public int MaximumAutomaticAttempts { get; }
    public static CommunityConsumerHostOptions LoadFromEnvironment() => new(
        R("CommunityDatabase"), R("RabbitMq"), R("PrimaryExchangeName"), R("PrimaryExchangeType"), R("PrimaryQueueName"), R("PrimaryRoutingKey"),
        R("RetryExchangeName"), R("RetryQueueName"), R("RetryRoutingKey"), R("DeadLetterExchangeName"), R("DeadLetterQueueName"), R("DeadLetterRoutingKey"),
        TimeSpan.FromMilliseconds(I("PollIntervalMilliseconds")), TimeSpan.FromSeconds(I("RetryDelaySeconds")), I("MaximumAutomaticAttempts"));
    private static string R(string name) => Environment.GetEnvironmentVariable(P + name) is string value && !string.IsNullOrWhiteSpace(value) ? value : throw new InvalidOperationException($"Required Community consumer configuration '{P}{name}' is missing.");
    private static int I(string name) => int.TryParse(R(name), out int value) && value > 0 ? value : throw new InvalidOperationException($"Community consumer configuration '{P}{name}' must be a positive integer.");
}
