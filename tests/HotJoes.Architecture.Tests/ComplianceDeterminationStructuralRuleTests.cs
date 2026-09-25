namespace HotJoes.Architecture.Tests;

public sealed class ComplianceDeterminationStructuralRuleTests
{
    [Fact]
    public void AI_COMP_001_CurrentComplianceBoundary_HasApprovedOwnership()
    {
        ArchitectureAssemblyCatalog catalog =
            ArchitectureAssemblyCatalog.LoadControlledAssemblies();

        Assert.Empty(
            ComplianceDeterminationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMP_001_VendorPortOutsideVendorApplication_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Interface(
                "HotJoes.Application.Vendor.IComplianceDeterminationPort",
                "HotJoes.Application.Compliance"));

        AssertViolation(
            "AI-COMP-001",
            ComplianceDeterminationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMP_001_AdapterOutsideVendorInfrastructure_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Vendor.ComplianceDeterminationAdapter",
                "HotJoes.Application.Vendor",
                implementedInterfaces:
                ["HotJoes.Application.Vendor.IComplianceDeterminationPort"]));

        AssertViolation(
            "AI-COMP-001",
            ComplianceDeterminationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMP_001_ComplianceServiceOutsideComplianceApplication_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Interface(
                "HotJoes.Application.Compliance.IComplianceDeterminationService",
                "HotJoes.Application.Vendor"));

        AssertViolation(
            "AI-COMP-001",
            ComplianceDeterminationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMP_002_ComplianceStubOutsideComplianceApplication_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Infrastructure.Vendor.Compliance.StubComplianceApplication",
                "HotJoes.Infrastructure.Vendor.Compliance",
                implementedInterfaces:
                [
                    "HotJoes.Application.Compliance.IComplianceDeterminationService"
                ]));

        AssertViolation(
            "AI-COMP-002",
            ComplianceDeterminationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMP_002_DuplicateComplianceServiceImplementation_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Compliance.StubComplianceApplication",
                "HotJoes.Application.Compliance",
                implementedInterfaces:
                [
                    "HotJoes.Application.Compliance.IComplianceDeterminationService"
                ]),
            StructuralTypeDescriptor.Class(
                "HotJoes.Api.Vendor.DuplicateCompliancePolicy",
                "HotJoes.Api.Vendor",
                implementedInterfaces:
                [
                    "HotJoes.Application.Compliance.IComplianceDeterminationService"
                ]));

        AssertViolation(
            "AI-COMP-002",
            ComplianceDeterminationStructuralRuleSet.Evaluate(catalog));
    }

    private static ArchitectureAssemblyCatalog Catalog(
        params StructuralTypeDescriptor[] types) =>
        ArchitectureAssemblyCatalog.FromTypes(types);

    private static void AssertViolation(
        string obligationId,
        IReadOnlyList<ArchitectureViolation> violations) =>
        Assert.Contains(
            violations,
            violation => violation.ObligationId == obligationId);
}
