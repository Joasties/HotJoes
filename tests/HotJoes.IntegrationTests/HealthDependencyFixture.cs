using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace HotJoes.IntegrationTests;

public sealed class HealthDependencyFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSql =
        new PostgreSqlBuilder("postgres:18-alpine")
            .Build();

    private readonly RabbitMqContainer _rabbitMq =
        new RabbitMqBuilder("rabbitmq:4.3.5-management-alpine")
            .Build();

    public string PostgreSqlConnectionString =>
        _postgreSql.GetConnectionString();

    public string RabbitMqConnectionString =>
        _rabbitMq.GetConnectionString();

    public Task InitializeAsync()
    {
        return Task.WhenAll(
            _postgreSql.StartAsync(),
            _rabbitMq.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await _rabbitMq.DisposeAsync();
        await _postgreSql.DisposeAsync();
    }

    public Task StopPostgreSqlAsync()
    {
        return _postgreSql.StopAsync();
    }

    public Task StartPostgreSqlAsync()
    {
        return _postgreSql.StartAsync();
    }

    public Task StopRabbitMqAsync()
    {
        return _rabbitMq.StopAsync();
    }

    public Task StartRabbitMqAsync()
    {
        return _rabbitMq.StartAsync();
    }
}
