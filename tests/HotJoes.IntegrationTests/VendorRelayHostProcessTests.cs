using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace HotJoes.IntegrationTests;

public sealed class VendorRelayHostProcessTests
{
    private static readonly TimeSpan StartupTimeout = TimeSpan.FromSeconds(15);

    [Fact]
    public async Task AI_RUNTIME_001_AI_HEALTH_001_Host_ExposesResponsibilityHealth()
    {
        int port = ReservePort();
        using Process process = StartHost(port, includeConfiguration: true);

        try
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{port}")
            };

            HttpResponseMessage liveness = await WaitForLivenessAsync(
                client,
                process);
            HttpResponseMessage readiness = await client.GetAsync(
                "/health/ready");

            Assert.Equal(HttpStatusCode.OK, liveness.StatusCode);
            Assert.Equal(
                HttpStatusCode.ServiceUnavailable,
                readiness.StatusCode);

            string readinessBody = await readiness.Content.ReadAsStringAsync();
            Assert.DoesNotContain(
                "Host=127.0.0.1",
                readinessBody,
                StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(
                "amqp://",
                readinessBody,
                StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Stop(process);
        }
    }

    [Fact]
    public async Task AI_RUNTIME_002_Host_MissingConfiguration_FailsClosed()
    {
        int port = ReservePort();
        using Process process = StartHost(port, includeConfiguration: false);

        using var timeout = new CancellationTokenSource(
            TimeSpan.FromSeconds(15));
        await process.WaitForExitAsync(timeout.Token);

        Assert.NotEqual(0, process.ExitCode);
    }

    private static Process StartHost(
        int port,
        bool includeConfiguration)
    {
        string repositoryRoot = FindRepositoryRoot();
        string projectPath = Path.Combine(
            repositoryRoot,
            "src",
            "HotJoes.Worker.VendorRelay",
            "HotJoes.Worker.VendorRelay.csproj");

        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = repositoryRoot
        };
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--no-build");
        startInfo.ArgumentList.Add("--no-launch-profile");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.Environment["ASPNETCORE_URLS"] =
            $"http://127.0.0.1:{port}";
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

        if (includeConfiguration)
        {
            AddConfiguration(startInfo);
        }

        Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "The Vendor relay host could not be started.");
        _ = DrainAsync(process.StandardOutput);
        _ = DrainAsync(process.StandardError);
        return process;
    }

    private static void AddConfiguration(ProcessStartInfo startInfo)
    {
        const string prefix = "HotJoes__VendorRelay__";
        startInfo.Environment[$"{prefix}VendorDatabase"] =
            "Host=127.0.0.1;Port=1;Database=hotjoes;" +
            "Username=test;Password=test;Timeout=1";
        startInfo.Environment[$"{prefix}RabbitMq"] =
            "amqp://guest:guest@127.0.0.1:1";
        startInfo.Environment[$"{prefix}ExchangeName"] = "hotjoes.vendor";
        startInfo.Environment[$"{prefix}ExchangeType"] = "direct";
        startInfo.Environment[$"{prefix}QueueName"] = "hotjoes.compliance";
        startInfo.Environment[$"{prefix}RoutingKey"] = "vendor.registered";
        startInfo.Environment[$"{prefix}PollIntervalMilliseconds"] = "100";
        startInfo.Environment[$"{prefix}LeaseDurationSeconds"] = "30";
        startInfo.Environment[$"{prefix}BatchSize"] = "10";
        startInfo.Environment[$"{prefix}RetryInitialDelaySeconds"] = "1";
        startInfo.Environment[$"{prefix}RetryMaximumDelaySeconds"] = "10";
        startInfo.Environment[$"{prefix}AutomaticAttemptLimit"] = "3";
    }

    private static async Task<HttpResponseMessage> WaitForLivenessAsync(
        HttpClient client,
        Process process)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow.Add(StartupTimeout);

        while (DateTimeOffset.UtcNow < deadline)
        {
            if (process.HasExited)
            {
                throw new Xunit.Sdk.XunitException(
                    $"Vendor relay host exited with code {process.ExitCode}.");
            }

            try
            {
                return await client.GetAsync("/health/live");
            }
            catch (HttpRequestException)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
        }

        throw new TimeoutException(
            "Vendor relay liveness did not become available within the " +
            "bounded startup interval.");
    }

    private static int ReservePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static void Stop(Process process)
    {
        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
        }
    }

    private static async Task DrainAsync(StreamReader reader)
    {
        _ = await reader.ReadToEndAsync();
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
