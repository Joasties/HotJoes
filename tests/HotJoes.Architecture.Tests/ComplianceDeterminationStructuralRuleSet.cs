namespace HotJoes.Architecture.Tests;

public static class ComplianceDeterminationStructuralRuleSet
{
    private const string VendorApplicationAssembly =
        "HotJoes.Application.Vendor";
    private const string ComplianceApplicationAssembly =
        "HotJoes.Application.Compliance";
    private const string VendorComplianceInfrastructureAssembly =
        "HotJoes.Infrastructure.Vendor.Compliance";
    private const string VendorPort =
        "HotJoes.Application.Vendor.IComplianceDeterminationPort";
    private const string ComplianceService =
        "HotJoes.Application.Compliance.IComplianceDeterminationService";
    private const string StubComplianceApplication =
        "HotJoes.Application.Compliance.StubComplianceApplication";
    private const string ComplianceAdapter =
        "HotJoes.Infrastructure.Vendor.Compliance.ComplianceDeterminationAdapter";

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ArchitectureAssemblyCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var violations = new List<ArchitectureViolation>();

        EvaluateRequiredInterface(
            catalog,
            VendorPort,
            VendorApplicationAssembly,
            "Vendor Application-owned consumed port",
            violations);
        EvaluateRequiredInterface(
            catalog,
            ComplianceService,
            ComplianceApplicationAssembly,
            "Compliance Application-owned supplied service",
            violations);
        EvaluateVendorPortImplementations(catalog, violations);
        EvaluateComplianceServiceImplementations(catalog, violations);

        return violations
            .OrderBy(violation => violation.ObligationId)
            .ThenBy(violation => violation.ProjectName)
            .ThenBy(violation => violation.Description)
            .ToArray();
    }

    private static void EvaluateRequiredInterface(
        ArchitectureAssemblyCatalog catalog,
        string fullName,
        string assemblyName,
        string responsibility,
        ICollection<ArchitectureViolation> violations)
    {
        StructuralTypeDescriptor? type = catalog.Types.SingleOrDefault(
            candidate => candidate.FullName == fullName);

        if (type is null)
        {
            Add(
                violations,
                "AI-COMP-001",
                assemblyName,
                $"Required {responsibility} '{fullName}' is missing.");
            return;
        }

        if (type.AssemblyName != assemblyName ||
            type.Kind != StructuralTypeKind.Interface)
        {
            Add(
                violations,
                "AI-COMP-001",
                type.AssemblyName,
                $"'{fullName}' is not the approved {responsibility}.");
        }
    }

    private static void EvaluateVendorPortImplementations(
        ArchitectureAssemblyCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        StructuralTypeDescriptor[] implementations = catalog.Types
            .Where(type => type.ImplementedInterfaces.Contains(
                VendorPort,
                StringComparer.Ordinal))
            .ToArray();

        foreach (StructuralTypeDescriptor implementation in implementations)
        {
            if (implementation.FullName != ComplianceAdapter ||
                implementation.AssemblyName !=
                    VendorComplianceInfrastructureAssembly)
            {
                Add(
                    violations,
                    "AI-COMP-001",
                    implementation.AssemblyName,
                    $"Vendor Compliance port implementation " +
                    $"'{implementation.FullName}' is outside the approved " +
                    "outer adapter boundary.");
            }
        }

        if (!implementations.Any(type =>
                type.FullName == ComplianceAdapter &&
                type.AssemblyName == VendorComplianceInfrastructureAssembly))
        {
            Add(
                violations,
                "AI-COMP-001",
                VendorComplianceInfrastructureAssembly,
                $"Approved adapter '{ComplianceAdapter}' is missing.");
        }
    }

    private static void EvaluateComplianceServiceImplementations(
        ArchitectureAssemblyCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        StructuralTypeDescriptor[] implementations = catalog.Types
            .Where(type => type.ImplementedInterfaces.Contains(
                ComplianceService,
                StringComparer.Ordinal))
            .ToArray();

        foreach (StructuralTypeDescriptor implementation in implementations)
        {
            if (implementation.FullName != StubComplianceApplication ||
                implementation.AssemblyName != ComplianceApplicationAssembly)
            {
                Add(
                    violations,
                    "AI-COMP-002",
                    implementation.AssemblyName,
                    $"Compliance policy implementation " +
                    $"'{implementation.FullName}' is outside the approved " +
                    "Compliance Application owner.");
            }
        }

        if (implementations.Count(type =>
                type.FullName == StubComplianceApplication &&
                type.AssemblyName == ComplianceApplicationAssembly) != 1)
        {
            Add(
                violations,
                "AI-COMP-002",
                ComplianceApplicationAssembly,
                "Exactly one approved Epic 1 Compliance determination " +
                "policy implementation is required.");
        }
    }

    private static void Add(
        ICollection<ArchitectureViolation> violations,
        string obligationId,
        string projectName,
        string description) =>
        violations.Add(new ArchitectureViolation(
            obligationId,
            projectName,
            description));
}
