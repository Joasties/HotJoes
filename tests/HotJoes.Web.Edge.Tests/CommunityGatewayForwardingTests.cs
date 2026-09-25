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

public sealed class CommunityGatewayForwardingTests
{
    [Fact]
    public async Task AI_GW_006_CommunityRequestAndResponseRemainTransparentAndUnretried()
    {
        await using DownstreamServer downstream = await DownstreamServer.Start();
        await using EdgeFactory edge = new(downstream.Address);
        using HttpClient client = edge.CreateClient();
        const string body =
            "{\"vendorId\":\"187c1149-b8be-4b44-9472-812545833661\",\"contactPreference\":\"sms\"}";

        using HttpResponseMessage response = await client.PostAsync(
            "/community-participations",
            new StringContent(body, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(
            "/community-participations/b9c32512-6459-4210-a1cf-b6f999bf7018",
            response.Headers.Location?.OriginalString);
        Assert.Equal("{\"result\":\"unchanged\"}",
            await response.Content.ReadAsStringAsync());
        RecordedRequest forwarded = Assert.Single(downstream.Requests);
        Assert.Equal("POST", forwarded.Method);
        Assert.Equal("/community-participations", forwarded.Path);
        Assert.Equal(body, forwarded.Body);
    }

    [Theory]
    [InlineData("GET", "/community-participations", HttpStatusCode.MethodNotAllowed)]
    [InlineData("GET", "/community-participations/anything", HttpStatusCode.NotFound)]
    [InlineData("POST", "/community-participations/anything", HttpStatusCode.NotFound)]
    public async Task AI_GW_006_NonAllowlistedCommunityRequestDoesNotReachApi(
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

    private sealed class EdgeFactory(string downstreamAddress)
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
            builder.UseSetting("Edge:VendorApiBaseAddress", downstreamAddress);
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

        public static async Task<DownstreamServer> Start()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("http://127.0.0.1:0");
            WebApplication application = builder.Build();
            ConcurrentQueue<RecordedRequest> requests = new();
            application.Run(async context =>
            {
                using StreamReader reader = new(context.Request.Body);
                string body = await reader.ReadToEndAsync();
                requests.Enqueue(new RecordedRequest(
                    context.Request.Method,
                    context.Request.Path + context.Request.QueryString,
                    body));
                context.Response.StatusCode = StatusCodes.Status201Created;
                context.Response.Headers.Location =
                    "/community-participations/b9c32512-6459-4210-a1cf-b6f999bf7018";
                await context.Response.WriteAsync("{\"result\":\"unchanged\"}");
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
        string Body);
}
