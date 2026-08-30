using HotJoes.Architecture.Tests;

namespace HotJoes.Architecture.Tests;

public sealed class ProjectDependencyRuleTests
{
    private static readonly string[] ExpectedProductionProjects =
    [
        "HotJoes.Api.Vendor",
        "HotJoes.Application.Address",
        "HotJoes.Application.Vendor",
        "HotJoes.Domain.Vendor",
        "HotJoes.Infrastructure.ComplianceConsumer",
        "HotJoes.Infrastructure.Health",
        "HotJoes.Infrastructure.Persistence",
        "HotJoes.Infrastructure.Vendor.Address",
        "HotJoes.Infrastructure.VendorRelay"
    ];

    [Fact]
    public void AI_ARCH_001_ApprovedSolutionProjectGraph_HasNoViolations()
    {
        ArchitectureRepository repository =
            ArchitectureRepository.FindFromTestAssembly();
        ProjectDependencyGraph graph = repository.LoadProjectGraph();

        IReadOnlyList<ArchitectureViolation> violations =
            ProjectDependencyRuleSet.Evaluate(graph);

        Assert.Empty(violations);
    }

    [Fact]
    public void AI_ARCH_001_ControlledProductionProjectSet_IsComplete()
    {
        ArchitectureRepository repository =
            ArchitectureRepository.FindFromTestAssembly();
        ProjectDependencyGraph graph = repository.LoadProjectGraph();

        Assert.Equal(
            ExpectedProductionProjects.Order(StringComparer.Ordinal),
            graph.ProductionProjects
                .Select(project => project.Name)
                .Order(StringComparer.Ordinal));
    }

    [Fact]
    public void AI_DE_001_DomainReferenceToInfrastructure_IsDetected()
    {
        ProjectDependencyGraph graph = CreateGraph(
            Node(
                "HotJoes.Domain.Vendor",
                projectReferences:
                ["HotJoes.Infrastructure.Persistence"]));

        AssertViolation(
            "AI-DE-001",
            ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_APP_002_ApplicationReferenceToApi_IsDetected()
    {
        ProjectDependencyGraph graph = CreateGraph(
            Node(
                "HotJoes.Application.Vendor",
                projectReferences: ["HotJoes.Api.Vendor"]));

        AssertViolation(
            "AI-APP-002",
            ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Npgsql.EntityFrameworkCore.PostgreSQL")]
    [InlineData("RabbitMQ.Client")]
    [InlineData("Microsoft.AspNetCore.OpenApi")]
    public void AI_API_001_InnerProjectForbiddenPackage_IsDetected(
        string packageReference)
    {
        ProjectDependencyGraph graph = CreateGraph(
            Node(
                "HotJoes.Application.Vendor",
                packageReferences: [packageReference]));

        AssertViolation(
            "AI-API-001",
            ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_CONS_003_ComplianceReferenceToVendorImplementation_IsDetected()
    {
        ProjectDependencyGraph graph = CreateGraph(
            Node(
                "HotJoes.Infrastructure.ComplianceConsumer",
                projectReferences: ["HotJoes.Application.Vendor"]));

        AssertViolation(
            "AI-CONS-003",
            ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_REP_001_RelayBypassingPersistenceBoundary_IsDetected()
    {
        ProjectDependencyGraph graph = CreateGraph(
            Node(
                "HotJoes.Infrastructure.VendorRelay",
                projectReferences: ["HotJoes.Application.Vendor"]));

        AssertViolation(
            "AI-REP-001",
            ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_ARCH_001_MissingControlledProductionProject_IsDetected()
    {
        ProjectDependencyGraph graph = CreateGraph(
            ExpectedProductionProjects
                .Where(name => name != "HotJoes.Domain.Vendor")
                .Select(name => Node(name))
                .ToArray());

        AssertViolation(
            "AI-ARCH-001",
            ProjectDependencyRuleSet.Evaluate(graph));
    }

    private static ProjectDependencyGraph CreateGraph(
        params ProjectDependencyNode[] nodes)
    {
        return new ProjectDependencyGraph(nodes);
    }

    private static ProjectDependencyNode Node(
        string name,
        string[]? projectReferences = null,
        string[]? packageReferences = null)
    {
        return new ProjectDependencyNode(
            name,
            projectPath: $"src/{name}/{name}.csproj",
            projectReferences: projectReferences ?? [],
            packageReferences: packageReferences ?? []);
    }

    private static void AssertViolation(
        string obligationId,
        IReadOnlyList<ArchitectureViolation> violations)
    {
        Assert.Contains(
            violations,
            violation => violation.ObligationId == obligationId);
    }
}
