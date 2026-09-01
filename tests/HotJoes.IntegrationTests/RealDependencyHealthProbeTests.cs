using System.Reflection;
using HotJoes.Infrastructure.Health;
using Npgsql;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(HealthDependencyIntegrationCollection.Name)]
public sealed class RealDependencyHealthProbeTests
{
    private readonly HealthDependencyFixture _fixture;

    public RealDependencyHealthProbeTests(
        HealthDependencyFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PostgreSqlProbe_Available_PreservesExistingState()
    {
        await using (var connection = new NpgsqlConnection(
            _fixture.PostgreSqlConnectionString))
        {
            await connection.OpenAsync();
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS health_probe_marker
                (
                    marker integer PRIMARY KEY
                );
                TRUNCATE TABLE health_probe_marker;
                INSERT INTO health_probe_marker (marker) VALUES (42);
                """;
            await command.ExecuteNonQueryAsync();
        }

        var probe = new PostgreSqlHealthDependencyProbe(
            HealthDependency.VendorPostgreSql,
            _fixture.PostgreSqlConnectionString);

        DependencyAvailability availability =
            await probe.CheckAvailabilityAsync();

        Assert.Equal(DependencyAvailability.Available, availability);
        await using var verificationConnection = new NpgsqlConnection(
            _fixture.PostgreSqlConnectionString);
        await verificationConnection.OpenAsync();
        await using NpgsqlCommand verification =
            verificationConnection.CreateCommand();
        verification.CommandText =
            "SELECT marker FROM health_probe_marker;";
        Assert.Equal(42, await verification.ExecuteScalarAsync());
    }

    [Fact]
    public async Task RabbitMqProbe_Available_PreservesQueuedMessage()
    {
        string queueName = $"hotjoes.health.probe.{Guid.NewGuid():N}";
        await using IConnection connection = await CreateRabbitConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: true,
            basicProperties: new BasicProperties
            {
                Persistent = true
            },
            body: new byte[] { 4, 2 });
        var probe = new RabbitMqHealthDependencyProbe(
            _fixture.RabbitMqConnectionString);

        DependencyAvailability availability =
            await probe.CheckAvailabilityAsync();

        Assert.Equal(DependencyAvailability.Available, availability);
        QueueDeclareOk queue = await channel.QueueDeclarePassiveAsync(
            queueName);
        Assert.Equal(1u, queue.MessageCount);
        await channel.QueueDeleteAsync(queueName);
    }

    [Fact]
    public async Task DependencyOutageMatrix_ReportsResponsibilityAndRecovers()
    {
        Epic1HealthEvaluator evaluator = CreateEvaluator();

        await AssertAllReadyAsync(evaluator);

        await _fixture.StopRabbitMqAsync();
        try
        {
            AssertHealthy(await evaluator.EvaluateReadinessAsync(
                Epic1Component.VendorApi));
            AssertUnhealthy(
                await evaluator.EvaluateReadinessAsync(
                    Epic1Component.VendorRelay),
                HealthDependency.RabbitMq);
            AssertUnhealthy(
                await evaluator.EvaluateReadinessAsync(
                    Epic1Component.ComplianceConsumer),
                HealthDependency.RabbitMq);
        }
        finally
        {
            await _fixture.StartRabbitMqAsync();
            await WaitUntilAvailableAsync(
                new RabbitMqHealthDependencyProbe(
                    _fixture.RabbitMqConnectionString));
        }

        evaluator = CreateEvaluator();
        await AssertAllReadyAsync(evaluator);

        await _fixture.StopPostgreSqlAsync();
        try
        {
            AssertUnhealthy(
                await evaluator.EvaluateReadinessAsync(
                    Epic1Component.VendorApi),
                HealthDependency.VendorPostgreSql);
            AssertUnhealthy(
                await evaluator.EvaluateReadinessAsync(
                    Epic1Component.VendorRelay),
                HealthDependency.VendorPostgreSql);
            AssertUnhealthy(
                await evaluator.EvaluateReadinessAsync(
                    Epic1Component.ComplianceConsumer),
                HealthDependency.CompliancePostgreSql);
        }
        finally
        {
            await _fixture.StartPostgreSqlAsync();
            await WaitUntilAvailableAsync(
                new PostgreSqlHealthDependencyProbe(
                    HealthDependency.VendorPostgreSql,
                    _fixture.PostgreSqlConnectionString));
        }

        evaluator = CreateEvaluator();
        await AssertAllReadyAsync(evaluator);
    }

    [Fact]
    public async Task DependencyProbes_CancelledRequest_PreservesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var postgreSql = new PostgreSqlHealthDependencyProbe(
            HealthDependency.VendorPostgreSql,
            _fixture.PostgreSqlConnectionString);
        var rabbitMq = new RabbitMqHealthDependencyProbe(
            _fixture.RabbitMqConnectionString);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            postgreSql.CheckAvailabilityAsync(cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            rabbitMq.CheckAvailabilityAsync(cancellation.Token));
    }

    [Theory]
    [InlineData(typeof(PostgreSqlHealthDependencyProbe))]
    [InlineData(typeof(RabbitMqHealthDependencyProbe))]
    public void DependencyProbe_PublicSurface_ExposesNoConnectionOrException(
        Type probeType)
    {
        PropertyInfo property = Assert.Single(probeType.GetProperties());

        Assert.Equal("Dependency", property.Name);
        Assert.Equal(typeof(HealthDependency), property.PropertyType);
        Assert.False(property.SetMethod?.IsPublic ?? false);
        Assert.DoesNotContain(
            probeType.GetProperties(),
            item => item.PropertyType == typeof(Exception));
    }

    private Epic1HealthEvaluator CreateEvaluator()
    {
        return new Epic1HealthEvaluator(
            [
                new PostgreSqlHealthDependencyProbe(
                    HealthDependency.VendorPostgreSql,
                    _fixture.PostgreSqlConnectionString),
                new RabbitMqHealthDependencyProbe(
                    _fixture.RabbitMqConnectionString),
                new PostgreSqlHealthDependencyProbe(
                    HealthDependency.CompliancePostgreSql,
                    _fixture.PostgreSqlConnectionString)
            ]);
    }

    private async Task<IConnection> CreateRabbitConnectionAsync()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_fixture.RabbitMqConnectionString),
            AutomaticRecoveryEnabled = false
        };

        return await factory.CreateConnectionAsync();
    }

    private static async Task WaitUntilAvailableAsync(
        IHealthDependencyProbe probe)
    {
        const int maximumAttempts = 50;
        const int requiredConsecutiveSuccesses = 3;
        int consecutiveSuccesses = 0;

        for (int attempt = 1; attempt <= maximumAttempts; attempt++)
        {
            if (await probe.CheckAvailabilityAsync() ==
                DependencyAvailability.Available)
            {
                consecutiveSuccesses++;

                if (consecutiveSuccesses == requiredConsecutiveSuccesses)
                {
                    return;
                }
            }
            else
            {
                consecutiveSuccesses = 0;
            }

            if (attempt < maximumAttempts)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
        }

        Assert.Fail(
            $"Dependency '{probe.Dependency}' did not recover in time.");
    }

    private static async Task AssertAllReadyAsync(
        Epic1HealthEvaluator evaluator)
    {
        AssertHealthy(await evaluator.EvaluateReadinessAsync(
            Epic1Component.VendorApi));
        AssertHealthy(await evaluator.EvaluateReadinessAsync(
            Epic1Component.VendorRelay));
        AssertHealthy(await evaluator.EvaluateReadinessAsync(
            Epic1Component.ComplianceConsumer));
    }

    private static void AssertHealthy(OperationalHealthEvidence evidence)
    {
        Assert.Equal(OperationalHealthStatus.Healthy, evidence.Status);
        Assert.Empty(evidence.UnavailableDependencies);
    }

    private static void AssertUnhealthy(
        OperationalHealthEvidence evidence,
        params HealthDependency[] unavailableDependencies)
    {
        Assert.Equal(OperationalHealthStatus.Unhealthy, evidence.Status);
        Assert.Equal(
            unavailableDependencies,
            evidence.UnavailableDependencies);
    }
}
