namespace HotJoes.Worker.VendorRelay;

public sealed class VendorRelayHostOptions
{
    private const string Prefix = "HotJoes__VendorRelay__";

    private VendorRelayHostOptions(
        string vendorDatabase,
        string rabbitMq,
        string exchangeName,
        string exchangeType,
        string queueName,
        string routingKey,
        TimeSpan pollInterval,
        TimeSpan leaseDuration,
        int batchSize,
        TimeSpan retryInitialDelay,
        TimeSpan retryMaximumDelay,
        int automaticAttemptLimit)
    {
        VendorDatabase = vendorDatabase;
        RabbitMq = rabbitMq;
        ExchangeName = exchangeName;
        ExchangeType = exchangeType;
        QueueName = queueName;
        RoutingKey = routingKey;
        PollInterval = pollInterval;
        LeaseDuration = leaseDuration;
        BatchSize = batchSize;
        RetryInitialDelay = retryInitialDelay;
        RetryMaximumDelay = retryMaximumDelay;
        AutomaticAttemptLimit = automaticAttemptLimit;
    }

    public string VendorDatabase { get; }
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

    public static VendorRelayHostOptions LoadFromEnvironment()
    {
        TimeSpan pollInterval = TimeSpan.FromMilliseconds(
            PositiveInt("PollIntervalMilliseconds"));
        TimeSpan leaseDuration = TimeSpan.FromSeconds(
            PositiveInt("LeaseDurationSeconds"));
        TimeSpan initialDelay = TimeSpan.FromSeconds(
            PositiveInt("RetryInitialDelaySeconds"));
        TimeSpan maximumDelay = TimeSpan.FromSeconds(
            PositiveInt("RetryMaximumDelaySeconds"));

        if (maximumDelay < initialDelay)
        {
            throw new InvalidOperationException(
                "Vendor relay retry maximum delay must not be less than " +
                "its initial delay.");
        }

        return new VendorRelayHostOptions(
            Required("VendorDatabase"),
            Required("RabbitMq"),
            Required("ExchangeName"),
            Required("ExchangeType"),
            Required("QueueName"),
            Required("RoutingKey"),
            pollInterval,
            leaseDuration,
            PositiveInt("BatchSize"),
            initialDelay,
            maximumDelay,
            PositiveInt("AutomaticAttemptLimit"));
    }

    private static string Required(string name)
    {
        string key = $"{Prefix}{name}";
        string? value = Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Required Vendor relay configuration '{key}' is missing.");
        }

        return value;
    }

    private static int PositiveInt(string name)
    {
        string value = Required(name);

        if (!int.TryParse(value, out int result) || result <= 0)
        {
            throw new InvalidOperationException(
                $"Vendor relay configuration '{Prefix}{name}' must be a " +
                "positive integer.");
        }

        return result;
    }
}
