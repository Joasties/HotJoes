namespace HotJoes.Architecture.Tests;

public sealed class CommunityOwnershipStructuralRuleTests
{
    [Fact]
    public void AI_COMMUNITY_001_And_002_CurrentCommunityBoundary_HasApprovedOwnership()
    {
        ArchitectureAssemblyCatalog catalog =
            ArchitectureAssemblyCatalog.LoadControlledAssemblies();

        Assert.Empty(CommunityOwnershipStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMMUNITY_001_MissingCommunityOwnedType_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Community.JoinCommunityService",
                "HotJoes.Application.Community"));

        AssertViolation(
            "AI-COMMUNITY-001",
            CommunityOwnershipStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMMUNITY_001_VendorDomainDependency_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = CompleteCatalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Community.JoinCommunityService",
                "HotJoes.Application.Community",
                referencedTypeAssemblies: ["HotJoes.Domain.Vendor"]));

        AssertViolation(
            "AI-COMMUNITY-001",
            CommunityOwnershipStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMMUNITY_001_PrimaryContactRepresentation_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = CompleteCatalog(
            StructuralTypeDescriptor.Record(
                "HotJoes.Application.Community.PrimaryContactSnapshot",
                "HotJoes.Application.Community"));

        AssertViolation(
            "AI-COMMUNITY-001",
            CommunityOwnershipStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_COMMUNITY_002_PublicMutation_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = CompleteCatalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Community.CommunityParticipation",
                "HotJoes.Application.Community",
                publicSettableProperties: ["ContactPreference"]));

        AssertViolation(
            "AI-COMMUNITY-002",
            CommunityOwnershipStructuralRuleSet.Evaluate(catalog));
    }

    private static ArchitectureAssemblyCatalog CompleteCatalog(
        StructuralTypeDescriptor replacement)
    {
        StructuralTypeDescriptor[] required =
        [
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Community.CommunityParticipation",
                "HotJoes.Application.Community"),
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Community.ContactPreference",
                "HotJoes.Application.Community"),
            StructuralTypeDescriptor.Interface(
                "HotJoes.Application.Community.ICommunityParticipationCommitter",
                "HotJoes.Application.Community"),
            StructuralTypeDescriptor.Interface(
                "HotJoes.Application.Community.IJoinCommunityService",
                "HotJoes.Application.Community"),
            StructuralTypeDescriptor.Interface(
                "HotJoes.Application.Community.IVendorRegistrationVerificationPort",
                "HotJoes.Application.Community"),
            StructuralTypeDescriptor.Record(
                "HotJoes.Application.Community.JoinCommunityRequest",
                "HotJoes.Application.Community"),
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Community.JoinCommunityResult",
                "HotJoes.Application.Community"),
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Community.JoinCommunityService",
                "HotJoes.Application.Community")
        ];

        return Catalog(
            required
                .Where(type => type.FullName != replacement.FullName)
                .Append(replacement)
                .ToArray());
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
