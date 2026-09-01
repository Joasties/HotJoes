using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace HotJoes.Infrastructure.Health;

public sealed class RabbitMqHealthDependencyProbe
    : IHealthDependencyProbe
{
    private readonly Uri _connectionUri;

    public RabbitMqHealthDependencyProbe(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "RabbitMQ connection string must be supplied.",
                nameof(connectionString));
        }

        _connectionUri = new Uri(connectionString, UriKind.Absolute);
    }

    public HealthDependency Dependency => HealthDependency.RabbitMq;

    public async Task<DependencyAvailability> CheckAvailabilityAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var factory = new ConnectionFactory
            {
                Uri = _connectionUri,
                AutomaticRecoveryEnabled = false
            };

            await using IConnection connection =
                await factory.CreateConnectionAsync(cancellationToken);
            await using IChannel channel =
                await connection.CreateChannelAsync(
                    cancellationToken: cancellationToken);

            return DependencyAvailability.Available;
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            return DependencyAvailability.Unavailable;
        }
        catch (BrokerUnreachableException)
        {
            return DependencyAvailability.Unavailable;
        }
        catch (ConnectFailureException)
        {
            return DependencyAvailability.Unavailable;
        }
        catch (OperationInterruptedException)
        {
            return DependencyAvailability.Unavailable;
        }
        catch (IOException)
        {
            return DependencyAvailability.Unavailable;
        }
        catch (TimeoutException)
        {
            return DependencyAvailability.Unavailable;
        }
    }
}
