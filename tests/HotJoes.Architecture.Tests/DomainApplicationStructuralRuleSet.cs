namespace HotJoes.Architecture.Tests;

public static class DomainApplicationStructuralRuleSet
{
    private const string DomainAssembly = "HotJoes.Domain.Vendor";
    private const string VendorApplicationAssembly =
        "HotJoes.Application.Vendor";
    private const string ComplianceAssembly =
        "HotJoes.Infrastructure.ComplianceConsumer";
    private const string VendorRepository =
        "HotJoes.Domain.Vendor.IVendorRepository";
    private const string AddressResolver =
        "HotJoes.Application.Vendor.IAddressResolver";

    private static readonly string[] PublishedContractTypeNames =
    [
        "HotJoes.Application.Vendor.VendorRegisteredIntegrationEvent",
        "HotJoes.Application.Vendor.VendorRegisteredIntegrationEventPayload",
        "HotJoes.Application.Vendor.VendorRegisteredBusinessAddress",
        "HotJoes.Application.Vendor.VendorRegisteredTradingCharacteristics",
        "HotJoes.Application.Vendor.VendorRegisteredOpeningHours"
    ];

    private static readonly string[] ProhibitedComplianceNameFragments =
    [
        "ComplianceRequirement",
        "PendingActivation",
        "VendorRepository",
        "VendorLookup"
    ];

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ArchitectureAssemblyCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var violations = new List<ArchitectureViolation>();

        foreach (StructuralTypeDescriptor type in catalog.Types)
        {
            EvaluateDomainType(type, violations);
            EvaluatePublishedContract(type, violations);
            EvaluateRepositoryPlacement(type, violations);
            EvaluateAddressPlacement(type, violations);
            EvaluateApplicationIsolation(type, violations);
            EvaluateComplianceIsolation(type, violations);
        }

        return violations
            .OrderBy(violation => violation.ObligationId)
            .ThenBy(violation => violation.ProjectName)
            .ThenBy(violation => violation.Description)
            .ToArray();
    }

    private static void EvaluateDomainType(
        StructuralTypeDescriptor type,
        ICollection<ArchitectureViolation> violations)
    {
        if (type.AssemblyName != DomainAssembly)
        {
            return;
        }

        foreach (string property in type.PublicSettableProperties)
        {
            string obligationId =
                property == "Id" || type.FullName.Contains(
                    "Entity",
                    StringComparison.Ordinal)
                    ? "AI-ENT-002"
                    : "AI-AGG-002";
            Add(
                violations,
                obligationId,
                type,
                $"Domain property '{property}' has a public setter.");
        }

        foreach (string assembly in type.ReferencedTypeAssemblies.Where(
            IsForbiddenDomainAssembly))
        {
            Add(
                violations,
                "AI-DE-001",
                type,
                $"Domain type references forbidden assembly '{assembly}'.");
        }

        if (type.FullName.Contains("Outbox", StringComparison.Ordinal) ||
            type.ReferencedTypeNames.Any(name => name.Contains(
                "Outbox",
                StringComparison.Ordinal)))
        {
            Add(
                violations,
                "AI-OUT-003",
                type,
                "Outbox representation exists in Domain decision-making.");
        }
    }

    private static void EvaluatePublishedContract(
        StructuralTypeDescriptor type,
        ICollection<ArchitectureViolation> violations)
    {
        if (!PublishedContractTypeNames.Contains(
                type.FullName,
                StringComparer.Ordinal))
        {
            return;
        }

        foreach (string domainType in type.ReferencedTypeNames.Where(name =>
            name.StartsWith(
                "HotJoes.Domain.",
                StringComparison.Ordinal)))
        {
            Add(
                violations,
                "AI-DE-003",
                type,
                $"Published contract reuses Domain type '{domainType}'.");
        }
    }

    private static void EvaluateRepositoryPlacement(
        StructuralTypeDescriptor type,
        ICollection<ArchitectureViolation> violations)
    {
        if (type.FullName == VendorRepository &&
            (type.AssemblyName != DomainAssembly ||
                type.Kind != StructuralTypeKind.Interface))
        {
            Add(
                violations,
                "AI-REP-001",
                type,
                "IVendorRepository is not a Domain-owned interface.");
        }

        if (type.ImplementedInterfaces.Contains(
                VendorRepository,
                StringComparer.Ordinal) &&
            !type.AssemblyName.StartsWith(
                "HotJoes.Infrastructure.",
                StringComparison.Ordinal))
        {
            Add(
                violations,
                "AI-REP-001",
                type,
                "Concrete Vendor repository is not Infrastructure-owned.");
        }
    }

    private static void EvaluateAddressPlacement(
        StructuralTypeDescriptor type,
        ICollection<ArchitectureViolation> violations)
    {
        if (type.FullName == AddressResolver &&
            (type.AssemblyName != VendorApplicationAssembly ||
                type.Kind != StructuralTypeKind.Interface))
        {
            Add(
                violations,
                "AI-ADDR-001",
                type,
                "IAddressResolver is not a Vendor Application-owned port.");
        }

        if (type.ImplementedInterfaces.Contains(
                AddressResolver,
                StringComparer.Ordinal) &&
            !type.AssemblyName.StartsWith(
                "HotJoes.Infrastructure.",
                StringComparison.Ordinal))
        {
            Add(
                violations,
                "AI-ADDR-001",
                type,
                "Address resolver implementation is not outer Infrastructure.");
        }
    }

    private static void EvaluateApplicationIsolation(
        StructuralTypeDescriptor type,
        ICollection<ArchitectureViolation> violations)
    {
        if (!type.AssemblyName.StartsWith(
                "HotJoes.Application.",
                StringComparison.Ordinal))
        {
            return;
        }

        foreach (string assembly in type.ReferencedTypeAssemblies.Where(
            assembly => assembly.StartsWith(
                    "HotJoes.Infrastructure.",
                    StringComparison.Ordinal) ||
                assembly.StartsWith(
                    "HotJoes.Api.",
                    StringComparison.Ordinal)))
        {
            Add(
                violations,
                "AI-APP-002",
                type,
                $"Application type references outer assembly '{assembly}'.");
        }
    }

    private static void EvaluateComplianceIsolation(
        StructuralTypeDescriptor type,
        ICollection<ArchitectureViolation> violations)
    {
        if (type.AssemblyName != ComplianceAssembly)
        {
            return;
        }

        if (ProhibitedComplianceNameFragments.Any(fragment =>
                type.FullName.Contains(fragment, StringComparison.Ordinal)))
        {
            Add(
                violations,
                "AI-CONS-003",
                type,
                "Thin Compliance consumer contains prohibited business " +
                "behavior or Vendor lookup type.");
        }

        foreach (string assembly in type.ReferencedTypeAssemblies.Where(
            assembly => assembly.Contains(
                ".Vendor",
                StringComparison.Ordinal)))
        {
            Add(
                violations,
                "AI-CONS-003",
                type,
                $"Compliance consumer references Vendor assembly '{assembly}'.");
        }
    }

    private static bool IsForbiddenDomainAssembly(string assemblyName)
    {
        return assemblyName.StartsWith(
                "HotJoes.Application.",
                StringComparison.Ordinal) ||
            assemblyName.StartsWith(
                "HotJoes.Infrastructure.",
                StringComparison.Ordinal) ||
            assemblyName.StartsWith(
                "HotJoes.Api.",
                StringComparison.Ordinal) ||
            assemblyName.Contains("RabbitMQ", StringComparison.Ordinal) ||
            assemblyName.Contains("EntityFrameworkCore", StringComparison.Ordinal) ||
            assemblyName.Contains("System.Text.Json", StringComparison.Ordinal);
    }

    private static void Add(
        ICollection<ArchitectureViolation> violations,
        string obligationId,
        StructuralTypeDescriptor type,
        string description)
    {
        violations.Add(new ArchitectureViolation(
            obligationId,
            type.AssemblyName,
            $"{type.FullName}: {description}"));
    }
}
