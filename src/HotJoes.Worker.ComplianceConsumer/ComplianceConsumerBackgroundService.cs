using HotJoes.Infrastructure.ComplianceConsumer;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Worker.ComplianceConsumer;

public sealed class ComplianceConsumerBackgroundService : BackgroundService
{
    private readonly ComplianceConsumerHostOptions _options;
    private readonly ILogger<ComplianceConsumerBackgroundService> _logger;

    public ComplianceConsumerBackgroundService(
        ComplianceConsumerHostOptions options,
        ILoggerFactory loggerFactory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _logger = loggerFactory.CreateLogger<
            ComplianceConsumerBackgroundService>();
        _processorLogger = loggerFactory.CreateLogger<
            ComplianceDeliveryProcessor>();
        _recoveryLogger = loggerFactory.CreateLogger<
            ComplianceDeliveryRecoveryHandler>();
    }

    private readonly ILogger<ComplianceDeliveryProcessor> _processorLogger;
    private readonly ILogger<ComplianceDeliveryRecoveryHandler>
        _recoveryLogger;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunConnectedAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception)
            {
                _logger.LogWarning(
                    "Compliance consumer dependency connection is " +
                    "unavailable.");
            }

            await DelayAsync(stoppingToken);
        }
    }

    private async Task RunConnectedAsync(CancellationToken stoppingToken)
    {
        DbContextOptions<ComplianceReceiptDbContext> contextOptions =
            new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
                .UseNpgsql(_options.ComplianceDatabase)
                .Options;
        await using var context =
            new ComplianceReceiptDbContext(contextOptions);
        var store = new PostgreSqlComplianceReceiptStore(context);
        var processor = new ComplianceDeliveryProcessor(
            store,
            _processorLogger);

        var recoveryOptions = new RabbitMqRecoveryOptions(
            _options.RabbitMq,
            _options.PrimaryExchangeName,
            _options.PrimaryRoutingKey,
            _options.RetryExchangeName,
            _options.RetryQueueName,
            _options.RetryRoutingKey,
            _options.DeadLetterExchangeName,
            _options.DeadLetterQueueName,
            _options.DeadLetterRoutingKey,
            _options.RetryDelay);
        await using RabbitMqComplianceRecoveryPublisher recoveryPublisher =
            await RabbitMqComplianceRecoveryPublisher.CreateAsync(
                recoveryOptions,
                stoppingToken);
        var dispatcher = new ComplianceRecoveryDispatcher(recoveryPublisher);
        var retryPolicy = new ComplianceConsumerRetryPolicy(
            _options.MaximumAutomaticAttempts,
            _options.RetryDelay);
        var recovery = new ComplianceDeliveryRecoveryHandler(
            retryPolicy,
            dispatcher,
            _recoveryLogger);

        var consumerOptions = new RabbitMqConsumerOptions(
            _options.RabbitMq,
            _options.PrimaryExchangeName,
            _options.PrimaryExchangeType,
            _options.PrimaryQueueName,
            _options.PrimaryRoutingKey);
        await using RabbitMqComplianceConsumer consumer =
            await RabbitMqComplianceConsumer.CreateAsync(
                consumerOptions,
                processor,
                recovery,
                cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await consumer.RunOnceAsync(
                DateTimeOffset.UtcNow,
                stoppingToken);
            await DelayAsync(stoppingToken);
        }
    }

    private async Task DelayAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(_options.PollInterval, stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal host shutdown ends the polling interval.
        }
    }
}
