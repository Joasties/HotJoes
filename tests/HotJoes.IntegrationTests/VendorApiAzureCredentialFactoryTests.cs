using Azure.Core;
using Azure.Identity;
using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiAzureCredentialFactoryTests
{
    [Fact]
    public void CreateProduction_SystemAssignedIdentity_ReturnsManagedIdentityCredential()
    {
        var factory = new VendorApiAzureCredentialFactory();

        TokenCredential credential = factory.CreateProduction();

        Assert.IsType<ManagedIdentityCredential>(credential);
    }

    [Fact]
    public void CreateProduction_UserAssignedIdentity_ReturnsManagedIdentityCredential()
    {
        var factory = new VendorApiAzureCredentialFactory();

        TokenCredential credential = factory.CreateProduction(
            "22222222-2222-2222-2222-222222222222");

        Assert.IsType<ManagedIdentityCredential>(credential);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateProduction_BlankUserAssignedIdentity_RejectsSelection(
        string invalidClientId)
    {
        var factory = new VendorApiAzureCredentialFactory();

        Assert.Throws<ArgumentException>(
            () => factory.CreateProduction(invalidClientId));
    }

    [Fact]
    public void CreateDevelopment_UsesExplicitDeveloperCredentialChain()
    {
        var factory = new VendorApiAzureCredentialFactory();

        TokenCredential credential = factory.CreateDevelopment();

        Assert.IsType<DefaultAzureCredential>(credential);
    }

    [Fact]
    public void Factory_HasNoEnvironmentDrivenImplicitCredentialSelection()
    {
        string[] publicMethodNames = typeof(VendorApiAzureCredentialFactory)
            .GetMethods()
            .Where(method => method.DeclaringType ==
                typeof(VendorApiAzureCredentialFactory))
            .Select(method => method.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[] { "CreateDevelopment", "CreateProduction" },
            publicMethodNames);
    }
}
