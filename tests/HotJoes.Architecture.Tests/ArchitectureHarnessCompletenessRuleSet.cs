namespace HotJoes.Architecture.Tests;

public static class ArchitectureHarnessCompletenessRuleSet
{
    private const string ArchitectureProject =
        "HotJoes.Architecture.Tests";
    private const string ArchitectureProjectPath =
        "tests/HotJoes.Architecture.Tests/HotJoes.Architecture.Tests.csproj";

    private static readonly string[] ApprovedRuleSetNames =
    [
        nameof(ApiAddressStructuralRuleSet),
        nameof(DomainApplicationStructuralRuleSet),
        nameof(MigrationStructuralRuleSet),
        nameof(ProjectDependencyRuleSet)
    ];

    private static readonly string[] ApprovedObligationIds =
    [
        "AI-ADDR-001",
        "AI-ADDR-007",
        "AI-AGG-002",
        "AI-API-001",
        "AI-API-003",
        "AI-APP-002",
        "AI-ARCH-001",
        "AI-CI-001",
        "AI-CI-002",
        "AI-CONS-003",
        "AI-DE-001",
        "AI-DE-003",
        "AI-ENT-002",
        "AI-MIG-002",
        "AI-OUT-003",
        "AI-REP-001"
    ];

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ArchitectureHarnessCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var violations = new List<ArchitectureViolation>();

        AddMissingRuleSets(catalog, violations);
        AddUnexpectedRuleSets(catalog, violations);
        AddMissingObligations(catalog, violations);
        AddUnexpectedObligations(catalog, violations);
        AddSolutionMembershipViolation(catalog, violations);

        return violations
            .OrderBy(violation => violation.Description, StringComparer.Ordinal)
            .ToArray();
    }

    private static void AddMissingRuleSets(
        ArchitectureHarnessCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (string ruleSet in ApprovedRuleSetNames.Except(
            catalog.RuleSetNames,
            StringComparer.Ordinal))
        {
            Add(
                violations,
                $"Approved architecture rule set '{ruleSet}' is not " +
                "registered.");
        }
    }

    private static void AddUnexpectedRuleSets(
        ArchitectureHarnessCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (string ruleSet in catalog.RuleSetNames.Except(
            ApprovedRuleSetNames,
            StringComparer.Ordinal))
        {
            Add(
                violations,
                $"Unapproved architecture rule set '{ruleSet}' is " +
                "registered.");
        }
    }

    private static void AddMissingObligations(
        ArchitectureHarnessCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (string obligationId in ApprovedObligationIds.Except(
            catalog.ExecutableObligationIds,
            StringComparer.Ordinal))
        {
            Add(
                violations,
                $"Approved enforcement obligation '{obligationId}' has no " +
                "executable architecture test evidence.");
        }
    }

    private static void AddUnexpectedObligations(
        ArchitectureHarnessCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (string obligationId in catalog.ExecutableObligationIds.Except(
            ApprovedObligationIds,
            StringComparer.Ordinal))
        {
            Add(
                violations,
                $"Unapproved enforcement obligation '{obligationId}' is " +
                "registered by the architecture harness.");
        }
    }

    private static void AddSolutionMembershipViolation(
        ArchitectureHarnessCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        if (!catalog.SolutionProjectPaths.Contains(
                ArchitectureProjectPath,
                StringComparer.Ordinal))
        {
            Add(
                violations,
                $"Architecture test project '{ArchitectureProjectPath}' is " +
                "not included in HotJoes.sln.");
        }
    }

    private static void Add(
        ICollection<ArchitectureViolation> violations,
        string description)
    {
        violations.Add(new ArchitectureViolation(
            "AI-ARCH-001",
            ArchitectureProject,
            description));
    }
}
