namespace HotJoes.Architecture.Tests;

public sealed class SecretExposureStructuralRuleTests
{
    [Fact]
    public void AI_SEC_001_CurrentProductionSources_HaveNoStructuralExposure()
    {
        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.EvaluateCurrent();

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData("Password")]
    [InlineData("ClientSecret")]
    [InlineData("ConnectionString")]
    [InlineData("PrivateKey")]
    [InlineData("AccessToken")]
    public void AI_SEC_001_ConfigurationContainsSecretValue_IsDetected(
        string key)
    {
        string canary = "synthetic-" + "redaction-canary";
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Api.Vendor/appsettings.json",
                $$"""
                {
                  "{{key}}": "{{canary}}"
                }
                """));

        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.Evaluate(sources);

        AssertViolation(violations);
    }

    [Theory]
    [InlineData("DatabasePassword")]
    [InlineData("DatabaseConnectionString")]
    [InlineData("SigningPrivateKey")]
    [InlineData("ServiceAccessToken")]
    public void AI_SEC_001_ConfigurationSnapshotOwnsSecretValue_IsDetected(
        string propertyName)
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Api.Vendor/Configuration/" +
                    "VendorApiConfigurationSnapshot.cs",
                $$"""
                namespace HotJoes.Api.Vendor.Configuration;

                public sealed record VendorApiConfigurationSnapshot(
                    string {{propertyName}});
                """));

        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.Evaluate(sources);

        AssertViolation(violations);
    }

    [Theory]
    [InlineData("Password")]
    [InlineData("ClientSecret")]
    [InlineData("AccessToken")]
    public void AI_SEC_001_ApiContractExposesSecretValue_IsDetected(
        string propertyName)
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Api.Vendor/Contracts/SecretResponse.cs",
                $$"""
                namespace HotJoes.Api.Vendor.Contracts;

                public sealed record SecretResponse(string {{propertyName}});
                """));

        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.Evaluate(sources);

        AssertViolation(violations);
    }

    [Fact]
    public void AI_SEC_001_LoggingSecretBearingValue_IsDetected()
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Api.Vendor/Configuration/ConfigurationWorker.cs",
                """
                logger.LogError(
                    "Configuration failed for {Credential}",
                    credential);
                """));

        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.Evaluate(sources);

        AssertViolation(violations);
    }

    [Fact]
    public void AI_SEC_001_CredentialBearingUri_IsDetectedWithoutDisclosure()
    {
        string canary = "synthetic-" + "redaction-canary";
        string content =
            "const string endpoint = \"postgresql://vendor:" + canary +
            "@database.internal/hotjoes\";";
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Api.Vendor/Program.cs",
                content));

        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.Evaluate(sources);

        ArchitectureViolation violation = AssertViolation(violations);
        Assert.DoesNotContain(
            canary,
            violation.Description,
            StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("DatabaseSecretName")]
    [InlineData("DatabaseSecretReference")]
    [InlineData("SigningKeyVaultUri")]
    public void AI_SEC_001_NonSecretReferenceMetadata_IsPermitted(
        string propertyName)
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Api.Vendor/Configuration/" +
                    "VendorApiConfigurationSnapshot.cs",
                $$"""
                namespace HotJoes.Api.Vendor.Configuration;

                public sealed record VendorApiConfigurationSnapshot(
                    string {{propertyName}});
                """));

        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.Evaluate(sources);

        Assert.Empty(violations);
    }

    [Fact]
    public void AI_SEC_001_ManagedIdentityCredentialUse_IsPermitted()
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Infrastructure.Configuration.Azure/" +
                    "AzureConfigurationCredentialFactory.cs",
                """
                using Azure.Identity;

                namespace HotJoes.Infrastructure.Configuration.Azure;

                internal static class AzureConfigurationCredentialFactory
                {
                    public static DefaultAzureCredential Create() => new();
                }
                """));

        IReadOnlyList<ArchitectureViolation> violations =
            SecretExposureStructuralRuleSet.Evaluate(sources);

        Assert.Empty(violations);
    }

    private static ArchitectureSourceCatalog Sources(
        params SourceFileDescriptor[] files)
    {
        return ArchitectureSourceCatalog.FromFiles(files);
    }

    private static ArchitectureViolation AssertViolation(
        IReadOnlyList<ArchitectureViolation> violations)
    {
        return Assert.Single(
            violations,
            violation => violation.ObligationId == "AI-SEC-001");
    }
}
