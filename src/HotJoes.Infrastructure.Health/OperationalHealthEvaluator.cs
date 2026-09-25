namespace HotJoes.Infrastructure.Health;

public sealed class OperationalHealthEvaluator
{
    private static readonly IReadOnlyDictionary<
        OperationalComponent,
        HealthDependency[]> ReadinessDependencies =
        new Dictionary<OperationalComponent, HealthDependency[]>
        {
            [OperationalComponent.VendorApi] =
                [HealthDependency.VendorPostgreSql],
            [OperationalComponent.VendorRelay] =
                [
                    HealthDependency.VendorPostgreSql,
                    HealthDependency.RabbitMq
                ],
            [OperationalComponent.ComplianceConsumer] =
                [
                    HealthDependency.RabbitMq,
                    HealthDependency.CompliancePostgreSql
                ],
            [OperationalComponent.CommunityConsumer] =
                [
                    HealthDependency.RabbitMq,
                    HealthDependency.CommunityPostgreSql
                ],
            [OperationalComponent.CommunityRelay] =
                [
                    HealthDependency.CommunityPostgreSql,
                    HealthDependency.RabbitMq
                ]
        };

    private readonly IReadOnlyDictionary<
        HealthDependency,
        IHealthDependencyProbe> _dependencyProbes;
    private readonly IReadOnlyDictionary<
        OperationalComponent,
        IReadOnlyList<IDegradedWorkProbe>> _degradedWorkProbes;

    public OperationalHealthEvaluator(
        IEnumerable<IHealthDependencyProbe> dependencyProbes)
        : this(dependencyProbes, [])
    {
    }

    public OperationalHealthEvaluator(
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
        OperationalComponent component)
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
        OperationalComponent component,
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
            OperationalComponent component,
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
            OperationalComponent component,
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
        OperationalComponent,
        IReadOnlyList<IDegradedWorkProbe>> RegisterDegradedWorkProbes(
            IEnumerable<IDegradedWorkProbe> probes)
    {
        var registered = new Dictionary<
            OperationalComponent,
            List<IDegradedWorkProbe>>();
        var registrations = new HashSet<
            (OperationalComponent Component, DegradedWorkKind Kind)>();

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
                probe.Component == OperationalComponent.VendorRelay,
            DegradedWorkKind.ComplianceDeadLetter =>
                probe.Component == OperationalComponent.ComplianceConsumer,
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

    private static void ValidateComponent(OperationalComponent component)
    {
        if (!ReadinessDependencies.ContainsKey(component))
        {
            throw new ArgumentOutOfRangeException(nameof(component));
        }
    }
}
