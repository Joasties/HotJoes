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

        if (!Uri.TryCreate(
                options.VendorApiBaseAddress,
                UriKind.Absolute,
                out Uri? vendorApi) ||
            (vendorApi.Scheme != Uri.UriSchemeHttp &&
             vendorApi.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "Edge:VendorApiBaseAddress must be an absolute HTTP or HTTPS URI.");
        }

        RouteConfig[] routes =
        [
            new RouteConfig
            {
                RouteId = "register-vendor",
                ClusterId = "vendor-api",
                Match = new RouteMatch
                {
                    Path = "/vendors",
                    Methods = [HttpMethods.Post]
                },
                Transforms = ForwardingHeaderTransforms()
            },
            new RouteConfig
            {
                RouteId = "retrieve-registered-vendor",
                ClusterId = "vendor-api",
                Match = new RouteMatch
                {
                    Path = "/vendors/{vendorId}",
                    Methods = [HttpMethods.Get]
                },
                Transforms = ForwardingHeaderTransforms()
            }
        ];

        ClusterConfig[] clusters =
        [
            new ClusterConfig
            {
                ClusterId = "vendor-api",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["vendor-api"] = new()
                    {
                        Address = vendorApi.AbsoluteUri
                    }
                }
            }
        ];

        services.AddReverseProxy().LoadFromMemory(routes, clusters);
        return services;
    }

    private static IReadOnlyList<IReadOnlyDictionary<string, string>>
        ForwardingHeaderTransforms()
    {
        return
        [
            new Dictionary<string, string>
            {
                ["X-Forwarded"] = "Off"
            }
        ];
    }
}
