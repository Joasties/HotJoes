namespace HotJoes.Worker.ComplianceConsumer;

public sealed class ComplianceConsumerHostOptions
{
    private const string Prefix = "HotJoes__ComplianceConsumer__";

    private ComplianceConsumerHostOptions(
        string complianceDatabase,
        string rabbitMq,
        string primaryExchangeName,
        string primaryExchangeType,
        string primaryQueueName,
        string primaryRoutingKey,
        string retryExchangeName,
        string retryQueueName,
        string retryRoutingKey,
        string deadLetterExchangeName,
        string deadLetterQueueName,
        string deadLetterRoutingKey,
        TimeSpan pollInterval,
        TimeSpan retryDelay,
        int maximumAutomaticAttempts)
    {
        ComplianceDatabase = complianceDatabase;
        RabbitMq = rabbitMq;
        PrimaryExchangeName = primaryExchangeName;
        PrimaryExchangeType = primaryExchangeType;
        PrimaryQueueName = primaryQueueName;
        PrimaryRoutingKey = primaryRoutingKey;
        RetryExchangeName = retryExchangeName;
        RetryQueueName = retryQueueName;
        RetryRoutingKey = retryRoutingKey;
        DeadLetterExchangeName = deadLetterExchangeName;
        DeadLetterQueueName = deadLetterQueueName;
        DeadLetterRoutingKey = deadLetterRoutingKey;
        PollInterval = pollInterval;
        RetryDelay = retryDelay;
        MaximumAutomaticAttempts = maximumAutomaticAttempts;
    }

    public string ComplianceDatabase { get; }
    public string RabbitMq { get; }
    public string PrimaryExchangeName { get; }
    public string PrimaryExchangeType { get; }
    public string PrimaryQueueName { get; }
    public string PrimaryRoutingKey { get; }
    public string RetryExchangeName { get; }
    public string RetryQueueName { get; }
    public string RetryRoutingKey { get; }
    public string DeadLetterExchangeName { get; }
    public string DeadLetterQueueName { get; }
    public string DeadLetterRoutingKey { get; }
    public TimeSpan PollInterval { get; }
    public TimeSpan RetryDelay { get; }
    public int MaximumAutomaticAttempts { get; }

    public static ComplianceConsumerHostOptions LoadFromEnvironment()
    {
        return new ComplianceConsumerHostOptions(
            Required("ComplianceDatabase"),
            Required("RabbitMq"),
            Required("PrimaryExchangeName"),
            Required("PrimaryExchangeType"),
            Required("PrimaryQueueName"),
            Required("PrimaryRoutingKey"),
            Required("RetryExchangeName"),
            Required("RetryQueueName"),
            Required("RetryRoutingKey"),
            Required("DeadLetterExchangeName"),
            Required("DeadLetterQueueName"),
            Required("DeadLetterRoutingKey"),
            TimeSpan.FromMilliseconds(
                PositiveInt("PollIntervalMilliseconds")),
            TimeSpan.FromSeconds(PositiveInt("RetryDelaySeconds")),
            PositiveInt("MaximumAutomaticAttempts"));
    }

    private static string Required(string name)
    {
        string key = $"{Prefix}{name}";
        string? value = Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Required Compliance consumer configuration '{key}' is " +
                "missing.");
        }

        return value;
    }

    private static int PositiveInt(string name)
    {
        string value = Required(name);

        if (!int.TryParse(value, out int result) || result <= 0)
        {
            throw new InvalidOperationException(
                $"Compliance consumer configuration '{Prefix}{name}' " +
                "must be a positive integer.");
        }

        return result;
    }
}
