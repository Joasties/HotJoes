using HotJoes.Application.Address;
using HotJoes.Application.Vendor;
using HotJoes.Infrastructure.Vendor.Address;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiCompositionRootTests
{
    [Fact]
    public void AI_RUNTIME_007_ComposesAuthoritativeAddressAndVendorServices()
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
        Assert.IsType<RetrieveRegisteredVendorService>(
            scope.ServiceProvider.GetRequiredService<IRetrieveRegisteredVendorService>());
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

    private static IConfiguration CreateConfiguration(string cataloguePath)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:VendorDatabase"] =
                    "Host=localhost;Database=hotjoes;Username=hotjoes;Password=synthetic",
                ["AddressBootstrap:CataloguePath"] = cataloguePath
            })
            .Build();
    }

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
