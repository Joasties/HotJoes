using Testcontainers.RabbitMq;

namespace HotJoes.IntegrationTests;

public sealed class RabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _container =
        new RabbitMqBuilder("rabbitmq:4.3.5-management-alpine")
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task StopAsync()
    {
        return _container.StopAsync();
    }

    public Task StartAsync()
    {
        return _container.StartAsync();
    }

    public async Task RestartAsync()
    {
        await StopAsync();
        await StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }
}
