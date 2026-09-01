namespace HotJoes.Infrastructure.Health;

public interface IHealthDependencyProbe
{
    HealthDependency Dependency { get; }

    Task<DependencyAvailability> CheckAvailabilityAsync(
        CancellationToken cancellationToken = default);
}
