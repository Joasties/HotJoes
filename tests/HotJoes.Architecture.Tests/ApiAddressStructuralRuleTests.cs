namespace HotJoes.Architecture.Tests;

public sealed class ApiAddressStructuralRuleTests
{
    [Fact]
    public void AI_ARCH_001_ApprovedApiAndAddressSources_HaveNoViolations()
    {
        ArchitectureSourceCatalog catalog =
            ArchitectureSourceCatalog.LoadControlledSources();

        IReadOnlyList<ArchitectureViolation> violations =
            ApiAddressStructuralRuleSet.Evaluate(catalog);

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData("IAddressResolver")]
    [InlineData("IVendorRepository")]
    [InlineData("VendorRegistrationDbContext")]
    [InlineData("IOutboxRelayStore")]
    [InlineData("RabbitMqComplianceRecoveryPublisher")]
    [InlineData("HotJoes.Infrastructure.Persistence")]
    public void AI_API_001_EndpointImplementationCollaborator_IsDetected(
        string prohibitedCollaborator)
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/VendorEndpointMappings.cs",
                $$"""
                public static class VendorEndpointMappings
                {
                    private static void Endpoint({{prohibitedCollaborator}} collaborator) { }
                }
                """));

        AssertViolation(
            "AI-API-001",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_API_003_EndpointSpecificExceptionHandling_IsDetected()
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/VendorEndpointMappings.cs",
                "try { } catch (Exception) { return Results.Problem(); }"));

        AssertViolation(
            "AI-API-003",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_API_003_EndpointConstructingErrorPayload_IsDetected()
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/VendorEndpointMappings.cs",
                "return new VendorApiErrorResponse(\"local\", \"local\", null);"));

        AssertViolation(
            "AI-API-003",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_API_003_MultipleControlledMapperBoundaries_AreDetected()
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/VendorApiErrorMapper.cs",
                "public sealed class VendorApiErrorMapper { }"),
            File(
                "src/HotJoes.Api.Vendor/SecondVendorApiErrorMapper.cs",
                "public sealed class VendorApiErrorMapper { }"));

        AssertViolation(
            "AI-API-003",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    [Theory]
    [InlineData("RegistrationDeclarationFailure")]
    [InlineData("ConditionalRuleFailure")]
    public void AI_API_003_SupersededFailureMapping_IsDetected(
        string supersededFailure)
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/VendorApiErrorMapper.cs",
                $"RegisterVendorResult.{supersededFailure} => Map()"));

        AssertViolation(
            "AI-API-003",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_API_003_MultipleValidationOutcomeBranches_AreDetected()
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/VendorApiErrorMapper.cs",
                """
                RegisterVendorResult.RequestValidationFailure first => Map(first),
                RegisterVendorResult.RequestValidationFailure second => Map(second)
                """));

        AssertViolation(
            "AI-API-003",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    [Fact]
    public void AI_API_003_MissingCentralExceptionComposition_IsDetected()
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/Program.cs",
                "var app = builder.Build(); app.MapVendorEndpoints();"));

        AssertViolation(
            "AI-API-003",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    [Theory]
    [InlineData("Polly")]
    [InlineData("CircuitBreaker")]
    [InlineData("AddResilienceHandler")]
    [InlineData("CircuitBreakerStrategyOptions")]
    public void AI_ADDR_007_AddressCircuitBreakerMechanism_IsDetected(
        string circuitBreakerEvidence)
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Infrastructure.Vendor.Address/AddressResolutionAdapter.cs",
                circuitBreakerEvidence));

        AssertViolation(
            "AI-ADDR-007",
            ApiAddressStructuralRuleSet.Evaluate(catalog));
    }

    private static ArchitectureSourceCatalog Catalog(
        params SourceFileDescriptor[] files)
    {
        return ArchitectureSourceCatalog.FromFiles(files);
    }

    private static SourceFileDescriptor File(string path, string content)
    {
        return new SourceFileDescriptor(path, content);
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
