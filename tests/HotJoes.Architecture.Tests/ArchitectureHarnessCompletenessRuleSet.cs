namespace HotJoes.Architecture.Tests;

public static class ArchitectureHarnessCompletenessRuleSet
{
    private const string ArchitectureProject = "HotJoes.Architecture.Tests";
    private const string ArchitectureProjectPath = "tests/HotJoes.Architecture.Tests/HotJoes.Architecture.Tests.csproj";

    private static readonly string[] ApprovedRuleSetNames =
    [
        nameof(ApiAddressStructuralRuleSet), nameof(CloudConfigurationStructuralRuleSet),
        nameof(CommunityOwnershipStructuralRuleSet), nameof(ComplianceDeterminationStructuralRuleSet),
        nameof(DomainApplicationStructuralRuleSet), nameof(MigrationStructuralRuleSet),
        nameof(ProjectDependencyRuleSet), nameof(SecretExposureStructuralRuleSet)
    ];

    private static readonly string[] ApprovedObligationIds =
    [
        "AI-ADDR-001", "AI-ADDR-007", "AI-AGG-002", "AI-API-001", "AI-API-003",
        "AI-APP-002", "AI-ARCH-001", "AI-BROWSER-012", "AI-CFG-001", "AI-CI-001",
        "AI-CI-002", "AI-COMMUNITY-001", "AI-COMMUNITY-002", "AI-COMMUNITY-003",
        "AI-CONS-003", "AI-COMP-001", "AI-COMP-002", "AI-DE-001", "AI-DE-003",
        "AI-ENT-002", "AI-MIG-002", "AI-OUT-003", "AI-REP-001", "AI-RUNTIME-001",
        "AI-RUNTIME-002", "AI-RUNTIME-004", "AI-RUNTIME-006", "AI-RUNTIME-009",
        "AI-SEC-001", "AI-SEC-002"
    ];

    public static IReadOnlyList<ArchitectureViolation> Evaluate(ArchitectureHarnessCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        var violations = new List<ArchitectureViolation>();
        AddDifferences(ApprovedRuleSetNames, catalog.RuleSetNames, "Approved architecture rule set '{0}' is not registered.", violations);
        AddDifferences(catalog.RuleSetNames, ApprovedRuleSetNames, "Unapproved architecture rule set '{0}' is registered.", violations);
        AddDifferences(ApprovedObligationIds, catalog.ExecutableObligationIds, "Approved enforcement obligation '{0}' has no executable architecture test evidence.", violations);
        AddDifferences(catalog.ExecutableObligationIds, ApprovedObligationIds, "Unapproved enforcement obligation '{0}' is registered by the architecture harness.", violations);
        if (!catalog.SolutionProjectPaths.Contains(ArchitectureProjectPath, StringComparer.Ordinal))
            Add(violations, $"Architecture test project '{ArchitectureProjectPath}' is not included in HotJoes.sln.");
        return violations.OrderBy(value => value.Description, StringComparer.Ordinal).ToArray();
    }

    private static void AddDifferences(IEnumerable<string> source, IEnumerable<string> other, string format, ICollection<ArchitectureViolation> violations)
    {
        foreach (string value in source.Except(other, StringComparer.Ordinal)) Add(violations, string.Format(format, value));
    }

    private static void Add(ICollection<ArchitectureViolation> violations, string description) =>
        violations.Add(new ArchitectureViolation("AI-ARCH-001", ArchitectureProject, description));
}
