using System.Text.RegularExpressions;

namespace HotJoes.Architecture.Tests;

public static class CloudConfigurationStructuralRuleSet
{
    private const string ObligationId = "AI-CFG-001";
    private const string VendorApiProject = "HotJoes.Api.Vendor";

    private static readonly string[] CloudProviderPackageFragments =
    [
        "Azure.Data.AppConfiguration",
        "Azure.Identity",
        "Azure.Security.KeyVault",
        "AzureAppConfiguration"
    ];

    private static readonly string[] CloudOrConfigurationTypeFragments =
    [
        "Azure.Data.AppConfiguration",
        "Azure.Identity",
        "Azure.Security.KeyVault",
        "Microsoft.Extensions.Configuration"
    ];

    private static readonly Regex StringKeyConfigurationAccess = new(
        "\\b(?:configuration|config)\\s*\\[\\s*\"",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ProjectDependencyGraph graph,
        ArchitectureSourceCatalog sources)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(sources);

        var violations = new List<ArchitectureViolation>();

        AddProjectPackageViolations(graph, violations);
        AddSourceViolations(sources, violations);

        return violations
            .OrderBy(violation => violation.ProjectName)
            .ThenBy(violation => violation.Description)
            .ToArray();
    }

    private static void AddProjectPackageViolations(
        ProjectDependencyGraph graph,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (ProjectDependencyNode project in graph.ProductionProjects
            .Where(project => IsInnerProject(project.Name)))
        {
            foreach (string package in project.PackageReferences.Where(
                IsCloudProviderPackage))
            {
                violations.Add(new ArchitectureViolation(
                    ObligationId,
                    project.Name,
                    $"Inner project references cloud-provider package " +
                    $"'{package}'."));
            }
        }
    }

    private static void AddSourceViolations(
        ArchitectureSourceCatalog sources,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (SourceFileDescriptor file in sources.Files)
        {
            string projectName = ProjectName(file.RelativePath);

            if (!IsSelectedProject(projectName) ||
                IsApprovedCompositionBoundary(file.RelativePath))
            {
                continue;
            }

            foreach (string fragment in
                CloudOrConfigurationTypeFragments.Where(fragment =>
                    file.Content.Contains(
                        fragment,
                        StringComparison.Ordinal)))
            {
                violations.Add(new ArchitectureViolation(
                    ObligationId,
                    projectName,
                    $"Source '{file.RelativePath}' references prohibited " +
                    $"cloud or configuration type fragment '{fragment}'."));
            }

            if (StringKeyConfigurationAccess.IsMatch(file.Content))
            {
                violations.Add(new ArchitectureViolation(
                    ObligationId,
                    projectName,
                    $"Source '{file.RelativePath}' uses arbitrary " +
                    "string-key configuration outside an approved " +
                    "composition or configuration adapter boundary."));
            }
        }
    }

    private static bool IsCloudProviderPackage(string packageName)
    {
        return CloudProviderPackageFragments.Any(fragment =>
            packageName.Contains(
                fragment,
                StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsSelectedProject(string projectName)
    {
        return projectName == VendorApiProject ||
            IsInnerProject(projectName);
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

    private static bool IsApprovedCompositionBoundary(string relativePath)
    {
        return relativePath ==
                "src/HotJoes.Api.Vendor/Program.cs" ||
            relativePath.StartsWith(
                "src/HotJoes.Api.Vendor/Configuration/",
                StringComparison.Ordinal);
    }

    private static string ProjectName(string relativePath)
    {
        string[] segments = relativePath.Split('/');

        return segments.Length > 1 && segments[0] == "src"
            ? segments[1]
            : relativePath;
    }
}
