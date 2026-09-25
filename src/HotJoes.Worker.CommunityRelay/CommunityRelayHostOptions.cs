namespace HotJoes.Worker.CommunityRelay;

public sealed class CommunityRelayHostOptions
{
    private const string Prefix = "HotJoes__CommunityRelay__";

    private CommunityRelayHostOptions(string database, string rabbitMq,
        string exchange, string exchangeType, string queue, string routingKey,
        TimeSpan poll, TimeSpan lease, int batchSize, TimeSpan initialDelay,
        TimeSpan maximumDelay, int attemptLimit)
    {
        CommunityDatabase = database; RabbitMq = rabbitMq;
        ExchangeName = exchange; ExchangeType = exchangeType;
        QueueName = queue; RoutingKey = routingKey; PollInterval = poll;
        LeaseDuration = lease; BatchSize = batchSize;
        RetryInitialDelay = initialDelay; RetryMaximumDelay = maximumDelay;
        AutomaticAttemptLimit = attemptLimit;
    }

    public string CommunityDatabase { get; }
    public string RabbitMq { get; }
    public string ExchangeName { get; }
    public string ExchangeType { get; }
    public string QueueName { get; }
    public string RoutingKey { get; }
    public TimeSpan PollInterval { get; }
    public TimeSpan LeaseDuration { get; }
    public int BatchSize { get; }
    public TimeSpan RetryInitialDelay { get; }
    public TimeSpan RetryMaximumDelay { get; }
    public int AutomaticAttemptLimit { get; }

    public static CommunityRelayHostOptions LoadFromEnvironment()
    {
        TimeSpan initial = TimeSpan.FromSeconds(Positive("RetryInitialDelaySeconds"));
        TimeSpan maximum = TimeSpan.FromSeconds(Positive("RetryMaximumDelaySeconds"));
        if (maximum < initial)
            throw new InvalidOperationException("Community relay retry maximum delay must not be less than its initial delay.");
        return new(Required("CommunityDatabase"), Required("RabbitMq"),
            Required("ExchangeName"), Required("ExchangeType"),
            Required("QueueName"), Required("RoutingKey"),
            TimeSpan.FromMilliseconds(Positive("PollIntervalMilliseconds")),
            TimeSpan.FromSeconds(Positive("LeaseDurationSeconds")),
            Positive("BatchSize"), initial, maximum,
            Positive("AutomaticAttemptLimit"));
    }

    private static string Required(string name) =>
        Environment.GetEnvironmentVariable(Prefix + name) is string value &&
        !string.IsNullOrWhiteSpace(value) ? value : throw new InvalidOperationException(
            $"Required Community relay configuration '{Prefix}{name}' is missing.");

    private static int Positive(string name) =>
        int.TryParse(Required(name), out int value) && value > 0 ? value :
        throw new InvalidOperationException(
            $"Community relay configuration '{Prefix}{name}' must be a positive integer.");
}
