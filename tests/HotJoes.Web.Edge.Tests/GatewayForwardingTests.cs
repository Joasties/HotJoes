using System.Collections.Concurrent;
using System.Net;
using System.Text;
using HotJoes.Web.Edge;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace HotJoes.Web.Edge.Tests;

public sealed class GatewayForwardingTests
{
    [Fact]
    public async Task AI_GW_001_And_002_RequestAndResponseRemainTransparent()
    {
        await using DownstreamServer downstream = await DownstreamServer.Start();
        await using EdgeFactory edge = new(downstream.Address);
        using HttpClient client = edge.CreateClient();
        using HttpRequestMessage request = new(HttpMethod.Post, "/vendors")
        {
            Content = new StringContent(
                "{\"registration\":\"unchanged\"}",
                Encoding.UTF8,
                "application/json")
        };
        request.Headers.Add("traceparent", "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01");
        request.Headers.Add("X-Test-Semantic-Header", "preserve-me");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("vendor-api", response.Headers.GetValues("X-Outcome-Owner").Single());
        Assert.Equal("{\"outcome\":\"controlled\"}", await response.Content.ReadAsStringAsync());
        RecordedRequest forwarded = Assert.Single(downstream.Requests);
        Assert.Equal("POST", forwarded.Method);
        Assert.Equal("/vendors", forwarded.Path);
        Assert.Equal("{\"registration\":\"unchanged\"}", forwarded.Body);
        Assert.Equal("preserve-me", forwarded.Headers["X-Test-Semantic-Header"]);
        Assert.Equal(
            request.Headers.GetValues("traceparent").Single().Split('-')[1],
            forwarded.Headers["traceparent"].Split('-')[1]);
    }

    [Theory]
    [InlineData("GET", "/vendors", HttpStatusCode.MethodNotAllowed)]
    [InlineData("POST", "/vendors/not-allowed", HttpStatusCode.MethodNotAllowed)]
    [InlineData("GET", "/addresses", HttpStatusCode.NotFound)]
    [InlineData("POST", "/address-resolution", HttpStatusCode.NotFound)]
    public async Task AI_GW_001_NonAllowlistedRequestsNeverReachVendorApi(
        string method,
        string path,
        HttpStatusCode expectedStatus)
    {
        await using DownstreamServer downstream = await DownstreamServer.Start();
        await using EdgeFactory edge = new(downstream.Address);
        using HttpClient client = edge.CreateClient();

        using HttpResponseMessage response = await client.SendAsync(
            new HttpRequestMessage(new HttpMethod(method), path));

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Empty(downstream.Requests);
    }

    [Fact]
    public async Task AI_GW_003_DownstreamFailureResponseIsNotRetriedOrRemapped()
    {
        await using DownstreamServer downstream = await DownstreamServer.Start(
            HttpStatusCode.ServiceUnavailable);
        await using EdgeFactory edge = new(downstream.Address);
        using HttpClient client = edge.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Single(downstream.Requests);
    }

    [Fact]
    public async Task AI_GW_002_PreResponseFailureProducesSafeGatewayFailure()
    {
        await using EdgeFactory edge = new("http://127.0.0.1:1");
        using HttpClient client = edge.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            new StringContent(
                "{\"contactName\":\"must-not-echo\"}",
                Encoding.UTF8,
                "application/json"));

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.DoesNotContain(
            "must-not-echo",
            await response.Content.ReadAsStringAsync(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task AI_GW_004_UntrustedForwardingHeadersAreNotForwarded()
    {
        await using DownstreamServer downstream = await DownstreamServer.Start();
        await using EdgeFactory edge = new(downstream.Address);
        using HttpClient client = edge.CreateClient();
        using HttpRequestMessage request = new(HttpMethod.Get, "/vendors/vendor-123");
        request.Headers.TryAddWithoutValidation("Forwarded", "for=203.0.113.7;proto=https");
        request.Headers.TryAddWithoutValidation("X-Forwarded-For", "203.0.113.7");
        request.Headers.TryAddWithoutValidation("X-Forwarded-Host", "attacker.example");
        request.Headers.TryAddWithoutValidation("X-Forwarded-Proto", "https");

        await client.SendAsync(request);

        RecordedRequest forwarded = Assert.Single(downstream.Requests);
        Assert.DoesNotContain("Forwarded", forwarded.Headers.Keys, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("X-Forwarded-For", forwarded.Headers.Keys, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("X-Forwarded-Host", forwarded.Headers.Keys, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("X-Forwarded-Proto", forwarded.Headers.Keys, StringComparer.OrdinalIgnoreCase);
    }

    private sealed class EdgeFactory(string downstreamAddress)
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
            builder.UseSetting(
                "Edge:VendorApiBaseAddress",
                downstreamAddress);
        }
    }

    private sealed class DownstreamServer : IAsyncDisposable
    {
        private readonly WebApplication application;

        private DownstreamServer(
            WebApplication application,
            string address,
            ConcurrentQueue<RecordedRequest> requests)
        {
            this.application = application;
            Address = address;
            Requests = requests;
        }

        public string Address { get; }

        public ConcurrentQueue<RecordedRequest> Requests { get; }

        public static async Task<DownstreamServer> Start(
            HttpStatusCode status = HttpStatusCode.UnprocessableEntity)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("http://127.0.0.1:0");
            WebApplication application = builder.Build();
            ConcurrentQueue<RecordedRequest> requests = new();
            application.Run(async context =>
            {
                string body;
                using (StreamReader reader = new(context.Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                requests.Enqueue(new RecordedRequest(
                    context.Request.Method,
                    context.Request.Path,
                    body,
                    context.Request.Headers.ToDictionary(
                        header => header.Key,
                        header => header.Value.ToString(),
                        StringComparer.OrdinalIgnoreCase)));
                context.Response.StatusCode = (int)status;
                context.Response.Headers["X-Outcome-Owner"] = "vendor-api";
                await context.Response.WriteAsync("{\"outcome\":\"controlled\"}");
            });
            await application.StartAsync();
            string address = application.Services
                .GetRequiredService<IServer>()
                .Features.Get<IServerAddressesFeature>()!
                .Addresses.Single();
            return new DownstreamServer(application, address, requests);
        }

        public async ValueTask DisposeAsync()
        {
            await application.StopAsync();
            await application.DisposeAsync();
        }
    }

    private sealed record RecordedRequest(
        string Method,
        string Path,
        string Body,
        IReadOnlyDictionary<string, string> Headers);
}
