using HotJoes.Api.Vendor.Configuration;
using HotJoes.Application.Address;
using HotJoes.Application.Community;
using HotJoes.Application.Compliance;
using HotJoes.Application.Vendor;
using HotJoes.Infrastructure.Community.Persistence;
using HotJoes.Infrastructure.Vendor.Persistence;
using HotJoes.Infrastructure.Vendor.Address;
using HotJoes.Infrastructure.Vendor.Compliance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiCompositionRootTests
{
    [Fact]
    public void AI_RUNTIME_007_ComposesAuthoritativeApplicationServices()
    {
        string cataloguePath = WriteValidCatalogue();
        IConfiguration configuration = CreateConfiguration(cataloguePath);
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddVendorApiComposition(configuration, Directory.GetCurrentDirectory());

        using ServiceProvider provider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        using IServiceScope scope = provider.CreateScope();

        Assert.IsType<StubAddressApplication>(
            scope.ServiceProvider.GetRequiredService<IAddressResolutionService>());
        Assert.IsType<AddressResolutionAdapter>(
            scope.ServiceProvider.GetRequiredService<IAddressResolver>());
        Assert.IsType<RegisterVendorService>(
            scope.ServiceProvider.GetRequiredService<IRegisterVendorService>());
        Assert.IsType<StubComplianceApplication>(
            scope.ServiceProvider.GetRequiredService<IComplianceDeterminationService>());
        Assert.IsType<ComplianceDeterminationAdapter>(
            scope.ServiceProvider.GetRequiredService<IComplianceDeterminationPort>());
        Assert.IsType<DetermineRequiredLicenceTypesService>(
            scope.ServiceProvider.GetRequiredService<IDetermineRequiredLicenceTypesService>());
        Assert.IsType<RetrieveRegisteredVendorService>(
            scope.ServiceProvider.GetRequiredService<IRetrieveRegisteredVendorService>());
        Assert.IsType<PostgreSqlVendorRegistrationVerificationAdapter>(
            scope.ServiceProvider.GetRequiredService<IVendorRegistrationVerificationPort>());
        Assert.IsType<PostgreSqlCommunityParticipationCommitter>(
            scope.ServiceProvider.GetRequiredService<ICommunityParticipationCommitter>());
        Assert.IsType<JoinCommunityService>(
            scope.ServiceProvider.GetRequiredService<IJoinCommunityService>());
    }

    [Fact]
    public void AI_RUNTIME_004_MissingCommunityDatabaseConfigurationFailsClosed()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:VendorDatabase"] =
                    "Host=localhost;Database=hotjoes;Username=hotjoes;Password=synthetic",
                ["AddressBootstrap:CataloguePath"] = WriteValidCatalogue()
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();

        Assert.Throws<InvalidOperationException>(() =>
            services.AddVendorApiComposition(configuration, Directory.GetCurrentDirectory()));
    }

    [Fact]
    public void AI_RUNTIME_007_MissingCatalogueFailsClosedWhenCapabilityIsBuilt()
    {
        IConfiguration configuration = CreateConfiguration("missing-catalogue.json");
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddVendorApiComposition(configuration, Directory.GetCurrentDirectory());

        using ServiceProvider provider = services.BuildServiceProvider();
        Assert.Throws<FileNotFoundException>(
            () => provider.GetRequiredService<IAddressResolutionService>());
    }

    private static IConfiguration CreateConfiguration(string cataloguePath) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:VendorDatabase"] =
                    "Host=localhost;Database=hotjoes;Username=hotjoes;Password=synthetic",
                ["ConnectionStrings:CommunityDatabase"] =
                    "Host=localhost;Database=hotjoes_community;Username=hotjoes;Password=synthetic",
                ["AddressBootstrap:CataloguePath"] = cataloguePath
            })
            .Build();

    private static string WriteValidCatalogue()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"hotjoes-address-{Guid.NewGuid():N}.json");
        File.WriteAllText(path, """
            {
              "revision": "epic1-address-catalogue-v1",
              "entries": [{
                "reference": "addr-restaurant-001",
                "tradingLocation": "restaurant",
                "canonicalAddressId": "canonical-001",
                "addressLine2": "1 Test Street",
                "postTown": "London",
                "postcode": "SE1 1AA",
                "foodRegistrationAuthority": "Test Authority"
              }]
            }
            """);
        return path;
    }
}
