using System.Collections.ObjectModel;

namespace HotJoes.Infrastructure.Health;

public sealed class OperationalHealthEvidence
{
    private readonly ReadOnlyCollection<HealthDependency>
        _unavailableDependencies;
    private readonly ReadOnlyCollection<DegradedWorkKind> _degradedWork;

    internal OperationalHealthEvidence(
        OperationalHealthCheckKind checkKind,
        Epic1Component component,
        OperationalHealthStatus status,
        IEnumerable<HealthDependency> unavailableDependencies,
        IEnumerable<DegradedWorkKind> degradedWork)
    {
        CheckKind = checkKind;
        Component = component;
        Status = status;
        _unavailableDependencies = Array.AsReadOnly(
            unavailableDependencies.ToArray());
        _degradedWork = Array.AsReadOnly(degradedWork.ToArray());
    }

    public OperationalHealthCheckKind CheckKind { get; }

    public Epic1Component Component { get; }

    public OperationalHealthStatus Status { get; }

    public IReadOnlyList<HealthDependency> UnavailableDependencies =>
        _unavailableDependencies;

    public IReadOnlyList<DegradedWorkKind> DegradedWork => _degradedWork;
}
