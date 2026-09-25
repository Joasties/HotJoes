namespace HotJoes.Architecture.Tests;

public sealed class CommunityProjectDependencyTests
{
    [Fact]
    public void AI_COMMUNITY_001_CommunityProjects_AreControlledWithApprovedDirection()
    {
        ArchitectureRepository repository =
            ArchitectureRepository.FindFromTestAssembly();
        ProjectDependencyGraph graph = repository.LoadProjectGraph();

        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name == "HotJoes.Application.Community");
        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name ==
                "HotJoes.Infrastructure.Community.Persistence");
        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name ==
                "HotJoes.Infrastructure.CommunityConsumer");
        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name ==
                "HotJoes.Infrastructure.CommunityRelay");
        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name ==
                "HotJoes.Worker.CommunityConsumer");
        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name ==
                "HotJoes.Worker.CommunityRelay");
        Assert.Empty(ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_COMMUNITY_001_CommunityApplicationReferenceToVendor_IsDetected()
    {
        ProjectDependencyGraph graph = Graph(
            Node(
                "HotJoes.Application.Community",
                ["HotJoes.Application.Vendor"]));

        AssertViolation(ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_COMMUNITY_001_CommunityPersistenceReferenceToVendor_IsDetected()
    {
        ProjectDependencyGraph graph = Graph(
            Node("HotJoes.Application.Community", []),
            Node(
                "HotJoes.Infrastructure.Community.Persistence",
                [
                    "HotJoes.Application.Community",
                    "HotJoes.Application.Vendor"
                ]));

        AssertViolation(ProjectDependencyRuleSet.Evaluate(graph));
    }

    private static ProjectDependencyGraph Graph(
        params ProjectDependencyNode[] nodes) =>
        new(nodes);

    private static ProjectDependencyNode Node(
        string name,
        string[] projectReferences) =>
        new(
            name,
            $"src/{name}/{name}.csproj",
            projectReferences,
            packageReferences: []);

    private static void AssertViolation(
        IReadOnlyList<ArchitectureViolation> violations) =>
        Assert.Contains(
            violations,
            violation => violation.ObligationId == "AI-COMMUNITY-001");
}
