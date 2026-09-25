using HotJoes.Infrastructure.Health;
using HotJoes.Worker.CommunityRelay;

CommunityRelayHostOptions options = CommunityRelayHostOptions.LoadFromEnvironment();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(options);
builder.Services.AddSingleton(new OperationalHealthEvaluator([
    new PostgreSqlHealthDependencyProbe(HealthDependency.CommunityPostgreSql, options.CommunityDatabase),
    new RabbitMqHealthDependencyProbe(options.RabbitMq)]));
builder.Services.AddHostedService<CommunityRelayBackgroundService>();
WebApplication app = builder.Build();
app.MapGet("/health/live", (OperationalHealthEvaluator evaluator) =>
    Results.Json(evaluator.EvaluateLiveness(OperationalComponent.CommunityRelay)));
app.MapGet("/health/ready", async (OperationalHealthEvaluator evaluator, CancellationToken token) =>
{
    OperationalHealthEvidence evidence = await evaluator.EvaluateReadinessAsync(OperationalComponent.CommunityRelay, token);
    return Results.Json(evidence, statusCode: evidence.Status == OperationalHealthStatus.Unhealthy ? 503 : 200);
});
await app.RunAsync();
public partial class Program;
