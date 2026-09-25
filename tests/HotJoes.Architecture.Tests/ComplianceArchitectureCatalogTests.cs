namespace HotJoes.Architecture.Tests;

public sealed class ComplianceArchitectureCatalogTests
{
    [Fact]
    public void AI_COMP_001_ControlledAssemblyCatalog_IncludesBothComplianceBoundaries()
    {
        ArchitectureAssemblyCatalog catalog =
            ArchitectureAssemblyCatalog.LoadControlledAssemblies();

        Assert.Contains(
            catalog.Types,
            type => type.AssemblyName ==
                "HotJoes.Application.Compliance");
        Assert.Contains(
            catalog.Types,
            type => type.AssemblyName ==
                "HotJoes.Infrastructure.Vendor.Compliance");
    }

    [Fact]
    public void AI_COMP_001_ArchitectureHarness_RegistersComplianceRuleAndObligations()
    {
        ArchitectureHarnessCatalog catalog =
            ArchitectureHarnessCatalog.LoadCurrent();

        Assert.Contains(
            nameof(ComplianceDeterminationStructuralRuleSet),
            catalog.RuleSetNames);
        Assert.Contains("AI-COMP-001", catalog.ExecutableObligationIds);
        Assert.Contains("AI-COMP-002", catalog.ExecutableObligationIds);
    }
}
