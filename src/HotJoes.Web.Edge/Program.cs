using HotJoes.Web.Edge.Configuration;

namespace HotJoes.Web.Edge;

public sealed partial class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHotJoesEdge(builder.Configuration);

        WebApplication app = builder.Build();

        app.Use(async (context, next) =>
        {
            context.Request.Headers.Remove("Forwarded");
            context.Request.Headers.Remove("X-Forwarded-For");
            context.Request.Headers.Remove("X-Forwarded-Host");
            context.Request.Headers.Remove("X-Forwarded-Proto");
            await next(context);
        });

        app.MapGet("/health/live", () => Results.Ok(new
        {
            component = "web-edge",
            status = "healthy"
        }));

        app.MapGet("/health/ready", async (
            IHttpClientFactory clients,
            CancellationToken cancellationToken) =>
        {
            try
            {
                using HttpResponseMessage response = await clients
                    .CreateClient("vendor-api-readiness")
                    .GetAsync("/health/ready", cancellationToken);
                bool healthy = response.IsSuccessStatusCode;
                return Results.Json(
                    new
                    {
                        component = "web-edge",
                        status = healthy ? "healthy" : "unhealthy"
                    },
                    statusCode: healthy
                        ? StatusCodes.Status200OK
                        : StatusCodes.Status503ServiceUnavailable);
            }
            catch (HttpRequestException)
            {
                return Unhealthy();
            }
            catch (OperationCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return Unhealthy();
            }
        });

        app.MapReverseProxy();
        app.Run();
    }

    private static IResult Unhealthy() => Results.Json(
        new { component = "web-edge", status = "unhealthy" },
        statusCode: StatusCodes.Status503ServiceUnavailable);
}
