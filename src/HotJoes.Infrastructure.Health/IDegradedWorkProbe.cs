namespace HotJoes.Infrastructure.Health;

public interface IDegradedWorkProbe
{
    Epic1Component Component { get; }

    DegradedWorkKind Kind { get; }

    Task<bool> HasDegradedWorkAsync(
        CancellationToken cancellationToken = default);
}
