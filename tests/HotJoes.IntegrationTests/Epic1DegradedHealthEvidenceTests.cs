using System.Reflection;
using HotJoes.Infrastructure.Health;

namespace HotJoes.IntegrationTests;

public sealed class Epic1DegradedHealthEvidenceTests
{
    [Fact]
    public async Task StalledOutboxWork_DegradesRelayReadinessButNotLiveness()
    {
        var stalled = new RecordingDegradedWorkProbe(
            Epic1Component.VendorRelay,
            DegradedWorkKind.StalledOutbox,
            hasDegradedWork: true);
        var evaluator = CreateEvaluator(stalled);

        OperationalHealthEvidence liveness =
            evaluator.EvaluateLiveness(Epic1Component.VendorRelay);
        OperationalHealthEvidence readiness =
            await evaluator.EvaluateReadinessAsync(
                Epic1Component.VendorRelay);

        AssertHealthy(liveness, OperationalHealthCheckKind.Liveness);
        Assert.Equal(1, stalled.CallCount);
        AssertDegraded(
            readiness,
            Epic1Component.VendorRelay,
            DegradedWorkKind.StalledOutbox);
    }

    [Fact]
    public async Task DeadLetterWork_DegradesConsumerReadinessButNotLiveness()
    {
        var deadLetter = new RecordingDegradedWorkProbe(
            Epic1Component.ComplianceConsumer,
            DegradedWorkKind.ComplianceDeadLetter,
            hasDegradedWork: true);
        var evaluator = CreateEvaluator(deadLetter);

        OperationalHealthEvidence liveness =
            evaluator.EvaluateLiveness(
                Epic1Component.ComplianceConsumer);
        OperationalHealthEvidence readiness =
            await evaluator.EvaluateReadinessAsync(
                Epic1Component.ComplianceConsumer);

        AssertHealthy(liveness, OperationalHealthCheckKind.Liveness);
        Assert.Equal(1, deadLetter.CallCount);
        AssertDegraded(
            readiness,
            Epic1Component.ComplianceConsumer,
            DegradedWorkKind.ComplianceDeadLetter);
    }

    [Fact]
    public async Task DegradedWork_ForAnotherResponsibility_IsNotInspected()
    {
        var stalled = new RecordingDegradedWorkProbe(
            Epic1Component.VendorRelay,
            DegradedWorkKind.StalledOutbox,
            hasDegradedWork: true);
        var deadLetter = new RecordingDegradedWorkProbe(
            Epic1Component.ComplianceConsumer,
            DegradedWorkKind.ComplianceDeadLetter,
            hasDegradedWork: true);
        var evaluator = CreateEvaluator(stalled, deadLetter);

        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(
                Epic1Component.VendorApi);

        AssertHealthy(evidence, OperationalHealthCheckKind.Readiness);
        Assert.Equal(0, stalled.CallCount);
        Assert.Equal(0, deadLetter.CallCount);
    }

    [Fact]
    public async Task UnavailableRequiredDependency_RemainsUnhealthyNotDegraded()
    {
        var stalled = new RecordingDegradedWorkProbe(
            Epic1Component.VendorRelay,
            DegradedWorkKind.StalledOutbox,
            hasDegradedWork: true);
        var evaluator = CreateEvaluator(
            [
                new RecordingDependencyProbe(
                    HealthDependency.VendorPostgreSql,
                    DependencyAvailability.Available),
                new RecordingDependencyProbe(
                    HealthDependency.RabbitMq,
                    DependencyAvailability.Unavailable),
                new RecordingDependencyProbe(
                    HealthDependency.CompliancePostgreSql,
                    DependencyAvailability.Available)
            ],
            stalled);

        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(
                Epic1Component.VendorRelay);

        Assert.Equal(OperationalHealthStatus.Unhealthy, evidence.Status);
        Assert.Equal(
            [HealthDependency.RabbitMq],
            evidence.UnavailableDependencies);
        Assert.Empty(evidence.DegradedWork);
        Assert.Equal(0, stalled.CallCount);
    }

    [Fact]
    public async Task HealthEvaluation_ObservesButDoesNotChangeDurableWork()
    {
        byte[] originalState = [4, 2, 8, 1, 5, 7];
        var stalled = new RecordingDegradedWorkProbe(
            Epic1Component.VendorRelay,
            DegradedWorkKind.StalledOutbox,
            hasDegradedWork: true,
            durableState: originalState);
        var evaluator = CreateEvaluator(stalled);

        await evaluator.EvaluateReadinessAsync(Epic1Component.VendorRelay);
        await evaluator.EvaluateReadinessAsync(Epic1Component.VendorRelay);

        Assert.Equal(2, stalled.CallCount);
        Assert.Equal(originalState, stalled.DurableState);
    }

    [Fact]
    public void OperationalHealthEvidence_PublicSurface_IsClosedAndSafe()
    {
        PropertyInfo[] properties =
            typeof(OperationalHealthEvidence).GetProperties();

        Assert.Equal(
            new[]
            {
                "CheckKind",
                "Component",
                "DegradedWork",
                "Status",
                "UnavailableDependencies"
            },
            properties
                .Select(property => property.Name)
                .Order(StringComparer.Ordinal)
                .ToArray());
        Assert.All(
            properties,
            property => Assert.False(
                property.SetMethod?.IsPublic ?? false));
        Assert.Equal(
            typeof(IReadOnlyList<DegradedWorkKind>),
            Assert.Single(
                properties,
                property => property.Name == "DegradedWork")
                .PropertyType);
        Assert.DoesNotContain(
            properties,
            property => property.PropertyType == typeof(Exception));
        Assert.DoesNotContain(
            properties,
            property => ContainsUnsafeName(property.Name));
    }

    [Fact]
    public void DegradedWorkKind_IsClosedLowCardinalityClassification()
    {
        Assert.Equal(
            new[]
            {
                nameof(DegradedWorkKind.StalledOutbox),
                nameof(DegradedWorkKind.ComplianceDeadLetter)
            },
            Enum.GetNames<DegradedWorkKind>());
    }

    private static Epic1HealthEvaluator CreateEvaluator(
        params IDegradedWorkProbe[] degradedWorkProbes)
    {
        return CreateEvaluator(
            [
                new RecordingDependencyProbe(
                    HealthDependency.VendorPostgreSql,
                    DependencyAvailability.Available),
                new RecordingDependencyProbe(
                    HealthDependency.RabbitMq,
                    DependencyAvailability.Available),
                new RecordingDependencyProbe(
                    HealthDependency.CompliancePostgreSql,
                    DependencyAvailability.Available)
            ],
            degradedWorkProbes);
    }

    private static Epic1HealthEvaluator CreateEvaluator(
        IEnumerable<IHealthDependencyProbe> dependencyProbes,
        params IDegradedWorkProbe[] degradedWorkProbes)
    {
        return new Epic1HealthEvaluator(
            dependencyProbes,
            degradedWorkProbes);
    }

    private static void AssertHealthy(
        OperationalHealthEvidence evidence,
        OperationalHealthCheckKind expectedKind)
    {
        Assert.Equal(expectedKind, evidence.CheckKind);
        Assert.Equal(OperationalHealthStatus.Healthy, evidence.Status);
        Assert.Empty(evidence.UnavailableDependencies);
        Assert.Empty(evidence.DegradedWork);
    }

    private static void AssertDegraded(
        OperationalHealthEvidence evidence,
        Epic1Component expectedComponent,
        params DegradedWorkKind[] expectedDegradedWork)
    {
        Assert.Equal(
            OperationalHealthCheckKind.Readiness,
            evidence.CheckKind);
        Assert.Equal(expectedComponent, evidence.Component);
        Assert.Equal(OperationalHealthStatus.Degraded, evidence.Status);
        Assert.Empty(evidence.UnavailableDependencies);
        Assert.Equal(expectedDegradedWork, evidence.DegradedWork);
    }

    private static bool ContainsUnsafeName(string name)
    {
        string[] unsafeTerms =
        [
            "Address",
            "Connection",
            "Contact",
            "Credential",
            "Exception",
            "Message",
            "Payload",
            "Queue",
            "Secret",
            "StackTrace"
        ];

        return unsafeTerms.Any(term => name.Contains(
            term,
            StringComparison.OrdinalIgnoreCase));
    }

    private sealed class RecordingDependencyProbe
        : IHealthDependencyProbe
    {
        private readonly DependencyAvailability _availability;

        public RecordingDependencyProbe(
            HealthDependency dependency,
            DependencyAvailability availability)
        {
            Dependency = dependency;
            _availability = availability;
        }

        public HealthDependency Dependency { get; }

        public Task<DependencyAvailability> CheckAvailabilityAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_availability);
        }
    }

    private sealed class RecordingDegradedWorkProbe
        : IDegradedWorkProbe
    {
        private readonly bool _hasDegradedWork;
        private readonly byte[] _durableState;

        public RecordingDegradedWorkProbe(
            Epic1Component component,
            DegradedWorkKind kind,
            bool hasDegradedWork,
            byte[]? durableState = null)
        {
            Component = component;
            Kind = kind;
            _hasDegradedWork = hasDegradedWork;
            _durableState = durableState?.ToArray() ?? [];
        }

        public Epic1Component Component { get; }

        public DegradedWorkKind Kind { get; }

        public int CallCount { get; private set; }

        public IReadOnlyList<byte> DurableState => _durableState;

        public Task<bool> HasDegradedWorkAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return Task.FromResult(_hasDegradedWork);
        }
    }
}
