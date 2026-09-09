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

        app.MapReverseProxy();
        app.Run();
    }
}
