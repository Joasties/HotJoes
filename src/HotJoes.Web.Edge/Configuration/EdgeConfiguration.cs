using Yarp.ReverseProxy.Configuration;

namespace HotJoes.Web.Edge.Configuration;

public static class EdgeConfiguration
{
    public static IServiceCollection AddHotJoesEdge(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        EdgeOptions options = configuration
            .GetRequiredSection(EdgeOptions.SectionName)
            .Get<EdgeOptions>() ?? throw new InvalidOperationException(
                "Mandatory Edge configuration is missing.");

        if (!Uri.TryCreate(options.VendorApiBaseAddress, UriKind.Absolute,
                out Uri? vendorApi) ||
            (vendorApi.Scheme != Uri.UriSchemeHttp &&
             vendorApi.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "Edge:VendorApiBaseAddress must be an absolute HTTP or HTTPS URI.");
        }

        RouteConfig[] routes =
        [
            Route("address-search", "/address-search", HttpMethods.Get),
            Route(
                "community-participations",
                "/community-participations",
                HttpMethods.Post),
            Route(
                "determine-required-licence-types",
                "/vendor-registration/required-licence-types",
                HttpMethods.Post),
            Route("register-vendor", "/vendors", HttpMethods.Post),
            Route(
                "retrieve-registered-vendor",
                "/vendors/{vendorId}",
                HttpMethods.Get)
        ];

        ClusterConfig[] clusters =
        [
            new ClusterConfig
            {
                ClusterId = "vendor-api",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["vendor-api"] = new() { Address = vendorApi.AbsoluteUri }
                }
            }
        ];

        services.AddReverseProxy().LoadFromMemory(routes, clusters);
        services.AddHttpClient("vendor-api-readiness", client =>
        {
            client.BaseAddress = vendorApi;
            client.Timeout = TimeSpan.FromSeconds(2);
        });
        return services;
    }

    private static RouteConfig Route(
        string routeId,
        string path,
        string method) =>
        new()
        {
            RouteId = routeId,
            ClusterId = "vendor-api",
            Match = new RouteMatch
            {
                Path = path,
                Methods = [method]
            },
            Transforms = ForwardingHeaderTransforms()
        };

    private static IReadOnlyList<IReadOnlyDictionary<string, string>>
        ForwardingHeaderTransforms() =>
        [
            new Dictionary<string, string> { ["X-Forwarded"] = "Off" }
        ];
}
