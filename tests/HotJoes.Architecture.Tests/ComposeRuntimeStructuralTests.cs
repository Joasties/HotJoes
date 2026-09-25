namespace HotJoes.Architecture.Tests;

public sealed class ComposeRuntimeStructuralTests
{
    private static readonly string[] RequiredServices =
    [
        "postgres:", "rabbitmq:", "database-migrations:", "vendor-api:",
        "vendor-relay:", "compliance-consumer:", "community-relay:",
        "community-consumer:", "web-edge:"
    ];

    [Fact]
    public void AI_RUNTIME_001_ComposeContainsRequiredRuntimeServices()
    {
        string compose = Read("compose.yaml");
        foreach (string service in RequiredServices)
            Assert.Contains(service, compose, StringComparison.Ordinal);
    }

    [Fact]
    public void AI_RUNTIME_002_And_005_StartupIsHealthAndMigrationGated()
    {
        string compose = Read("compose.yaml");
        Assert.Contains("condition: service_healthy", compose);
        Assert.Contains("condition: service_completed_successfully", compose);
        Assert.DoesNotContain("sleep ", compose, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AI_RUNTIME_004_OnlyEdgePublishesAHostPort()
    {
        string compose = Read("compose.yaml");
        Assert.Equal(1, Count(compose, "ports:"));
        Assert.Contains("127.0.0.1:${HOTJOES_EDGE_PORT:-8080}:8080", compose);
    }

    [Fact]
    public void AI_RUNTIME_006_RuntimeUsesNamedVolumesAndBoundedCleanup()
    {
        string compose = Read("compose.yaml");
        Assert.Contains("vendor-postgres-data:", compose);
        Assert.Contains("rabbitmq-data:", compose);
        Assert.Contains("down --volumes --remove-orphans", Read("scripts/compose-down.sh"));
    }

    [Fact]
    public void AI_COMMUNITY_003_CommunityConsumerIsPrivateHealthAndDependencyGated() =>
        AssertPrivateWorker("community-consumer", "community-relay", "HotJoes.Worker.CommunityConsumer.csproj");

    [Fact]
    public void AI_RUNTIME_009_CommunityRelayIsPrivateHealthAndDependencyGated() =>
        AssertPrivateWorker("community-relay", "web-edge", "HotJoes.Worker.CommunityRelay.csproj");

    [Fact]
    public void AI_BROWSER_012_EdgeBuildsAndServesTheVendorClient()
    {
        string dockerfile = Read("src/HotJoes.Web.Edge/Dockerfile");
        Assert.Contains("node:24.15.0-bookworm", dockerfile);
        Assert.Contains("src/HotJoes.Web.Vendor", dockerfile);
        Assert.Contains("npm run build", dockerfile);

        string edge = Read("src/HotJoes.Web.Edge/Program.cs");
        Assert.Contains("UseDefaultFiles", edge);
        Assert.Contains("UseStaticFiles", edge);
        Assert.Contains("MapFallbackToFile", edge);
    }

    [Fact]
    public void AI_BROWSER_012_DevelopmentProxyExposesOnlyApprovedApiRoutes()
    {
        string[] expectedRoutes =
        [
            "/address-search",
            "/community-participations",
            "/vendor-registration/required-licence-types",
            "/vendors"
        ];

        using System.Text.Json.JsonDocument document =
            System.Text.Json.JsonDocument.Parse(
                Read("src/HotJoes.Web.Vendor/proxy.conf.json"));

        System.Text.Json.JsonProperty[] routes = document.RootElement
            .EnumerateObject()
            .OrderBy(route => route.Name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedRoutes, routes.Select(route => route.Name));

        foreach (System.Text.Json.JsonProperty route in routes)
        {
            Assert.Equal(
                "http://host.docker.internal:8080",
                route.Value.GetProperty("target").GetString());
            Assert.False(route.Value.GetProperty("secure").GetBoolean());
            Assert.False(route.Value.GetProperty("changeOrigin").GetBoolean());
        }
    }

    private static void AssertPrivateWorker(string serviceName, string nextServiceName, string project)
    {
        string compose = Read("compose.yaml");
        int start = compose.IndexOf($"  {serviceName}:", StringComparison.Ordinal);
        Assert.True(start >= 0, $"Compose service '{serviceName}' was not found.");
        int end = compose.IndexOf($"\n  {nextServiceName}:", start, StringComparison.Ordinal);
        Assert.True(end > start, $"Compose service '{serviceName}' has no bounded service section before '{nextServiceName}'.");
        string service = compose[start..end];
        Assert.Contains(project, service);
        Assert.Contains("condition: service_completed_successfully", service);
        Assert.Contains("condition: service_healthy", service);
        Assert.Contains("/health/ready", service);
        Assert.DoesNotContain("ports:", service);
    }

    private static int Count(string value, string fragment) =>
        value.Split(fragment, StringSplitOptions.None).Length - 1;

    private static string Read(string path)
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, path);
            if (File.Exists(candidate)) return File.ReadAllText(candidate);
            directory = directory.Parent;
        }
        throw new InvalidOperationException($"Could not locate '{path}'.");
    }
}
