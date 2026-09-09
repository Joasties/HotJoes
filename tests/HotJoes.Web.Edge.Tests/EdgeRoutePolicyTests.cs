using HotJoes.Web.Edge.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;

namespace HotJoes.Web.Edge.Tests;

public sealed class EdgeRoutePolicyTests
{
    [Fact]
    public async Task AI_GW_001_OnlyApprovedVendorRoutesAreConfigured()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddHotJoesEdge(Configuration("http://vendor-api:8080"));

        await using ServiceProvider provider = services.BuildServiceProvider();
        IProxyConfig config = provider
            .GetRequiredService<IProxyConfigProvider>()
            .GetConfig();

        Assert.Collection(
            config.Routes.OrderBy(route => route.RouteId),
            route => AssertRoute(route, "register-vendor", "/vendors", "POST"),
            route => AssertRoute(
                route,
                "retrieve-registered-vendor",
                "/vendors/{vendorId}",
                "GET"));
        Assert.Single(config.Clusters);
        Assert.Equal(
            "http://vendor-api:8080/",
            config.Clusters[0].Destinations!["vendor-api"].Address);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("vendor-api:8080")]
    [InlineData("file:///tmp/vendor-api")]
    public void AI_RUNTIME_004_InvalidVendorApiAuthorityFailsClosed(
        string? authority)
    {
        ServiceCollection services = new();

        Assert.Throws<InvalidOperationException>(() =>
            services.AddHotJoesEdge(Configuration(authority)));
    }

    private static IConfiguration Configuration(string? authority)
    {
        Dictionary<string, string?> values = new();
        if (authority is not null)
        {
            values["Edge:VendorApiBaseAddress"] = authority;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private static void AssertRoute(
        RouteConfig route,
        string routeId,
        string path,
        string method)
    {
        Assert.Equal(routeId, route.RouteId);
        Assert.Equal("vendor-api", route.ClusterId);
        Assert.Equal(path, route.Match.Path);
        Assert.Equal([method], route.Match.Methods);
        IReadOnlyDictionary<string, string> transform =
            Assert.Single(route.Transforms!);
        Assert.Equal("Off", transform["X-Forwarded"]);
    }
}
