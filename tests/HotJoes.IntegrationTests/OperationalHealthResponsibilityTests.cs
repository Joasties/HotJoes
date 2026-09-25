using System.Reflection;
using HotJoes.Infrastructure.Health;

namespace HotJoes.IntegrationTests;

public sealed class OperationalHealthResponsibilityTests
{
    [Theory]
    [InlineData(OperationalComponent.VendorApi)]
    [InlineData(OperationalComponent.VendorRelay)]
    [InlineData(OperationalComponent.ComplianceConsumer)]
    public void EvaluateLiveness_ComponentIsRunning_UsesNoDependency(
        OperationalComponent component)
    {
        var probes = CreateProbes(
            DependencyAvailability.Unavailable,
            DependencyAvailability.Unavailable,
            DependencyAvailability.Unavailable);
        var evaluator = new OperationalHealthEvaluator(probes.All);

        OperationalHealthEvidence evidence =
            evaluator.EvaluateLiveness(component);

        Assert.Equal(OperationalHealthCheckKind.Liveness, evidence.CheckKind);
        Assert.Equal(component, evidence.Component);
        Assert.Equal(OperationalHealthStatus.Healthy, evidence.Status);
        Assert.Empty(evidence.UnavailableDependencies);
        Assert.Empty(evidence.DegradedWork);
        Assert.All(probes.All, probe => Assert.Equal(0, probe.CallCount));
    }

    [Fact]
    public async Task EvaluateReadiness_VendorApi_RequiresOnlyVendorPostgreSql()
    {
        var probes = CreateProbes(
            DependencyAvailability.Available,
            DependencyAvailability.Unavailable,
            DependencyAvailability.Unavailable);
        var evaluator = new OperationalHealthEvaluator(probes.All);

        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(OperationalComponent.VendorApi);

        AssertReady(evidence, OperationalComponent.VendorApi);
        Assert.Equal(1, probes.VendorPostgreSql.CallCount);
        Assert.Equal(0, probes.RabbitMq.CallCount);
        Assert.Equal(0, probes.CompliancePostgreSql.CallCount);
    }

    [Fact]
    public async Task EvaluateReadiness_VendorApiPostgreSqlUnavailable_IsUnhealthy()
    {
        var probes = CreateProbes(
            DependencyAvailability.Unavailable,
            DependencyAvailability.Available,
            DependencyAvailability.Available);
        var evaluator = new OperationalHealthEvaluator(probes.All);

        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(OperationalComponent.VendorApi);

        AssertUnhealthy(
            evidence,
            OperationalComponent.VendorApi,
            HealthDependency.VendorPostgreSql);
        Assert.Equal(1, probes.VendorPostgreSql.CallCount);
        Assert.Equal(0, probes.RabbitMq.CallCount);
        Assert.Equal(0, probes.CompliancePostgreSql.CallCount);
    }

    [Fact]
    public async Task EvaluateReadiness_VendorRelay_RequiresVendorPostgreSqlAndRabbitMq()
    {
        var probes = CreateProbes(
            DependencyAvailability.Unavailable,
            DependencyAvailability.Unavailable,
            DependencyAvailability.Available);
        var evaluator = new OperationalHealthEvaluator(probes.All);

        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(OperationalComponent.VendorRelay);

        AssertUnhealthy(
            evidence,
            OperationalComponent.VendorRelay,
            HealthDependency.VendorPostgreSql,
            HealthDependency.RabbitMq);
        Assert.Equal(1, probes.VendorPostgreSql.CallCount);
        Assert.Equal(1, probes.RabbitMq.CallCount);
        Assert.Equal(0, probes.CompliancePostgreSql.CallCount);
    }

    [Fact]
    public async Task EvaluateReadiness_ComplianceConsumer_RequiresReceiptPostgreSqlAndRabbitMq()
    {
        var probes = CreateProbes(
            DependencyAvailability.Unavailable,
            DependencyAvailability.Unavailable,
            DependencyAvailability.Unavailable);
        var evaluator = new OperationalHealthEvaluator(probes.All);

        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(
                OperationalComponent.ComplianceConsumer);

        AssertUnhealthy(
            evidence,
            OperationalComponent.ComplianceConsumer,
            HealthDependency.RabbitMq,
            HealthDependency.CompliancePostgreSql);
        Assert.Equal(0, probes.VendorPostgreSql.CallCount);
        Assert.Equal(1, probes.RabbitMq.CallCount);
        Assert.Equal(1, probes.CompliancePostgreSql.CallCount);
    }

    [Fact]
    public async Task EvaluateReadiness_AllRequiredDependenciesAvailable_IsHealthy()
    {
        var probes = CreateProbes(
            DependencyAvailability.Available,
            DependencyAvailability.Available,
            DependencyAvailability.Available);
        var evaluator = new OperationalHealthEvaluator(probes.All);

        OperationalHealthEvidence api =
            await evaluator.EvaluateReadinessAsync(OperationalComponent.VendorApi);
        OperationalHealthEvidence relay =
            await evaluator.EvaluateReadinessAsync(OperationalComponent.VendorRelay);
        OperationalHealthEvidence consumer =
            await evaluator.EvaluateReadinessAsync(
                OperationalComponent.ComplianceConsumer);

        AssertReady(api, OperationalComponent.VendorApi);
        AssertReady(relay, OperationalComponent.VendorRelay);
        AssertReady(consumer, OperationalComponent.ComplianceConsumer);
    }

    [Fact]
    public void OperationalHealthEvidence_PublicSurface_IsImmutableAndSafe()
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
            property => Assert.False(property.SetMethod?.IsPublic ?? false));
        Assert.Equal(
            typeof(IReadOnlyList<HealthDependency>),
            Assert.Single(
                properties,
                property => property.Name == "UnavailableDependencies")
                .PropertyType);
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
            property => property.Name.Contains(
                "Connection",
                StringComparison.OrdinalIgnoreCase));
    }

    private static ProbeSet CreateProbes(
        DependencyAvailability vendorPostgreSql,
        DependencyAvailability rabbitMq,
        DependencyAvailability compliancePostgreSql)
    {
        return new ProbeSet(
            new RecordingDependencyProbe(
                HealthDependency.VendorPostgreSql,
                vendorPostgreSql),
            new RecordingDependencyProbe(
                HealthDependency.RabbitMq,
                rabbitMq),
            new RecordingDependencyProbe(
                HealthDependency.CompliancePostgreSql,
                compliancePostgreSql));
    }

    private static void AssertReady(
        OperationalHealthEvidence evidence,
        OperationalComponent expectedComponent)
    {
        Assert.Equal(OperationalHealthCheckKind.Readiness, evidence.CheckKind);
        Assert.Equal(expectedComponent, evidence.Component);
        Assert.Equal(OperationalHealthStatus.Healthy, evidence.Status);
        Assert.Empty(evidence.UnavailableDependencies);
        Assert.Empty(evidence.DegradedWork);
    }

    private static void AssertUnhealthy(
        OperationalHealthEvidence evidence,
        OperationalComponent expectedComponent,
        params HealthDependency[] expectedUnavailable)
    {
        Assert.Equal(OperationalHealthCheckKind.Readiness, evidence.CheckKind);
        Assert.Equal(expectedComponent, evidence.Component);
        Assert.Equal(OperationalHealthStatus.Unhealthy, evidence.Status);
        Assert.Equal(expectedUnavailable, evidence.UnavailableDependencies);
        Assert.Empty(evidence.DegradedWork);
    }

    private sealed class RecordingDependencyProbe : IHealthDependencyProbe
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

        public int CallCount { get; private set; }

        public Task<DependencyAvailability> CheckAvailabilityAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return Task.FromResult(_availability);
        }
    }

    private sealed record ProbeSet(
        RecordingDependencyProbe VendorPostgreSql,
        RecordingDependencyProbe RabbitMq,
        RecordingDependencyProbe CompliancePostgreSql)
    {
        public IReadOnlyList<RecordingDependencyProbe> All =>
            [VendorPostgreSql, RabbitMq, CompliancePostgreSql];
    }
}
