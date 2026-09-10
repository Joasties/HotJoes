namespace HotJoes.Architecture.Tests;

public sealed class ComposeRuntimeStructuralTests
{
    private static readonly string[] RequiredServices =
    [
        "postgres:", "rabbitmq:", "database-migrations:", "vendor-api:",
        "vendor-relay:", "compliance-consumer:", "web-edge:"
    ];

    [Fact]
    public void AI_RUNTIME_001_ComposeContainsRequiredRuntimeServices()
    {
        string compose = Read("compose.yaml");
        foreach (string service in RequiredServices)
        {
            Assert.Contains(service, compose, StringComparison.Ordinal);
        }
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
        string cleanup = Read("scripts/compose-down.sh");
        Assert.Contains("down --volumes --remove-orphans", cleanup);
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
