using HotJoes.Infrastructure.Health;
using HotJoes.Worker.CommunityConsumer;

CommunityConsumerHostOptions options = CommunityConsumerHostOptions.LoadFromEnvironment();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(options);
builder.Services.AddSingleton(new OperationalHealthEvaluator([
    new RabbitMqHealthDependencyProbe(options.RabbitMq),
    new PostgreSqlHealthDependencyProbe(HealthDependency.CommunityPostgreSql, options.CommunityDatabase)]));
builder.Services.AddHostedService<CommunityConsumerBackgroundService>();
WebApplication app = builder.Build();
app.MapGet("/health/live", (OperationalHealthEvaluator evaluator) => Results.Json(evaluator.EvaluateLiveness(OperationalComponent.CommunityConsumer)));
app.MapGet("/health/ready", async (OperationalHealthEvaluator evaluator, CancellationToken token) =>
{
    OperationalHealthEvidence evidence = await evaluator.EvaluateReadinessAsync(OperationalComponent.CommunityConsumer, token);
    return Results.Json(evidence, statusCode: evidence.Status == OperationalHealthStatus.Unhealthy ? 503 : 200);
});
await app.RunAsync();
public partial class Program;
