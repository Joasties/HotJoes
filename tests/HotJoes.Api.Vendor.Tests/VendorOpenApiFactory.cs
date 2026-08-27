using HotJoes.Application.Vendor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorOpenApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IRegisterVendorService>();
            services.RemoveAll<IRetrieveRegisteredVendorService>();
            services.AddSingleton<IRegisterVendorService>(
                new StubRegisterVendorService());
            services.AddSingleton<IRetrieveRegisteredVendorService>(
                new StubRetrieveRegisteredVendorService());
        });
    }
}
