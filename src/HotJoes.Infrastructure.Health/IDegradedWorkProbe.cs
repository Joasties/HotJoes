namespace HotJoes.Infrastructure.Health;

public interface IDegradedWorkProbe
{
    OperationalComponent Component { get; }

    DegradedWorkKind Kind { get; }

    Task<bool> HasDegradedWorkAsync(
        CancellationToken cancellationToken = default);
}
