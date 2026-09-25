using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace HotJoes.IntegrationTests;

public sealed class CommunityConsumerHostProcessTests
{
    [Fact]
    public async Task AI_COMMUNITY_003_HostExposesSafeResponsibilityHealth()
    {
        int port = ReservePort();
        using Process process = Start(port, configured: true);
        try
        {
            using var client = new HttpClient
            { BaseAddress = new Uri($"http://127.0.0.1:{port}") };
            HttpResponseMessage live = await WaitForLive(client, process);
            HttpResponseMessage ready = await client.GetAsync("/health/ready");
            Assert.Equal(HttpStatusCode.OK, live.StatusCode);
            Assert.Equal(HttpStatusCode.ServiceUnavailable, ready.StatusCode);
            string body = await ready.Content.ReadAsStringAsync();
            Assert.DoesNotContain("Password=test", body,
                StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("amqp://", body,
                StringComparison.OrdinalIgnoreCase);
        }
        finally { Stop(process); }
    }

    [Fact]
    public async Task AI_COMMUNITY_003_MissingConfigurationFailsClosed()
    {
        using Process process = Start(ReservePort(), configured: false);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        await process.WaitForExitAsync(timeout.Token);
        Assert.NotEqual(0, process.ExitCode);
    }

    private static Process Start(int port, bool configured)
    {
        string root = Root();
        var info = new ProcessStartInfo("dotnet")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = root
        };
        foreach (string argument in new[] { "run", "--no-build",
            "--no-launch-profile", "--configuration", Configuration(),
            "--project", Path.Combine(root, "src",
                "HotJoes.Worker.CommunityConsumer",
                "HotJoes.Worker.CommunityConsumer.csproj") })
            info.ArgumentList.Add(argument);
        info.Environment["ASPNETCORE_URLS"] = $"http://127.0.0.1:{port}";
        info.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        if (configured) AddConfiguration(info);
        Process process = Process.Start(info) ?? throw new InvalidOperationException(
            "The Community consumer host could not be started.");
        _ = process.StandardOutput.ReadToEndAsync();
        _ = process.StandardError.ReadToEndAsync();
        return process;
    }

    private static void AddConfiguration(ProcessStartInfo info)
    {
        const string p = "HotJoes__CommunityConsumer__";
        info.Environment[$"{p}CommunityDatabase"] = "Host=127.0.0.1;Port=1;Database=hotjoes;Username=test;Password=test;Timeout=1";
        info.Environment[$"{p}RabbitMq"] = "amqp://guest:guest@127.0.0.1:1";
        info.Environment[$"{p}PrimaryExchangeName"] = "hotjoes.community.participation";
        info.Environment[$"{p}PrimaryExchangeType"] = "direct";
        info.Environment[$"{p}PrimaryQueueName"] = "hotjoes.community.consumer";
        info.Environment[$"{p}PrimaryRoutingKey"] = "community.participation.recorded";
        info.Environment[$"{p}RetryExchangeName"] = "hotjoes.community.retry";
        info.Environment[$"{p}RetryQueueName"] = "hotjoes.community.retry";
        info.Environment[$"{p}RetryRoutingKey"] = "community.participation.recorded.retry";
        info.Environment[$"{p}DeadLetterExchangeName"] = "hotjoes.community.deadletter";
        info.Environment[$"{p}DeadLetterQueueName"] = "hotjoes.community.deadletter";
        info.Environment[$"{p}DeadLetterRoutingKey"] = "community.participation.recorded.deadletter";
        info.Environment[$"{p}PollIntervalMilliseconds"] = "100";
        info.Environment[$"{p}RetryDelaySeconds"] = "1";
        info.Environment[$"{p}MaximumAutomaticAttempts"] = "3";
    }

    private static async Task<HttpResponseMessage> WaitForLive(HttpClient client,
        Process process)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow.AddSeconds(15);
        while (DateTimeOffset.UtcNow < deadline)
        {
            if (process.HasExited) throw new Xunit.Sdk.XunitException(
                $"Community consumer exited with code {process.ExitCode}.");
            try { return await client.GetAsync("/health/live"); }
            catch (HttpRequestException) { await Task.Delay(100); }
        }
        throw new TimeoutException("Community consumer did not become live.");
    }

    private static int ReservePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port; listener.Stop(); return port;
    }
    private static void Stop(Process process)
    { if (!process.HasExited) { process.Kill(true); process.WaitForExit(); } }
    private static string Configuration() => new DirectoryInfo(AppContext.BaseDirectory).Parent?.Name is string value && value is "Debug" or "Release" ? value : throw new InvalidOperationException("Cannot determine configuration.");
    private static string Root()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        { if (File.Exists(Path.Combine(directory.FullName, "HotJoes.sln"))) return directory.FullName; directory = directory.Parent; }
        throw new DirectoryNotFoundException("Could not find solution root.");
    }
}
