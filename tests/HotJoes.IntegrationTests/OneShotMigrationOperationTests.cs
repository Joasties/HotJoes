using System.Diagnostics;
using HotJoes.Infrastructure.ComplianceConsumer;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HotJoes.IntegrationTests;

[Collection(MigrationPostgreSqlCollection.Name)]
public sealed class OneShotMigrationOperationTests
{
    private const string VendorConnectionStringKey =
        "ConnectionStrings__VendorDatabase";
    private const string ComplianceConnectionStringKey =
        "ConnectionStrings__ComplianceDatabase";
    private static readonly TimeSpan ExecutionTimeout =
        TimeSpan.FromSeconds(90);

    private readonly MigrationPostgreSqlFixture _fixture;

    public OneShotMigrationOperationTests(
        MigrationPostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AI_RUNTIME_005_Operation_MigratesBothSchemas_AndIsIdempotent()
    {
        await ResetPublicSchemaAsync();

        int firstExitCode = await RunMigrationOperationAsync(
            _fixture.ConnectionString,
            _fixture.ConnectionString);
        int secondExitCode = await RunMigrationOperationAsync(
            _fixture.ConnectionString,
            _fixture.ConnectionString);

        AssertSuccess(firstExitCode);
        AssertSuccess(secondExitCode);

        await using VendorRegistrationDbContext vendorContext =
            CreateVendorContext();
        await using ComplianceReceiptDbContext complianceContext =
            CreateComplianceContext();

        Assert.Empty(
            await vendorContext.Database.GetPendingMigrationsAsync());
        Assert.Empty(
            await complianceContext.Database.GetPendingMigrationsAsync());
    }

    [Fact]
    public async Task AI_RUNTIME_005_Operation_ReturnsFailure_WhenMigrationFails()
    {
        await ResetPublicSchemaAsync();
        string unavailableDatabase = new NpgsqlConnectionStringBuilder(
            _fixture.ConnectionString)
        {
            Host = "127.0.0.1",
            Port = 1,
            Timeout = 1
        }.ConnectionString;

        int exitCode = await RunMigrationOperationAsync(
            _fixture.ConnectionString,
            unavailableDatabase);

        Assert.NotEqual(0, exitCode);
    }

    private static void AssertSuccess(int exitCode)
    {
        Assert.True(
            exitCode == 0,
            $"Migration operation exited with code {exitCode}.");
    }

    private async Task ResetPublicSchemaAsync()
    {
        await using var connection = new NpgsqlConnection(
            _fixture.ConnectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText =
            "DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public;";
        await command.ExecuteNonQueryAsync();
    }

    private VendorRegistrationDbContext CreateVendorContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private ComplianceReceiptDbContext CreateComplianceContext()
    {
        DbContextOptions<ComplianceReceiptDbContext> options =
            new DbContextOptionsBuilder<ComplianceReceiptDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new ComplianceReceiptDbContext(options);
    }

    private static async Task<int> RunMigrationOperationAsync(
        string vendorConnectionString,
        string complianceConnectionString)
    {
        string repositoryRoot = FindRepositoryRoot();
        string projectPath = Path.Combine(
            repositoryRoot,
            "src",
            "HotJoes.Database.Migrations",
            "HotJoes.Database.Migrations.csproj");

        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = repositoryRoot
        };
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--no-launch-profile");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.Environment[VendorConnectionStringKey] =
            vendorConnectionString;
        startInfo.Environment[ComplianceConnectionStringKey] =
            complianceConnectionString;

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "The migration operation could not be started.");

        Task<string> standardOutput = process.StandardOutput.ReadToEndAsync();
        Task<string> standardError = process.StandardError.ReadToEndAsync();

        using var timeout = new CancellationTokenSource(ExecutionTimeout);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException(
                "The one-shot migration operation exceeded its bounded " +
                $"{ExecutionTimeout.TotalSeconds}-second execution limit.");
        }

        _ = await standardOutput;
        _ = await standardError;
        return process.ExitCode;
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

        throw new DirectoryNotFoundException(
            "Could not locate the HotJoes solution root.");
    }
}
