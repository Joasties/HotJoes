using HotJoes.Application.Community;
using HotJoes.Application.Vendor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorApiFactory : WebApplicationFactory<Program>
{
    public StubRegisterVendorService Registration { get; } = new();
    public StubDetermineRequiredLicenceTypesService Determination { get; } = new();
    public StubRetrieveRegisteredVendorService Retrieval { get; } = new();
    public StubJoinCommunityService Community { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.UseSetting(
            "ConnectionStrings:VendorDatabase",
            "Host=127.0.0.1;Port=1;Database=hotjoes_test;Username=test;Password=test;Timeout=1");
        builder.UseSetting(
            "ConnectionStrings:CommunityDatabase",
            "Host=127.0.0.1;Port=1;Database=hotjoes_community_test;Username=test;Password=test;Timeout=1");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IRegisterVendorService>();
            services.RemoveAll<IDetermineRequiredLicenceTypesService>();
            services.RemoveAll<IRetrieveRegisteredVendorService>();
            services.RemoveAll<IJoinCommunityService>();
            services.AddSingleton<IRegisterVendorService>(Registration);
            services.AddSingleton<IDetermineRequiredLicenceTypesService>(Determination);
            services.AddSingleton<IRetrieveRegisteredVendorService>(Retrieval);
            services.AddSingleton<IJoinCommunityService>(Community);
        });
    }
}
