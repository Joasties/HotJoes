namespace HotJoes.Architecture.Tests;

public sealed class ComplianceProjectDependencyTests
{
    [Fact]
    public void AI_COMP_001_ComplianceProjects_AreControlledWithApprovedReferences()
    {
        ArchitectureRepository repository =
            ArchitectureRepository.FindFromTestAssembly();
        ProjectDependencyGraph graph = repository.LoadProjectGraph();

        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name == "HotJoes.Application.Compliance");
        Assert.Contains(
            graph.ProductionProjects,
            project => project.Name ==
                "HotJoes.Infrastructure.Vendor.Compliance");
        Assert.Empty(ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_COMP_001_ComplianceApplicationReferenceToVendor_IsDetected()
    {
        ProjectDependencyGraph graph = Graph(
            Node(
                "HotJoes.Application.Compliance",
                ["HotJoes.Application.Vendor"]));

        AssertViolation(ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_COMP_001_VendorComplianceAdapterMissingComplianceReference_IsDetected()
    {
        ProjectDependencyGraph graph = Graph(
            Node(
                "HotJoes.Infrastructure.Vendor.Compliance",
                ["HotJoes.Application.Vendor"]));

        AssertViolation(ProjectDependencyRuleSet.Evaluate(graph));
    }

    [Fact]
    public void AI_COMP_001_VendorComplianceAdapterReferenceToApi_IsDetected()
    {
        ProjectDependencyGraph graph = Graph(
            Node(
                "HotJoes.Infrastructure.Vendor.Compliance",
                [
                    "HotJoes.Application.Compliance",
                    "HotJoes.Application.Vendor",
                    "HotJoes.Api.Vendor"
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
            violation => violation.ObligationId == "AI-COMP-001");
}
