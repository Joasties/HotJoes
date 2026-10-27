namespace HotJoes.Architecture.Tests;

public sealed class BrowserRuntimeIsolationStructuralTests
{
    [Fact]
    public void AI_BROWSER_004_HarnessDelegatesAllocationAndLifecycle()
    {
        string launcher = Read(
            "tests/HotJoes.BrowserTests/support/run-playwright.mjs");
        string setup = Read(
            "tests/HotJoes.BrowserTests/support/global-setup.ts");

        Assert.Contains("browser-runtime-lifecycle.mjs", launcher);
        Assert.Contains("allocateBrowserRuntime", launcher);
        Assert.DoesNotContain("randomUUID", launcher, StringComparison.Ordinal);
        Assert.DoesNotContain("net.createServer", launcher, StringComparison.Ordinal);

        Assert.Contains("browser-runtime-lifecycle.mjs", setup);
        Assert.Contains("createBrowserRuntimeLifecycle", setup);
        Assert.DoesNotContain("execFileSync", setup, StringComparison.Ordinal);

        string browserTests = ReadBrowserTypeScript();
        Assert.Contains("createBrowserTestIdentity", browserTests);
        Assert.DoesNotContain("Math.random()", browserTests, StringComparison.Ordinal);
        Assert.DoesNotContain("Date.now()", browserTests, StringComparison.Ordinal);
    }

    [Fact]
    public void AI_BROWSER_007_IsolatedTestsAreParallelAndNonIsolatableCohortsAreExplicitlySerialized()
    {
        string configuration = Read(
            "tests/HotJoes.BrowserTests/playwright.config.ts");
        string parallelEvidence = Read(
            "tests/HotJoes.BrowserTests/tests/parallel-isolation.spec.ts");
        string communityJourney = Read(
            "tests/HotJoes.BrowserTests/tests/community-participation.spec.ts");

        Assert.Contains("fullyParallel: true", configuration);
        Assert.DoesNotContain("workers: 1", configuration, StringComparison.Ordinal);
        Assert.Contains("mode: \"parallel\"", parallelEvidence);
        Assert.Contains("test.describe.serial", communityJourney);
    }

    [Fact]
    public void AI_BROWSER_007_HarnessRejectsSyntheticIdentityCollisions()
    {
        string lifecycle = Read(
            "tests/HotJoes.BrowserTests/support/browser-runtime-lifecycle.mjs");
        string parallelEvidence = Read(
            "tests/HotJoes.BrowserTests/tests/parallel-isolation.spec.ts");

        Assert.Contains("createBrowserTestIdentityRegistry", lifecycle);
        Assert.Contains("Browser test identity collision detected.", lifecycle);
        Assert.Contains("registry.reserve(reservedIdentity)", parallelEvidence);
        Assert.DoesNotContain("test.skip", parallelEvidence, StringComparison.Ordinal);
        Assert.DoesNotContain("test.fixme", parallelEvidence, StringComparison.Ordinal);
    }

    [Fact]
    public void AI_RUNTIME_006_ComposeResourcesRemainProjectScoped()
    {
        string compose = Read("compose.yaml");

        Assert.Contains(
            "name: ${HOTJOES_COMPOSE_PROJECT:?HOTJOES_COMPOSE_PROJECT is required}",
            compose);
        Assert.Contains("${HOTJOES_EDGE_PORT:-8080}:8080", compose);
        Assert.Contains("vendor-postgres-data:", compose);
        Assert.Contains("rabbitmq-data:", compose);
        Assert.DoesNotContain("container_name:", compose, StringComparison.Ordinal);
        Assert.DoesNotContain("external: true", compose, StringComparison.Ordinal);
    }

    [Fact]
    public void AI_RUNTIME_006_LifecycleDefinesBoundedIdempotentCleanupAndSafeReporting()
    {
        string lifecycle = Read(
            "tests/HotJoes.BrowserTests/support/browser-runtime-lifecycle.mjs");

        Assert.Contains("allocateBrowserRuntime", lifecycle);
        Assert.Contains("createBrowserTestIdentity", lifecycle);
        Assert.Contains("createBrowserRuntimeLifecycle", lifecycle);
        Assert.Contains("--volumes", lifecycle);
        Assert.Contains("--remove-orphans", lifecycle);
        Assert.Contains("cleanupFailed", lifecycle);
    }

    private static string Read(string relativePath) =>
        File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static string ReadBrowserTypeScript()
    {
        string browserRoot = Path.Combine(
            FindRepositoryRoot(),
            "tests",
            "HotJoes.BrowserTests");

        return string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(
                browserRoot,
                "*.ts",
                SearchOption.AllDirectories)
                .Where(path => !path.Contains(
                    $"{Path.DirectorySeparatorChar}node_modules{Path.DirectorySeparatorChar}",
                    StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(File.ReadAllText));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "HotJoes.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate HotJoes.sln.");
    }
}
