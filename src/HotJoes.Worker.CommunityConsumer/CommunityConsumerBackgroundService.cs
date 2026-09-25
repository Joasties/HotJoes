using HotJoes.Infrastructure.CommunityConsumer;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Worker.CommunityConsumer;

public sealed class CommunityConsumerBackgroundService : BackgroundService
{
    private readonly CommunityConsumerHostOptions options;
    private readonly ILogger<CommunityConsumerBackgroundService> logger;
    public CommunityConsumerBackgroundService(CommunityConsumerHostOptions options,
        ILogger<CommunityConsumerBackgroundService> logger)
    { this.options = options; this.logger = logger; }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await RunConnected(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
            catch (Exception) { logger.LogWarning("Community consumer dependency connection is unavailable."); }
            try { await Task.Delay(options.PollInterval, stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
        }
    }
    private async Task RunConnected(CancellationToken token)
    {
        var dbOptions = new DbContextOptionsBuilder<CommunityReceiptDbContext>()
            .UseNpgsql(options.CommunityDatabase).Options;
        await using var context = new CommunityReceiptDbContext(dbOptions);
        var processor = new CommunityDeliveryProcessor(
            new DeterministicCommunityParticipationRecordedStub(),
            new PostgreSqlCommunityReceiptStore(context));
        var recoveryOptions = new CommunityRabbitMqRecoveryOptions(options.RabbitMq,
            options.PrimaryExchangeName, options.PrimaryRoutingKey,
            options.RetryExchangeName, options.RetryQueueName, options.RetryRoutingKey,
            options.DeadLetterExchangeName, options.DeadLetterQueueName,
            options.DeadLetterRoutingKey, options.RetryDelay);
        await using var publisher = await RabbitMqCommunityRecoveryPublisher.CreateAsync(recoveryOptions, token);
        var recovery = new CommunityDeliveryRecoveryHandler(
            new CommunityConsumerRetryPolicy(options.MaximumAutomaticAttempts, options.RetryDelay),
            new CommunityRecoveryDispatcher(publisher));
        var consumerOptions = new CommunityRabbitMqConsumerOptions(options.RabbitMq,
            options.PrimaryExchangeName, options.PrimaryExchangeType,
            options.PrimaryQueueName, options.PrimaryRoutingKey);
        await using var consumer = await RabbitMqCommunityConsumer.CreateAsync(
            consumerOptions, processor, recovery, token);
        while (!token.IsCancellationRequested)
        {
            await consumer.RunOnceAsync(DateTimeOffset.UtcNow, token);
            await Task.Delay(options.PollInterval, token);
        }
    }
}
