using HotJoes.Infrastructure.Persistence;
using HotJoes.Infrastructure.VendorRelay;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Worker.VendorRelay;

public sealed class VendorRelayBackgroundService : BackgroundService
{
    private readonly VendorRelayHostOptions _options;
    private readonly ILogger<VendorRelayBackgroundService> _logger;
    private readonly Guid _workerId = Guid.NewGuid();

    public VendorRelayBackgroundService(
        VendorRelayHostOptions options,
        ILogger<VendorRelayBackgroundService> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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
                    "Vendor relay dependency connection is unavailable.");
            }

            await DelayAsync(stoppingToken);
        }
    }

    private async Task RunConnectedAsync(CancellationToken stoppingToken)
    {
        var publisherOptions = new RabbitMqPublisherOptions(
            _options.RabbitMq,
            _options.ExchangeName,
            _options.ExchangeType,
            _options.QueueName,
            _options.RoutingKey);
        await using RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(
                publisherOptions,
                stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunOnceAsync(publisher, stoppingToken);
            await DelayAsync(stoppingToken);
        }
    }

    private async Task RunOnceAsync(
        IOutboxEventPublisher publisher,
        CancellationToken stoppingToken)
    {
        DbContextOptions<VendorRegistrationDbContext> contextOptions =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_options.VendorDatabase)
                .Options;
        await using var context =
            new VendorRegistrationDbContext(contextOptions);
        var store = new PostgreSqlOutboxRelayStore(context);
        var retryPolicy = new OutboxRelayRetryPolicy(
            _options.RetryInitialDelay,
            _options.RetryMaximumDelay,
            _options.AutomaticAttemptLimit);
        var runner = new VendorOutboxRelayRunner(
            store,
            publisher,
            _workerId,
            _options.LeaseDuration,
            _options.BatchSize,
            retryPolicy);

        await runner.RunOnceAsync(DateTimeOffset.UtcNow, stoppingToken);
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
