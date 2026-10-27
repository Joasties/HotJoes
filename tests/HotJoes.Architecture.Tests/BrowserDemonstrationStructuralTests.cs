using System.Text.Json;

namespace HotJoes.Architecture.Tests;

public sealed class BrowserDemonstrationStructuralTests
{
    [Fact]
    public void AI_BROWSER_003_DemonstrationHasOneVisibleDedicatedCommand()
    {
        using JsonDocument package = JsonDocument.Parse(
            Read("tests/HotJoes.BrowserTests/package.json"));

        string? command = package.RootElement
            .GetProperty("scripts")
            .GetProperty("demo")
            .GetString();

        Assert.NotNull(command);
        Assert.Contains("representative-demonstration.spec.ts", command);
        Assert.Contains("--headed", command);
        Assert.Contains("--project=chromium-demo", command);
    }

    [Fact]
    public void AI_BROWSER_004_RuntimeIdentityIsExplicitAndNotSharedByDefault()
    {
        string setup = Read(
            "tests/HotJoes.BrowserTests/support/global-setup.ts");

        Assert.Contains("HOTJOES_BROWSER_RUN_ID", setup);
        Assert.DoesNotContain(
            "?? \"hotjoes-browser-tests\"",
            setup,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AI_BROWSER_002_And_005_BrowserSourcesUseSemanticInteractionWithoutSleeps()
    {
        string source = ReadBrowserTypeScript();

        Assert.DoesNotContain(".locator(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("waitForTimeout(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("globalThis.setTimeout(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("window.setTimeout(", source, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "await new Promise(resolve => setTimeout(",
            source,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AI_BROWSER_008_VideoCaptureIsDisabledByDefault()
    {
        string configuration = Read(
            "tests/HotJoes.BrowserTests/playwright.config.ts");

        Assert.Contains("video: \"off\"", configuration);
        Assert.Contains("trace: \"retain-on-failure\"", configuration);
        Assert.Contains("screenshot: \"only-on-failure\"", configuration);
    }

    [Fact]
    public void AI_BROWSER_003_DemonstrationCatalogueIsControlledAndComplete()
    {
        string catalogue = Read(
            "tests/HotJoes.BrowserTests/support/demonstration-catalogue.ts");

        Assert.Contains("DEMONSTRATION_CATALOGUE", catalogue);
        Assert.Contains("repeatEachIndex % DEMONSTRATION_CATALOGUE.length", catalogue);
        Assert.DoesNotContain(
            "HOTJOES_BROWSER_DEMO_CATALOGUE_INDEX",
            catalogue,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AI_BROWSER_002_And_005_DemonstrationChoreographyUsesActionsNotSleeps()
    {
        string demonstration = Read(
            "tests/HotJoes.BrowserTests/support/representative-demonstration.ts");

        Assert.Contains("showInformation", demonstration);
        Assert.Contains("presentOptions", demonstration);
        Assert.Contains("getByRole", demonstration);
        Assert.Contains("getByLabel", demonstration);
        Assert.DoesNotContain("waitForTimeout(", demonstration, StringComparison.Ordinal);
        Assert.DoesNotContain("setTimeout(", demonstration, StringComparison.Ordinal);
    }

    private static string ReadBrowserTypeScript()
    {
        string root = FindRepositoryRoot();
        string browserRoot = Path.Combine(root, "tests", "HotJoes.BrowserTests");

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
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate HotJoes.sln.");
    }
}
