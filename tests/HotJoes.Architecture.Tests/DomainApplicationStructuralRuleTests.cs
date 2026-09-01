namespace HotJoes.Architecture.Tests;

public sealed class DomainApplicationStructuralRuleTests
{
    [Fact]
    public void AI_ARCH_001_ApprovedCompiledAssemblies_HaveNoStructuralViolations()
    {
        ArchitectureAssemblyCatalog catalog =
            ArchitectureAssemblyCatalog.LoadControlledAssemblies();

        IReadOnlyList<ArchitectureViolation> violations =
            DomainApplicationStructuralRuleSet.Evaluate(catalog);

        Assert.Empty(violations);
    }

    [Fact]
    public void AI_AGG_002_PublicDomainStateSetter_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Domain.Vendor.Vendor",
                "HotJoes.Domain.Vendor",
                publicSettableProperties: ["State"]));

        AssertViolation(
            "AI-AGG-002",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_ENT_002_PublicDomainEntityIdentitySetter_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Domain.Vendor.ExampleEntity",
                "HotJoes.Domain.Vendor",
                publicSettableProperties: ["Id"]));

        AssertViolation(
            "AI-ENT-002",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_DE_001_DomainInfrastructureTypeReference_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Record(
                "HotJoes.Domain.Vendor.VendorRegistered",
                "HotJoes.Domain.Vendor",
                referencedTypeAssemblies:
                ["HotJoes.Infrastructure.Persistence"]));

        AssertViolation(
            "AI-DE-001",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_DE_003_IntegrationEventContractReusingDomainType_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Record(
                "HotJoes.Application.Vendor.VendorRegisteredIntegrationEvent",
                "HotJoes.Application.Vendor",
                referencedTypeNames:
                ["HotJoes.Domain.Vendor.BusinessAddressSnapshot"]));

        AssertViolation(
            "AI-DE-003",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_OUT_003_OutboxRepresentationInsideDomain_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Domain.Vendor.VendorRegistrationOutboxRecord",
                "HotJoes.Domain.Vendor"));

        AssertViolation(
            "AI-OUT-003",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_REP_001_RepositoryConcreteImplementationInsideDomain_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Interface(
                "HotJoes.Domain.Vendor.IVendorRepository",
                "HotJoes.Domain.Vendor"),
            StructuralTypeDescriptor.Class(
                "HotJoes.Domain.Vendor.SqlVendorRepository",
                "HotJoes.Domain.Vendor",
                implementedInterfaces:
                ["HotJoes.Domain.Vendor.IVendorRepository"]));

        AssertViolation(
            "AI-REP-001",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_ADDR_001_AddressResolverImplementationInsideApplication_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Interface(
                "HotJoes.Application.Vendor.IAddressResolver",
                "HotJoes.Application.Vendor"),
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Vendor.AddressResolver",
                "HotJoes.Application.Vendor",
                implementedInterfaces:
                ["HotJoes.Application.Vendor.IAddressResolver"]));

        AssertViolation(
            "AI-ADDR-001",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_APP_002_ApplicationInfrastructureReference_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Application.Vendor.RegisterVendorService",
                "HotJoes.Application.Vendor",
                referencedTypeAssemblies:
                ["HotJoes.Infrastructure.Persistence"]));

        AssertViolation(
            "AI-APP-002",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Theory]
    [InlineData("HotJoes.Infrastructure.ComplianceConsumer.VendorRepositoryLookup")]
    [InlineData("HotJoes.Infrastructure.ComplianceConsumer.ComplianceRequirement")]
    [InlineData("HotJoes.Infrastructure.ComplianceConsumer.PendingActivationProcessor")]
    public void AI_CONS_003_ProhibitedComplianceConsumerBehavior_IsDetected(
        string prohibitedTypeName)
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                prohibitedTypeName,
                "HotJoes.Infrastructure.ComplianceConsumer"));

        AssertViolation(
            "AI-CONS-003",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_CONS_003_ComplianceConsumerVendorTypeReference_IsDetected()
    {
        ArchitectureAssemblyCatalog catalog = Catalog(
            StructuralTypeDescriptor.Class(
                "HotJoes.Infrastructure.ComplianceConsumer.Consumer",
                "HotJoes.Infrastructure.ComplianceConsumer",
                referencedTypeAssemblies:
                ["HotJoes.Application.Vendor"]));

        AssertViolation(
            "AI-CONS-003",
            DomainApplicationStructuralRuleSet.Evaluate(catalog));
    }

    private static ArchitectureAssemblyCatalog Catalog(
        params StructuralTypeDescriptor[] types)
    {
        return ArchitectureAssemblyCatalog.FromTypes(types);
    }

    private static void AssertViolation(
        string obligationId,
        IReadOnlyList<ArchitectureViolation> violations)
    {
        Assert.Contains(
            violations,
            violation => violation.ObligationId == obligationId);
    }
}
