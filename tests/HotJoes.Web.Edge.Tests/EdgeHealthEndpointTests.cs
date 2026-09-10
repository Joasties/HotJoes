using System.Net;
using HotJoes.Web.Edge;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace HotJoes.Web.Edge.Tests;

public sealed class EdgeHealthEndpointTests
{
    [Fact]
    public async Task AI_HEALTH_001_LivenessDoesNotRequireVendorApi()
    {
        await using EdgeFactory edge = new("http://127.0.0.1:1");
        using HttpClient client = edge.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.ServiceUnavailable)]
    public async Task AI_HEALTH_001_ReadinessReflectsVendorApiReadiness(
        HttpStatusCode vendorApiStatus,
        HttpStatusCode expectedEdgeStatus)
    {
        await using DownstreamHealthServer downstream =
            await DownstreamHealthServer.Start(vendorApiStatus);
        await using EdgeFactory edge = new(downstream.Address);
        using HttpClient client = edge.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/health/ready");

        Assert.Equal(expectedEdgeStatus, response.StatusCode);
        Assert.Equal(1, downstream.ReadinessRequestCount);
    }

    [Fact]
    public async Task AI_HEALTH_001_UnreachableVendorApiMakesEdgeUnready()
    {
        await using EdgeFactory edge = new("http://127.0.0.1:1");
        using HttpClient client = edge.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    private sealed class EdgeFactory(string vendorApiAddress)
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
            builder.UseSetting(
                "Edge:VendorApiBaseAddress",
                vendorApiAddress);
        }
    }

    private sealed class DownstreamHealthServer : IAsyncDisposable
    {
        private readonly WebApplication application;
        private int readinessRequestCount;

        private DownstreamHealthServer(
            WebApplication application,
            string address)
        {
            this.application = application;
            Address = address;
        }

        public string Address { get; }

        public int ReadinessRequestCount => readinessRequestCount;

        public static async Task<DownstreamHealthServer> Start(
            HttpStatusCode status)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("http://127.0.0.1:0");
            WebApplication application = builder.Build();
            DownstreamHealthServer? server = null;
            application.MapGet("/health/ready", () =>
            {
                Interlocked.Increment(ref server!.readinessRequestCount);
                return Results.StatusCode((int)status);
            });
            await application.StartAsync();
            string address = application.Services
                .GetRequiredService<IServer>()
                .Features.Get<IServerAddressesFeature>()!
                .Addresses.Single();
            server = new DownstreamHealthServer(application, address);
            return server;
        }

        public async ValueTask DisposeAsync()
        {
            await application.StopAsync();
            await application.DisposeAsync();
        }
    }
}
