using HotJoes.Infrastructure.Community.Persistence;
using HotJoes.Infrastructure.CommunityRelay;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Worker.CommunityRelay;

public sealed class CommunityRelayBackgroundService : BackgroundService
{
    private readonly CommunityRelayHostOptions options;
    private readonly ILogger<CommunityRelayBackgroundService> logger;
    private readonly ILoggerFactory loggerFactory;
    private readonly Guid workerId = Guid.NewGuid();

    public CommunityRelayBackgroundService(CommunityRelayHostOptions options,
        ILogger<CommunityRelayBackgroundService> logger,
        ILoggerFactory loggerFactory)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await RunConnectedAsync(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
            catch (Exception) { logger.LogWarning("Community relay dependency connection is unavailable."); }
            await DelayAsync(stoppingToken);
        }
    }

    private async Task RunConnectedAsync(CancellationToken token)
    {
        var publisherOptions = new CommunityRabbitMqPublisherOptions(
            options.RabbitMq, options.ExchangeName, options.ExchangeType,
            options.QueueName, options.RoutingKey);
        await using RabbitMqCommunityOutboxEventPublisher publisher =
            await RabbitMqCommunityOutboxEventPublisher.CreateAsync(publisherOptions, token);
        while (!token.IsCancellationRequested)
        {
            await RunOnceAsync(publisher, token);
            await DelayAsync(token);
        }
    }

    private async Task RunOnceAsync(ICommunityOutboxEventPublisher publisher,
        CancellationToken token)
    {
        DbContextOptions<CommunityPersistenceDbContext> contextOptions =
            new DbContextOptionsBuilder<CommunityPersistenceDbContext>()
                .UseNpgsql(options.CommunityDatabase,
                    npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "community"))
                .Options;
        await using var context = new CommunityPersistenceDbContext(contextOptions);
        var store = new PostgreSqlCommunityOutboxRelayStore(context);
        var policy = new CommunityOutboxRelayRetryPolicy(options.RetryInitialDelay,
            options.RetryMaximumDelay, options.AutomaticAttemptLimit);
        IReadOnlyList<CommunityOutboxRelayClaim> claims = await store.ClaimEligibleAsync(
            workerId, DateTimeOffset.UtcNow, options.LeaseDuration, options.BatchSize, token);
        var processor = new CommunityOutboxRelayProcessor(
            store, publisher,
            loggerFactory.CreateLogger<CommunityOutboxRelayProcessor>());
        foreach (CommunityOutboxRelayClaim claim in claims)
            await processor.ProcessAsync(claim, workerId, DateTimeOffset.UtcNow, policy, token);
    }

    private async Task DelayAsync(CancellationToken token)
    {
        try { await Task.Delay(options.PollInterval, token); }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
    }
}
