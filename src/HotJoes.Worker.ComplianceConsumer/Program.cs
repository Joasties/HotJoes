using HotJoes.Infrastructure.Health;
using HotJoes.Worker.ComplianceConsumer;

ComplianceConsumerHostOptions options =
    ComplianceConsumerHostOptions.LoadFromEnvironment();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(options);
builder.Services.AddSingleton(
    new OperationalHealthEvaluator(
        [
            new RabbitMqHealthDependencyProbe(options.RabbitMq),
            new PostgreSqlHealthDependencyProbe(
                HealthDependency.CompliancePostgreSql,
                options.ComplianceDatabase)
        ]));
builder.Services.AddHostedService<ComplianceConsumerBackgroundService>();

WebApplication app = builder.Build();

app.MapGet(
    "/health/live",
    (OperationalHealthEvaluator evaluator) => Results.Json(
        evaluator.EvaluateLiveness(OperationalComponent.ComplianceConsumer)));

app.MapGet(
    "/health/ready",
    async (
        OperationalHealthEvaluator evaluator,
        CancellationToken cancellationToken) =>
    {
        OperationalHealthEvidence evidence =
            await evaluator.EvaluateReadinessAsync(
                OperationalComponent.ComplianceConsumer,
                cancellationToken);
        int statusCode = evidence.Status == OperationalHealthStatus.Unhealthy
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status200OK;

        return Results.Json(evidence, statusCode: statusCode);
    });

await app.RunAsync();

public partial class Program;
