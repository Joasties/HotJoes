namespace HotJoes.Infrastructure.Health;

public sealed class Epic1HealthEvaluator
{
    private static readonly IReadOnlyDictionary<
        Epic1Component,
        HealthDependency[]> ReadinessDependencies =
        new Dictionary<Epic1Component, HealthDependency[]>
        {
            [Epic1Component.VendorApi] =
                [HealthDependency.VendorPostgreSql],
            [Epic1Component.VendorRelay] =
                [
                    HealthDependency.VendorPostgreSql,
                    HealthDependency.RabbitMq
                ],
            [Epic1Component.ComplianceConsumer] =
                [
                    HealthDependency.RabbitMq,
                    HealthDependency.CompliancePostgreSql
                ]
        };

    private readonly IReadOnlyDictionary<
        HealthDependency,
        IHealthDependencyProbe> _dependencyProbes;
    private readonly IReadOnlyDictionary<
        Epic1Component,
        IReadOnlyList<IDegradedWorkProbe>> _degradedWorkProbes;

    public Epic1HealthEvaluator(
        IEnumerable<IHealthDependencyProbe> dependencyProbes)
        : this(dependencyProbes, [])
    {
    }

    public Epic1HealthEvaluator(
        IEnumerable<IHealthDependencyProbe> dependencyProbes,
        IEnumerable<IDegradedWorkProbe> degradedWorkProbes)
    {
        ArgumentNullException.ThrowIfNull(dependencyProbes);
        ArgumentNullException.ThrowIfNull(degradedWorkProbes);

        _dependencyProbes = RegisterDependencyProbes(dependencyProbes);
        _degradedWorkProbes = RegisterDegradedWorkProbes(
            degradedWorkProbes);
    }

    public OperationalHealthEvidence EvaluateLiveness(
        Epic1Component component)
    {
        ValidateComponent(component);

        return new OperationalHealthEvidence(
            OperationalHealthCheckKind.Liveness,
            component,
            OperationalHealthStatus.Healthy,
            [],
            []);
    }

    public async Task<OperationalHealthEvidence> EvaluateReadinessAsync(
        Epic1Component component,
        CancellationToken cancellationToken = default)
    {
        ValidateComponent(component);

        IReadOnlyList<HealthDependency> unavailable =
            await FindUnavailableDependenciesAsync(
                component,
                cancellationToken);

        if (unavailable.Count > 0)
        {
            return new OperationalHealthEvidence(
                OperationalHealthCheckKind.Readiness,
                component,
                OperationalHealthStatus.Unhealthy,
                unavailable,
                []);
        }

        IReadOnlyList<DegradedWorkKind> degradedWork =
            await FindDegradedWorkAsync(component, cancellationToken);

        return new OperationalHealthEvidence(
            OperationalHealthCheckKind.Readiness,
            component,
            degradedWork.Count == 0
                ? OperationalHealthStatus.Healthy
                : OperationalHealthStatus.Degraded,
            [],
            degradedWork);
    }

    private async Task<IReadOnlyList<HealthDependency>>
        FindUnavailableDependenciesAsync(
            Epic1Component component,
            CancellationToken cancellationToken)
    {
        var unavailable = new List<HealthDependency>();

        foreach (HealthDependency dependency in
            ReadinessDependencies[component])
        {
            if (!_dependencyProbes.TryGetValue(
                    dependency,
                    out IHealthDependencyProbe? probe))
            {
                throw new InvalidOperationException(
                    $"Required health dependency '{dependency}' has no " +
                    "registered probe.");
            }

            DependencyAvailability availability =
                await probe.CheckAvailabilityAsync(cancellationToken);

            if (!Enum.IsDefined(availability))
            {
                throw new InvalidOperationException(
                    $"Dependency '{dependency}' returned an unsupported " +
                    "availability value.");
            }

            if (availability == DependencyAvailability.Unavailable)
            {
                unavailable.Add(dependency);
            }
        }

        return unavailable;
    }

    private async Task<IReadOnlyList<DegradedWorkKind>>
        FindDegradedWorkAsync(
            Epic1Component component,
            CancellationToken cancellationToken)
    {
        if (!_degradedWorkProbes.TryGetValue(
                component,
                out IReadOnlyList<IDegradedWorkProbe>? probes))
        {
            return [];
        }

        var degradedWork = new List<DegradedWorkKind>();

        foreach (IDegradedWorkProbe probe in probes)
        {
            if (await probe.HasDegradedWorkAsync(cancellationToken))
            {
                degradedWork.Add(probe.Kind);
            }
        }

        return degradedWork;
    }

    private static IReadOnlyDictionary<
        HealthDependency,
        IHealthDependencyProbe> RegisterDependencyProbes(
            IEnumerable<IHealthDependencyProbe> probes)
    {
        var registered = new Dictionary<
            HealthDependency,
            IHealthDependencyProbe>();

        foreach (IHealthDependencyProbe probe in probes)
        {
            ArgumentNullException.ThrowIfNull(probe);

            if (!Enum.IsDefined(probe.Dependency))
            {
                throw new ArgumentOutOfRangeException(nameof(probes));
            }

            if (!registered.TryAdd(probe.Dependency, probe))
            {
                throw new ArgumentException(
                    $"Dependency '{probe.Dependency}' is registered twice.",
                    nameof(probes));
            }
        }

        return registered;
    }

    private static IReadOnlyDictionary<
        Epic1Component,
        IReadOnlyList<IDegradedWorkProbe>> RegisterDegradedWorkProbes(
            IEnumerable<IDegradedWorkProbe> probes)
    {
        var registered = new Dictionary<
            Epic1Component,
            List<IDegradedWorkProbe>>();
        var registrations = new HashSet<
            (Epic1Component Component, DegradedWorkKind Kind)>();

        foreach (IDegradedWorkProbe probe in probes)
        {
            ArgumentNullException.ThrowIfNull(probe);
            ValidateDegradedWorkProbe(probe);

            if (!registrations.Add((probe.Component, probe.Kind)))
            {
                throw new ArgumentException(
                    $"Degraded work '{probe.Kind}' for '{probe.Component}' " +
                    "is registered twice.",
                    nameof(probes));
            }

            if (!registered.TryGetValue(
                    probe.Component,
                    out List<IDegradedWorkProbe>? componentProbes))
            {
                componentProbes = [];
                registered.Add(probe.Component, componentProbes);
            }

            componentProbes.Add(probe);
        }

        return registered.ToDictionary(
            item => item.Key,
            item => (IReadOnlyList<IDegradedWorkProbe>)item.Value.AsReadOnly());
    }

    private static void ValidateDegradedWorkProbe(
        IDegradedWorkProbe probe)
    {
        if (!Enum.IsDefined(probe.Component) ||
            !Enum.IsDefined(probe.Kind))
        {
            throw new ArgumentOutOfRangeException(nameof(probe));
        }

        bool validPair = probe.Kind switch
        {
            DegradedWorkKind.StalledOutbox =>
                probe.Component == Epic1Component.VendorRelay,
            DegradedWorkKind.ComplianceDeadLetter =>
                probe.Component == Epic1Component.ComplianceConsumer,
            _ => false
        };

        if (!validPair)
        {
            throw new ArgumentException(
                $"Degraded work '{probe.Kind}' does not belong to " +
                $"'{probe.Component}'.",
                nameof(probe));
        }
    }

    private static void ValidateComponent(Epic1Component component)
    {
        if (!ReadinessDependencies.ContainsKey(component))
        {
            throw new ArgumentOutOfRangeException(nameof(component));
        }
    }
}
