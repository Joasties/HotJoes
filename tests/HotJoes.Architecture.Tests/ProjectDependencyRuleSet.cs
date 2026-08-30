namespace HotJoes.Architecture.Tests;

public static class ProjectDependencyRuleSet
{
    private static readonly IReadOnlyDictionary<string, ProjectRule>
        ApprovedProjects = new Dictionary<string, ProjectRule>(
            StringComparer.Ordinal)
        {
            ["HotJoes.Api.Vendor"] = new(
                "AI-API-001",
                ["HotJoes.Application.Vendor"]),
            ["HotJoes.Application.Address"] = new(
                "AI-APP-002",
                []),
            ["HotJoes.Application.Vendor"] = new(
                "AI-APP-002",
                ["HotJoes.Domain.Vendor"]),
            ["HotJoes.Domain.Vendor"] = new(
                "AI-DE-001",
                []),
            ["HotJoes.Infrastructure.ComplianceConsumer"] = new(
                "AI-CONS-003",
                []),
            ["HotJoes.Infrastructure.Health"] = new(
                "AI-ARCH-001",
                []),
            ["HotJoes.Infrastructure.Persistence"] = new(
                "AI-REP-001",
                ["HotJoes.Application.Vendor"]),
            ["HotJoes.Infrastructure.Vendor.Address"] = new(
                "AI-ADDR-001",
                [
                    "HotJoes.Application.Address",
                    "HotJoes.Application.Vendor"
                ]),
            ["HotJoes.Infrastructure.VendorRelay"] = new(
                "AI-REP-001",
                ["HotJoes.Infrastructure.Persistence"])
        };

    private static readonly string[] ForbiddenInnerPackageFragments =
    [
        "AspNetCore",
        "EntityFrameworkCore",
        "Npgsql",
        "OpenApi",
        "RabbitMQ"
    ];

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ProjectDependencyGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        var violations = new List<ArchitectureViolation>();
        IReadOnlyDictionary<string, ProjectDependencyNode> actualProjects =
            graph.ProductionProjects.ToDictionary(
                project => project.Name,
                StringComparer.Ordinal);

        AddCompletenessViolations(actualProjects, violations);

        foreach (ProjectDependencyNode project in graph.ProductionProjects)
        {
            if (ApprovedProjects.TryGetValue(
                    project.Name,
                    out ProjectRule? rule))
            {
                AddReferenceViolations(project, rule, violations);
            }

            if (IsInnerProject(project.Name))
            {
                AddForbiddenPackageViolations(project, violations);
            }
        }

        return violations
            .OrderBy(violation => violation.ObligationId)
            .ThenBy(violation => violation.ProjectName)
            .ThenBy(violation => violation.Description)
            .ToArray();
    }

    private static void AddCompletenessViolations(
        IReadOnlyDictionary<string, ProjectDependencyNode> actualProjects,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (string expectedProject in ApprovedProjects.Keys)
        {
            if (!actualProjects.ContainsKey(expectedProject))
            {
                violations.Add(new ArchitectureViolation(
                    "AI-ARCH-001",
                    expectedProject,
                    "Controlled production project is missing from the " +
                    "solution graph."));
            }
        }

        foreach (string unexpectedProject in actualProjects.Keys
            .Except(ApprovedProjects.Keys, StringComparer.Ordinal))
        {
            violations.Add(new ArchitectureViolation(
                "AI-ARCH-001",
                unexpectedProject,
                "Unapproved production project exists in the solution " +
                "graph."));
        }
    }

    private static void AddReferenceViolations(
        ProjectDependencyNode project,
        ProjectRule rule,
        ICollection<ArchitectureViolation> violations)
    {
        string[] unexpected = project.ProjectReferences
            .Except(rule.ApprovedReferences, StringComparer.Ordinal)
            .ToArray();
        string[] missing = rule.ApprovedReferences
            .Except(project.ProjectReferences, StringComparer.Ordinal)
            .ToArray();

        foreach (string reference in unexpected)
        {
            violations.Add(new ArchitectureViolation(
                rule.ObligationId,
                project.Name,
                $"Project reference '{reference}' is not approved."));
        }

        foreach (string reference in missing)
        {
            violations.Add(new ArchitectureViolation(
                rule.ObligationId,
                project.Name,
                $"Approved project reference '{reference}' is missing."));
        }
    }

    private static void AddForbiddenPackageViolations(
        ProjectDependencyNode project,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (string package in project.PackageReferences)
        {
            if (ForbiddenInnerPackageFragments.Any(fragment =>
                    package.Contains(
                        fragment,
                        StringComparison.OrdinalIgnoreCase)))
            {
                violations.Add(new ArchitectureViolation(
                    "AI-API-001",
                    project.Name,
                    $"Inner project references forbidden package " +
                    $"'{package}'."));
            }
        }
    }

    private static bool IsInnerProject(string projectName)
    {
        return projectName.StartsWith(
                "HotJoes.Domain.",
                StringComparison.Ordinal) ||
            projectName.StartsWith(
                "HotJoes.Application.",
                StringComparison.Ordinal);
    }

    private sealed record ProjectRule(
        string ObligationId,
        IReadOnlyList<string> ApprovedReferences);
}
