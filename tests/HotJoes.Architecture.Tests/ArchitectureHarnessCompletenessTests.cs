namespace HotJoes.Architecture.Tests;

public sealed class ArchitectureHarnessCompletenessTests
{
    private const string ArchitectureProjectPath =
        "tests/HotJoes.Architecture.Tests/HotJoes.Architecture.Tests.csproj";

    private static readonly string[] ExpectedRuleSetNames =
    [
        nameof(ApiAddressStructuralRuleSet),
        nameof(CloudConfigurationStructuralRuleSet),
        nameof(DomainApplicationStructuralRuleSet),
        nameof(MigrationStructuralRuleSet),
        nameof(ProjectDependencyRuleSet),
        nameof(SecretExposureStructuralRuleSet)
    ];

    private static readonly string[] ExpectedObligationIds =
    [
        "AI-ADDR-001",
        "AI-ADDR-007",
        "AI-AGG-002",
        "AI-API-001",
        "AI-API-003",
        "AI-APP-002",
        "AI-ARCH-001",
        "AI-CFG-001",
        "AI-CI-001",
        "AI-CI-002",
        "AI-CONS-003",
        "AI-DE-001",
        "AI-DE-003",
        "AI-ENT-002",
        "AI-MIG-002",
        "AI-OUT-003",
        "AI-REP-001",
        "AI-SEC-001",
        "AI-SEC-002"
    ];

    [Fact]
    public void AI_ARCH_001_CurrentHarness_HasCompleteApprovedCoverage()
    {
        ArchitectureHarnessCatalog catalog =
            ArchitectureHarnessCatalog.LoadCurrent();

        IReadOnlyList<ArchitectureViolation> violations =
            ArchitectureHarnessCompletenessRuleSet.Evaluate(catalog);

        Assert.Empty(violations);
        Assert.Equal(
            ExpectedRuleSetNames.Order(StringComparer.Ordinal),
            catalog.RuleSetNames.Order(StringComparer.Ordinal));
        Assert.Equal(
            ExpectedObligationIds.Order(StringComparer.Ordinal),
            catalog.ExecutableObligationIds.Order(StringComparer.Ordinal));
        Assert.Contains(
            ArchitectureProjectPath,
            catalog.SolutionProjectPaths);
    }

    [Fact]
    public void AI_ARCH_001_MissingApprovedObligation_IsDetected()
    {
        ArchitectureHarnessCatalog catalog = Catalog(
            obligationIds: ExpectedObligationIds
                .Where(id => id != "AI-API-003")
                .ToArray());

        AssertViolation(
            ArchitectureHarnessCompletenessRuleSet.Evaluate(catalog),
            "Approved enforcement obligation 'AI-API-003' has no " +
            "executable architecture test evidence.");
    }

    [Fact]
    public void AI_ARCH_001_UnapprovedObligationRegistration_IsDetected()
    {
        ArchitectureHarnessCatalog catalog = Catalog(
            obligationIds: [.. ExpectedObligationIds, "AI-UNKNOWN-999"]);

        AssertViolation(
            ArchitectureHarnessCompletenessRuleSet.Evaluate(catalog),
            "Unapproved enforcement obligation 'AI-UNKNOWN-999' is " +
            "registered by the architecture harness.");
    }

    [Fact]
    public void AI_ARCH_001_MissingApprovedRuleSet_IsDetected()
    {
        ArchitectureHarnessCatalog catalog = Catalog(
            ruleSetNames: ExpectedRuleSetNames
                .Where(name => name != nameof(ApiAddressStructuralRuleSet))
                .ToArray());

        AssertViolation(
            ArchitectureHarnessCompletenessRuleSet.Evaluate(catalog),
            $"Approved architecture rule set " +
            $"'{nameof(ApiAddressStructuralRuleSet)}' is not registered.");
    }

    [Fact]
    public void AI_ARCH_001_ArchitectureTestProjectMissingFromSolution_IsDetected()
    {
        ArchitectureHarnessCatalog catalog = Catalog(
            solutionProjectPaths: []);

        AssertViolation(
            ArchitectureHarnessCompletenessRuleSet.Evaluate(catalog),
            $"Architecture test project '{ArchitectureProjectPath}' is not " +
            "included in HotJoes.sln.");
    }

    private static ArchitectureHarnessCatalog Catalog(
        IReadOnlyList<string>? ruleSetNames = null,
        IReadOnlyList<string>? obligationIds = null,
        IReadOnlyList<string>? solutionProjectPaths = null)
    {
        return ArchitectureHarnessCatalog.FromValues(
            ruleSetNames ?? ExpectedRuleSetNames,
            obligationIds ?? ExpectedObligationIds,
            solutionProjectPaths ?? [ArchitectureProjectPath]);
    }

    private static void AssertViolation(
        IReadOnlyList<ArchitectureViolation> violations,
        string expectedDescription)
    {
        ArchitectureViolation violation = Assert.Single(violations);
        Assert.Equal("AI-ARCH-001", violation.ObligationId);
        Assert.Equal(
            "HotJoes.Architecture.Tests",
            violation.ProjectName);
        Assert.Equal(expectedDescription, violation.Description);
    }
}
