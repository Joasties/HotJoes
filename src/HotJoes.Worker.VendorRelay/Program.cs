using HotJoes.Infrastructure.Health;
using HotJoes.Worker.VendorRelay;

VendorRelayHostOptions options =
    VendorRelayHostOptions.LoadFromEnvironment();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(options);
builder.Services.AddSingleton(
    new OperationalHealthEvaluator(
        [
            new PostgreSqlHealthDependencyProbe(
                HealthDependency.VendorPostgreSql,
                options.VendorDatabase),
            new RabbitMqHealthDependencyProbe(options.RabbitMq)
        ]));
builder.Services.AddHostedService<VendorRelayBackgroundService>();

WebApplication app = builder.Build();

app.MapGet(
    "/health/live",
    (OperationalHealthEvaluator evaluator) => Results.Json(
        evaluator.EvaluateLiveness(OperationalComponent.VendorRelay)));

app.MapGet(
    "/health/ready",
    async (
        OperationalHealthEvaluator evaluator,
        CancellationToken cancellationToken) =>
    {
        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(
                OperationalComponent.VendorRelay,
                cancellationToken);
        int statusCode = evidence.Status == OperationalHealthStatus.Unhealthy
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status200OK;

        return Results.Json(evidence, statusCode: statusCode);
    });

await app.RunAsync();

public partial class Program;
