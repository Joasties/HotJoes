using Npgsql;

namespace HotJoes.Infrastructure.Health;

public sealed class PostgreSqlHealthDependencyProbe
    : IHealthDependencyProbe
{
    private readonly string _connectionString;

    public PostgreSqlHealthDependencyProbe(
        HealthDependency dependency,
        string connectionString)
    {
        if (dependency is not HealthDependency.VendorPostgreSql and
            not HealthDependency.CompliancePostgreSql)
        {
            throw new ArgumentOutOfRangeException(nameof(dependency));
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "PostgreSQL connection string must be supplied.",
                nameof(connectionString));
        }

        Dependency = dependency;
        _connectionString = new NpgsqlConnectionStringBuilder(
            connectionString).ConnectionString;
    }

    public HealthDependency Dependency { get; }

    public async Task<DependencyAvailability> CheckAvailabilityAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await using var connection = new NpgsqlConnection(
                _connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new NpgsqlCommand(
                "SELECT 1;",
                connection);
            await command.ExecuteScalarAsync(cancellationToken);

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
        catch (NpgsqlException)
        {
            return DependencyAvailability.Unavailable;
        }
        catch (TimeoutException)
        {
            return DependencyAvailability.Unavailable;
        }
    }
}
