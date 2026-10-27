namespace HotJoes.Architecture.Tests;

public sealed class BrowserExecutionPolicyStructuralTests
{
    [Fact]
    public void AI_BROWSER_002_And_005_SourcesUseSemanticInteractionAndObservableSynchronization()
    {
        string source = ReadBrowserTypeScript();
        Assert.DoesNotContain(".locator(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("$$(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("xpath=", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("waitForTimeout(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("globalThis.setTimeout(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("window.setTimeout(", source, StringComparison.Ordinal);

        string representative = Read(
            "tests/HotJoes.BrowserTests/support/representative-demonstration.ts");
        Assert.Contains("page.waitForResponse", representative);
        Assert.Contains("response.request().method() === \"POST\"", representative);
        Assert.Contains("response.status() === 201", representative);
        Assert.Contains("getByRole", representative);
        Assert.Contains("getByLabel", representative);
    }

    [Fact]
    public void AI_BROWSER_006_LocalExecutionHasNoAutomaticRetryAndCiRetryIsVisibleAndGateFailing()
    {
        string configuration = Read(
            "tests/HotJoes.BrowserTests/playwright.config.ts");
        Assert.Contains("retries: process.env.CI ? 1 : 0", configuration);
        Assert.Contains("failOnFlakyTests: Boolean(process.env.CI)", configuration);
        Assert.DoesNotContain("retries: 2", configuration, StringComparison.Ordinal);
        Assert.DoesNotContain("retries: 3", configuration, StringComparison.Ordinal);
    }

    [Fact]
    public void AI_BROWSER_006_DiagnosticRetryUsesASeparateDeterministicIdentity()
    {
        string lifecycle = Read(
            "tests/HotJoes.BrowserTests/support/browser-runtime-lifecycle.mjs");
        string browserTests = ReadBrowserTypeScript();
        Assert.Contains("retryIndex", lifecycle);
        Assert.Contains("retryIndex: testInfo.retry", browserTests);
    }

    [Fact]
    public void AI_CI_002_BrowserGatePinsInstallationRunsTestsAndRetainsFailureEvidence()
    {
        string workflow = Read(".github/workflows/build.yml");
        string browserJob = BrowserJob(workflow);
        Assert.Contains("npm ci", browserJob);
        Assert.Contains("npx playwright install --with-deps chromium", browserJob);
        Assert.Contains("npm run test:ci", browserJob);
        Assert.Contains("CI: \"true\"", browserJob);
        Assert.Contains("if: failure()", browserJob);
        Assert.Contains("playwright-report", browserJob);
        Assert.Contains("test-results", browserJob);
        Assert.Contains("retention-days: 7", browserJob);
    }

    [Fact]
    public void AI_BROWSER_008_DiagnosticsAreFailureOnlyPurposefulAndBodyCaptureIsDisabled()
    {
        string configuration = Read(
            "tests/HotJoes.BrowserTests/playwright.config.ts");
        string browserJob = BrowserJob(Read(".github/workflows/build.yml"));

        Assert.Contains("trace: \"retain-on-failure\"", configuration);
        Assert.Contains("screenshot: \"only-on-failure\"", configuration);
        Assert.Contains("video: \"off\"", configuration);
        Assert.DoesNotContain("recordHar", configuration, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("if: failure()", browserJob);
        Assert.Contains("retention-days: 7", browserJob);
        Assert.Contains("continue-on-error: true", browserJob);
    }

    [Fact]
    public void AI_BROWSER_008_DiagnosticEvidenceIsScannedBeforePublication()
    {
        string configuration = Read(
            "tests/HotJoes.BrowserTests/playwright.config.ts");
        string package = Read("tests/HotJoes.BrowserTests/package.json");
        string scanner = Read(
            "tests/HotJoes.BrowserTests/support/diagnostic-evidence.mjs");

        Assert.Contains("diagnostic-evidence-reporter.mjs", configuration);
        Assert.Contains("scan:diagnostics", package);
        Assert.Contains("scanDiagnosticEvidence", scanner);
        Assert.DoesNotContain("console.log", scanner, StringComparison.Ordinal);
        Assert.DoesNotContain("console.error", scanner, StringComparison.Ordinal);
    }

    [Fact]
    public void AI_BROWSER_006_ControlledInterceptionRemainsSeparateFromMandatoryJourney()
    {
        string representative = Read(
            "tests/HotJoes.BrowserTests/tests/representative-demonstration.spec.ts");
        string community = Read(
            "tests/HotJoes.BrowserTests/tests/community-participation.spec.ts");
        Assert.DoesNotContain("page.route(", representative, StringComparison.Ordinal);
        Assert.Contains("failure retains selections and permits a safe retry", community);
        Assert.Contains("page.route(\"**/community-participations\"", community);
        Assert.Contains("page.unroute(\"**/community-participations\"", community);
    }

    private static string BrowserJob(string workflow)
    {
        const string marker = "  browser-tests:";
        int start = workflow.IndexOf(marker, StringComparison.Ordinal);
        Assert.True(start >= 0, "The browser-tests workflow job is absent.");
        return workflow[start..];
    }

    private static string ReadBrowserTypeScript()
    {
        string browserRoot = Path.Combine(
            FindRepositoryRoot(), "tests", "HotJoes.BrowserTests");
        return string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(browserRoot, "*.ts", SearchOption.AllDirectories)
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
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new InvalidOperationException("Could not locate HotJoes.sln.");
    }
}
