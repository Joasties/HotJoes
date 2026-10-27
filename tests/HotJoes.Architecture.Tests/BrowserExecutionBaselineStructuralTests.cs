using System.Text.Json;

namespace HotJoes.Architecture.Tests;

public sealed class BrowserExecutionBaselineStructuralTests
{
    private const string BrowserProject = "tests/HotJoes.BrowserTests";

    [Fact]
    public void AI_BROWSER_001_PlaywrightAndBrowserBaselineArePinnedTogether()
    {
        using JsonDocument package = ReadJson($"{BrowserProject}/package.json");
        using JsonDocument lockFile = ReadJson($"{BrowserProject}/package-lock.json");
        using JsonDocument matrix = ReadJson($"{BrowserProject}/browser-matrix.json");

        string declaredVersion = RequiredString(
            package.RootElement.GetProperty("devDependencies"),
            "@playwright/test");
        string lockedVersion = RequiredString(
            lockFile.RootElement
                .GetProperty("packages")
                .GetProperty("node_modules/@playwright/test"),
            "version");
        string matrixVersion = RequiredString(matrix.RootElement, "playwrightVersion");

        Assert.Equal("1.64.1", declaredVersion);
        Assert.Equal(declaredVersion, lockedVersion);
        Assert.Equal(declaredVersion, matrixVersion);

        string packageSource = Read($"{BrowserProject}/package.json");
        Assert.Contains("verify:browser-baseline", packageSource);
        Assert.Contains("support/verify-browser-baseline.mjs", packageSource);
    }

    [Fact]
    public void AI_BROWSER_003_MatrixDeclaresSupportedEmulatedContexts()
    {
        using JsonDocument matrix = ReadJson($"{BrowserProject}/browser-matrix.json");
        JsonElement contexts = matrix.RootElement.GetProperty("contexts");

        Assert.Equal(JsonValueKind.Array, contexts.ValueKind);
        Assert.NotEmpty(contexts.EnumerateArray());

        foreach (JsonElement context in contexts.EnumerateArray())
        {
            Assert.False(string.IsNullOrWhiteSpace(RequiredString(context, "name")));
            Assert.Equal("chromium", RequiredString(context, "browser"));
            Assert.Equal("emulated", RequiredString(context, "evidenceKind"));

            JsonElement viewport = context.GetProperty("viewport");
            Assert.True(viewport.GetProperty("width").GetInt32() > 0);
            Assert.True(viewport.GetProperty("height").GetInt32() > 0);
        }

        Assert.Contains(
            matrix.RootElement.GetProperty("evidenceLimitations")
                .EnumerateArray()
                .Select(value => value.GetString()),
            value => string.Equals(
                value,
                "Browser emulation is not physical-device evidence.",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AI_BROWSER_003_PlaywrightConfigurationConsumesTheControlledMatrix()
    {
        string configuration = Read($"{BrowserProject}/playwright.config.ts");

        Assert.Contains("browser-matrix.json", configuration);
        Assert.Contains("matrix.contexts", configuration);
        Assert.DoesNotContain(
            "devices[\"Desktop Chrome\"]",
            configuration,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AI_CI_001_And_002_BrowserGateVerifiesAndExecutesTheControlledMatrix()
    {
        string workflow = Read(".github/workflows/build.yml");
        string browserJob = BrowserJob(workflow);
        string packageSource = Read($"{BrowserProject}/package.json");

        Assert.Contains("npm run verify:browser-baseline", browserJob);
        Assert.Contains("npm run test:ci", browserJob);
        Assert.DoesNotContain("--headed", browserJob, StringComparison.Ordinal);
        Assert.Contains("\"test:ci\"", packageSource);
        Assert.Contains("--project=", packageSource);
        Assert.Contains("retries: process.env.CI ? 1 : 0", Read(
            $"{BrowserProject}/playwright.config.ts"));
        Assert.Contains("failOnFlakyTests: Boolean(process.env.CI)", Read(
            $"{BrowserProject}/playwright.config.ts"));
        Assert.Contains("retention-days: 7", browserJob);
    }

    private static JsonDocument ReadJson(string relativePath) =>
        JsonDocument.Parse(Read(relativePath));

    private static string RequiredString(JsonElement element, string propertyName)
    {
        string? value = element.GetProperty(propertyName).GetString();
        Assert.False(string.IsNullOrWhiteSpace(value));
        return value!;
    }

    private static string BrowserJob(string workflow)
    {
        const string marker = "  browser-tests:";
        int start = workflow.IndexOf(marker, StringComparison.Ordinal);
        Assert.True(start >= 0, "The browser-tests workflow job is absent.");
        return workflow[start..];
    }

    private static string Read(string relativePath) =>
        File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "HotJoes.sln")))
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate HotJoes.sln.");
    }
}
