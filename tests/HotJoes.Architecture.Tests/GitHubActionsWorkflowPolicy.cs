using System.Text.RegularExpressions;

namespace HotJoes.Architecture.Tests;

public static partial class GitHubActionsWorkflowPolicy
{
    private const string WorkflowRelativePath =
        ".github/workflows/build.yml";
    private const string ObligationId = "AI-CI-001/AI-CI-002";
    private const string SecurityObligationId = "AI-SEC-001";
    private const string ApprovedSecretScannerImage =
        "ghcr.io/gitleaks/gitleaks@sha256:" +
        "c00b6bd0aeb3071cbcb79009cb16a60dd9e0a7c60e2be9ab65d25e6bc8abbb7f";

    public static IReadOnlyList<ArchitectureViolation> EvaluateCurrent()
    {
        string repositoryRoot = FindRepositoryRoot();
        string workflowPath = Path.Combine(
            repositoryRoot,
            WorkflowRelativePath.Replace(
                '/',
                Path.DirectorySeparatorChar));

        if (!File.Exists(workflowPath))
        {
            return
            [
                Violation("Required GitHub Actions workflow does not exist.")
            ];
        }

        return Evaluate(File.ReadAllText(workflowPath));
    }

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        string workflow)
    {
        ArgumentNullException.ThrowIfNull(workflow);

        string normalized = workflow.Replace("\r\n", "\n");
        var violations = new List<ArchitectureViolation>();

        RequireTriggers(normalized, violations);
        RequireQualityGate(normalized, violations);
        RequireSecretScan(normalized, violations);
        RequireTestGates(normalized, violations);
        RequireSafeDiagnostics(normalized, violations);

        return violations;
    }

    private static void RequireTriggers(
        string workflow,
        ICollection<ArchitectureViolation> violations)
    {
        if (!PushMainTrigger().IsMatch(workflow))
        {
            violations.Add(Violation(
                "Workflow does not run for pushes to main."));
        }

        if (!PullRequestMainTrigger().IsMatch(workflow))
        {
            violations.Add(Violation(
                "Workflow does not run for pull requests targeting main."));
        }
    }

    private static void RequireQualityGate(
        string workflow,
        ICollection<ArchitectureViolation> violations)
    {
        if (!workflow.Contains(
                "dotnet-version: '10.0.x'",
                StringComparison.Ordinal) &&
            !workflow.Contains(
                "dotnet-version: \"10.0.x\"",
                StringComparison.Ordinal))
        {
            violations.Add(Violation(
                "Workflow does not select the supported .NET 10 SDK."));
        }

        if (!workflow.Contains(
                "dotnet restore HotJoes.sln",
                StringComparison.Ordinal))
        {
            violations.Add(Violation(
                "HotJoes.sln is not restored explicitly."));
        }

        if (!workflow.Contains(
                "dotnet format HotJoes.sln --verify-no-changes " +
                    "--no-restore",
                StringComparison.Ordinal))
        {
            violations.Add(Violation(
                "Repository formatting is not verified."));
        }

        bool releaseBuild = workflow.Contains(
            "dotnet build HotJoes.sln --configuration Release " +
                "--no-restore",
            StringComparison.Ordinal);
        bool warningsFail = releaseBuild && workflow.Contains(
            "--warnaserror",
            StringComparison.Ordinal);

        if (!releaseBuild)
        {
            violations.Add(Violation(
                "HotJoes.sln is not built in Release configuration."));
        }
        else if (!warningsFail)
        {
            violations.Add(Violation(
                "Release build does not treat warnings as errors."));
        }
    }

    private static void RequireTestGates(
        string workflow,
        ICollection<ArchitectureViolation> violations)
    {
        RequireProjectTest(
            workflow,
            "tests/HotJoes.Domain.Vendor.Tests/" +
                "HotJoes.Domain.Vendor.Tests.csproj",
            "Mandatory Domain unit-test project is not executed.",
            violations);
        RequireProjectTest(
            workflow,
            "tests/HotJoes.Application.Vendor.Tests/" +
                "HotJoes.Application.Vendor.Tests.csproj",
            "Mandatory Application unit-test project is not executed.",
            violations);
        RequireProjectTest(
            workflow,
            "tests/HotJoes.Architecture.Tests/" +
                "HotJoes.Architecture.Tests.csproj",
            "Mandatory architecture-test project is not executed.",
            violations);
        RequireProjectTest(
            workflow,
            "tests/HotJoes.Api.Vendor.Tests/" +
                "HotJoes.Api.Vendor.Tests.csproj",
            "Mandatory API-test project is not executed.",
            violations);
        RequireProjectTest(
            workflow,
            "tests/HotJoes.IntegrationTests/" +
                "HotJoes.IntegrationTests.csproj",
            "Mandatory migration, PostgreSQL and RabbitMQ integration-test " +
                "project is not executed.",
            violations);

        string[] requiredJobNames =
        [
            "quality",
            "unit-tests",
            "architecture-tests",
            "api-tests",
            "integration-tests"
        ];

        foreach (string jobName in requiredJobNames)
        {
            if (!Regex.IsMatch(
                    workflow,
                    $"(?m)^  {Regex.Escape(jobName)}:$",
                    RegexOptions.CultureInvariant))
            {
                violations.Add(Violation(
                    $"Mandatory workflow job '{jobName}' is absent."));
            }
        }

        if (ContinueOnError().IsMatch(workflow))
        {
            violations.Add(Violation(
                "Required workflow gates may not continue after failure."));
        }
    }

    private static void RequireSecretScan(
        string workflow,
        ICollection<ArchitectureViolation> violations)
    {
        string approvedCommand =
            $"docker run --rm -v \"$PWD:/repo\" " +
            $"{ApprovedSecretScannerImage} " +
            "git --redact --no-banner /repo";

        if (!workflow.Contains(
                approvedCommand,
                StringComparison.Ordinal))
        {
            violations.Add(SecurityViolation(
                "Mandatory redacted repository-history secret scan is not " +
                    "executed."));
        }

        if (workflow.Contains(
                "ghcr.io/gitleaks/gitleaks",
                StringComparison.Ordinal) &&
            !workflow.Contains(
                ApprovedSecretScannerImage,
                StringComparison.Ordinal))
        {
            violations.Add(SecurityViolation(
                "Secret scan does not use the approved immutable scanner " +
                    "image."));
        }

        if (!FullHistorySecretScanCheckout().IsMatch(workflow))
        {
            violations.Add(SecurityViolation(
                "Secret scan checkout does not fetch repository history."));
        }
    }

    private static void RequireProjectTest(
        string workflow,
        string projectPath,
        string violationDescription,
        ICollection<ArchitectureViolation> violations)
    {
        if (!workflow.Contains(
                $"dotnet test {projectPath}",
                StringComparison.Ordinal))
        {
            violations.Add(Violation(violationDescription));
        }
    }

    private static void RequireSafeDiagnostics(
        string workflow,
        ICollection<ArchitectureViolation> violations)
    {
        int testCommandCount = Regex.Matches(
            workflow,
            "dotnet test ",
            RegexOptions.CultureInvariant).Count;
        int trxLoggerCount = Regex.Matches(
            workflow,
            "--logger [\\\"']trx;",
            RegexOptions.CultureInvariant).Count;
        int uploadCount = Regex.Matches(
            workflow,
            "uses: actions/upload-artifact@v6",
            RegexOptions.CultureInvariant).Count;

        if (testCommandCount != 5 ||
            trxLoggerCount != testCommandCount ||
            uploadCount != 4 ||
            !workflow.Contains("if: always()", StringComparison.Ordinal) ||
            !workflow.Contains("retention-days:", StringComparison.Ordinal))
        {
            violations.Add(Violation(
                "Test-result diagnostics are not published for every test " +
                "gate."));
        }

        if (UnsafeDiagnosticCommand().IsMatch(workflow))
        {
            violations.Add(Violation(
                "Workflow contains an unsafe diagnostic-publication " +
                "command."));
        }
    }

    private static ArchitectureViolation Violation(string description)
    {
        return new ArchitectureViolation(
            ObligationId,
            WorkflowRelativePath,
            description);
    }

    private static ArchitectureViolation SecurityViolation(
        string description)
    {
        return new ArchitectureViolation(
            SecurityObligationId,
            WorkflowRelativePath,
            description);
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

        throw new InvalidOperationException(
            "Could not locate HotJoes.sln from the architecture-test " +
            "assembly location.");
    }

    [GeneratedRegex(
        "(?ms)^  push:\\n(?:    .+\\n)*?    branches: \\[main\\]$",
        RegexOptions.CultureInvariant)]
    private static partial Regex PushMainTrigger();

    [GeneratedRegex(
        "(?ms)^  pull_request:\\n(?:    .+\\n)*?" +
            "    branches: \\[main\\]$",
        RegexOptions.CultureInvariant)]
    private static partial Regex PullRequestMainTrigger();

    [GeneratedRegex(
        "(?m)^\\s*continue-on-error:\\s*true\\s*$",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    private static partial Regex ContinueOnError();

    [GeneratedRegex(
        "(?m)uses: actions/checkout@v6\\n" +
            "\\s+with:\\n" +
            "\\s+fetch-depth: 0\\n" +
            "(?:\\s*\\n)?" +
            "\\s+- (?:name: [^\\n]+\\n\\s+)?run: docker run " +
            "--rm -v \\\"\\$PWD:/repo\\\" " +
            "ghcr\\.io/gitleaks/gitleaks@sha256:",
        RegexOptions.CultureInvariant)]
    private static partial Regex FullHistorySecretScanCheckout();

    [GeneratedRegex(
        "(?im)^\\s*-?\\s*run:\\s*(?:printenv|env(?:\\s|$)|" +
            "cat\\s+.*(?:appsettings|payload))",
        RegexOptions.CultureInvariant)]
    private static partial Regex UnsafeDiagnosticCommand();
}
