namespace HotJoes.Architecture.Tests;

public static class CommunityOwnershipStructuralRuleSet
{
    private const string CommunityAssembly =
        "HotJoes.Application.Community";

    private static readonly string[] RequiredCommunityTypes =
    [
        "HotJoes.Application.Community.CommunityParticipation",
        "HotJoes.Application.Community.ContactPreference",
        "HotJoes.Application.Community.ICommunityParticipationCommitter",
        "HotJoes.Application.Community.IJoinCommunityService",
        "HotJoes.Application.Community.IVendorRegistrationVerificationPort",
        "HotJoes.Application.Community.JoinCommunityRequest",
        "HotJoes.Application.Community.JoinCommunityResult",
        "HotJoes.Application.Community.JoinCommunityService"
    ];

    private static readonly string[] ProhibitedNameFragments =
    [
        "CommunicationConsent",
        "DeliveryInstruction",
        "PrimaryContact",
        "RecipientResolution"
    ];

    private static readonly string[] ProhibitedAssemblyReferences =
    [
        "HotJoes.Api.Vendor",
        "HotJoes.Application.Vendor",
        "HotJoes.Domain.Vendor",
        "HotJoes.Infrastructure.Vendor.Persistence",
        "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore",
        "Npgsql",
        "RabbitMQ"
    ];

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ArchitectureAssemblyCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var violations = new List<ArchitectureViolation>();
        StructuralTypeDescriptor[] communityTypes = catalog.Types
            .Where(type => type.AssemblyName == CommunityAssembly)
            .ToArray();

        foreach (string requiredType in RequiredCommunityTypes)
        {
            if (!communityTypes.Any(type => type.FullName == requiredType))
            {
                violations.Add(new ArchitectureViolation(
                    "AI-COMMUNITY-001",
                    CommunityAssembly,
                    $"Required Community-owned type '{requiredType}' " +
                    "is missing."));
            }
        }

        foreach (StructuralTypeDescriptor type in communityTypes)
        {
            if (type.PublicSettableProperties.Count > 0)
            {
                violations.Add(new ArchitectureViolation(
                    "AI-COMMUNITY-002",
                    CommunityAssembly,
                    $"Community type '{type.FullName}' exposes public " +
                    "settable state."));
            }

            foreach (string referencedAssembly in
                type.ReferencedTypeAssemblies)
            {
                if (ProhibitedAssemblyReferences.Any(fragment =>
                    referencedAssembly.Contains(
                        fragment,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    violations.Add(new ArchitectureViolation(
                        "AI-COMMUNITY-001",
                        CommunityAssembly,
                        $"Community type '{type.FullName}' references " +
                        $"prohibited assembly '{referencedAssembly}'."));
                }
            }

            if (ProhibitedNameFragments.Any(fragment =>
                type.FullName.Contains(fragment, StringComparison.Ordinal) ||
                type.ReferencedTypeNames.Any(name =>
                    name.Contains(fragment, StringComparison.Ordinal))))
            {
                violations.Add(new ArchitectureViolation(
                    "AI-COMMUNITY-001",
                    CommunityAssembly,
                    $"Community type '{type.FullName}' includes prohibited " +
                    "contact, consent, recipient or delivery content."));
            }
        }

        return violations
            .OrderBy(violation => violation.ObligationId)
            .ThenBy(violation => violation.Description)
            .ToArray();
    }
}
