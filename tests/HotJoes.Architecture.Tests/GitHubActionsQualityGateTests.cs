namespace HotJoes.Architecture.Tests;

public sealed class GitHubActionsQualityGateTests
{
    private const string CompliantWorkflow = """
        name: Epic 1 Quality Gates

        on:
          push:
            branches: [main]
          pull_request:
            branches: [main]

        jobs:
          quality:
            runs-on: ubuntu-latest
            steps:
              - uses: actions/checkout@v6
              - uses: actions/setup-dotnet@v6
                with:
                  dotnet-version: '10.0.x'
              - run: dotnet restore HotJoes.sln
              - run: dotnet format HotJoes.sln --verify-no-changes --no-restore
              - run: dotnet build HotJoes.sln --configuration Release --no-restore --warnaserror

          unit-tests:
            needs: quality
            runs-on: ubuntu-latest
            steps:
              - uses: actions/checkout@v6
              - uses: actions/setup-dotnet@v6
                with:
                  dotnet-version: '10.0.x'
              - run: dotnet restore HotJoes.sln
              - run: dotnet test tests/HotJoes.Domain.Vendor.Tests/HotJoes.Domain.Vendor.Tests.csproj --configuration Release --no-restore --logger "trx;LogFileName=domain.trx" --results-directory TestResults
              - run: dotnet test tests/HotJoes.Application.Vendor.Tests/HotJoes.Application.Vendor.Tests.csproj --configuration Release --no-restore --logger "trx;LogFileName=application.trx" --results-directory TestResults
              - if: always()
                uses: actions/upload-artifact@v4
                with:
                  name: unit-test-results
                  path: TestResults/*.trx
                  retention-days: 7

          architecture-tests:
            needs: quality
            runs-on: ubuntu-latest
            steps:
              - uses: actions/checkout@v6
              - uses: actions/setup-dotnet@v6
                with:
                  dotnet-version: '10.0.x'
              - run: dotnet restore HotJoes.sln
              - run: dotnet test tests/HotJoes.Architecture.Tests/HotJoes.Architecture.Tests.csproj --configuration Release --no-restore --logger "trx;LogFileName=architecture.trx" --results-directory TestResults
              - if: always()
                uses: actions/upload-artifact@v4
                with:
                  name: architecture-test-results
                  path: TestResults/*.trx
                  retention-days: 7

          api-tests:
            needs: quality
            runs-on: ubuntu-latest
            steps:
              - uses: actions/checkout@v6
              - uses: actions/setup-dotnet@v6
                with:
                  dotnet-version: '10.0.x'
              - run: dotnet restore HotJoes.sln
              - run: dotnet test tests/HotJoes.Api.Vendor.Tests/HotJoes.Api.Vendor.Tests.csproj --configuration Release --no-restore --logger "trx;LogFileName=api.trx" --results-directory TestResults
              - if: always()
                uses: actions/upload-artifact@v4
                with:
                  name: api-test-results
                  path: TestResults/*.trx
                  retention-days: 7

          integration-tests:
            needs: quality
            runs-on: ubuntu-latest
            steps:
              - uses: actions/checkout@v6
              - uses: actions/setup-dotnet@v6
                with:
                  dotnet-version: '10.0.x'
              - run: dotnet restore HotJoes.sln
              - run: dotnet test tests/HotJoes.IntegrationTests/HotJoes.IntegrationTests.csproj --configuration Release --no-restore --logger "trx;LogFileName=integration.trx" --results-directory TestResults
              - if: always()
                uses: actions/upload-artifact@v4
                with:
                  name: integration-test-results
                  path: TestResults/*.trx
                  retention-days: 7
        """;

    [Fact]
    public void AI_CI_001_CurrentWorkflow_ExecutesEveryMandatoryGate()
    {
        IReadOnlyList<ArchitectureViolation> violations =
            GitHubActionsWorkflowPolicy.EvaluateCurrent();

        Assert.Empty(violations);
    }

    [Fact]
    public void AI_CI_001_MissingArchitectureGate_IsDetected()
    {
        string workflow = CompliantWorkflow.Replace(
            "dotnet test tests/HotJoes.Architecture.Tests/" +
                "HotJoes.Architecture.Tests.csproj",
            "dotnet test tests/HotJoes.Domain.Vendor.Tests/" +
                "HotJoes.Domain.Vendor.Tests.csproj",
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Mandatory architecture-test project is not executed.");
    }

    [Fact]
    public void AI_CI_001_MissingIntegrationGate_IsDetected()
    {
        string workflow = CompliantWorkflow.Replace(
            "dotnet test tests/HotJoes.IntegrationTests/" +
                "HotJoes.IntegrationTests.csproj",
            "dotnet test tests/HotJoes.Application.Vendor.Tests/" +
                "HotJoes.Application.Vendor.Tests.csproj",
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Mandatory migration, PostgreSQL and RabbitMQ integration-test " +
                "project is not executed.");
    }

    [Fact]
    public void AI_CI_001_MissingApiGate_IsDetected()
    {
        string workflow = CompliantWorkflow.Replace(
            "dotnet test tests/HotJoes.Api.Vendor.Tests/" +
                "HotJoes.Api.Vendor.Tests.csproj",
            "dotnet test tests/HotJoes.Application.Vendor.Tests/" +
                "HotJoes.Application.Vendor.Tests.csproj",
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Mandatory API-test project is not executed.");
    }

    [Fact]
    public void AI_CI_001_WarningsNotTreatedAsErrors_IsDetected()
    {
        string workflow = CompliantWorkflow.Replace(
            " --warnaserror",
            string.Empty,
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Release build does not treat warnings as errors.");
    }

    [Fact]
    public void AI_CI_001_MainPullRequestTriggerMissing_IsDetected()
    {
        string workflow = CompliantWorkflow.Replace(
            "pull_request:",
            "workflow_dispatch:",
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Workflow does not run for pull requests targeting main.");
    }

    [Fact]
    public void AI_CI_002_FailureTolerantRequiredGate_IsDetected()
    {
        string workflow = CompliantWorkflow.Replace(
            "  architecture-tests:\n" +
                "    needs: quality",
            "  architecture-tests:\n" +
                "    continue-on-error: true\n" +
                "    needs: quality",
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Required workflow gates may not continue after failure.");
    }

    [Fact]
    public void AI_CI_002_MissingSafeTestResultPublication_IsDetected()
    {
        string workflow = CompliantWorkflow.Replace(
            "uses: actions/upload-artifact@v4",
            "uses: actions/cache@v4",
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Test-result diagnostics are not published for every test gate.");
    }

    [Theory]
    [InlineData("      - run: printenv")]
    [InlineData("      - run: env")]
    [InlineData("      - run: cat appsettings.json")]
    [InlineData("      - run: cat TestResults/event-payload.json")]
    public void AI_CI_002_UnsafeDiagnosticPublication_IsDetected(
        string unsafeStep)
    {
        string workflow = CompliantWorkflow.Replace(
            "    steps:\n",
            $"    steps:\n{unsafeStep}\n",
            StringComparison.Ordinal);

        AssertViolation(
            workflow,
            "Workflow contains an unsafe diagnostic-publication command.");
    }

    private static void AssertViolation(
        string workflow,
        string expectedDescription)
    {
        IReadOnlyList<ArchitectureViolation> violations =
            GitHubActionsWorkflowPolicy.Evaluate(workflow);

        ArchitectureViolation violation = Assert.Single(violations);
        Assert.Equal("AI-CI-001/AI-CI-002", violation.ObligationId);
        Assert.Equal(".github/workflows/build.yml", violation.ProjectName);
        Assert.Equal(expectedDescription, violation.Description);
    }
}
