using HotJoes.Application.Vendor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorApiFactory : WebApplicationFactory<Program>
{
    public StubRegisterVendorService Registration { get; } = new();

    public StubRetrieveRegisteredVendorService Retrieval { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IRegisterVendorService>();
            services.RemoveAll<IRetrieveRegisteredVendorService>();
            services.AddSingleton<IRegisterVendorService>(Registration);
            services.AddSingleton<IRetrieveRegisteredVendorService>(Retrieval);
        });
    }
}
