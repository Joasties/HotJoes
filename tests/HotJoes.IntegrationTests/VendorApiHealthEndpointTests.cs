using System.Net;
using HotJoes.Api.Vendor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiHealthEndpointTests
{
    [Fact]
    public async Task AI_HEALTH_001_LivenessIsDependencyFree()
    {
        await using var factory = new ApiFactory();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AI_HEALTH_001_ReadinessRequiresPostgreSql()
    {
        await using var factory = new ApiFactory();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.GetAsync("/health/ready");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    private sealed class ApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting(
                "ConnectionStrings:VendorDatabase",
                "Host=127.0.0.1;Port=1;Database=hotjoes;Username=test;Password=test;Timeout=1");
        }
    }
}
