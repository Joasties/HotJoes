using System.Text.Json;
using System.Text.Json.Serialization;
using HotJoes.Api.Vendor;
using HotJoes.Application.Vendor;
using HotJoes.Api.Vendor.Configuration;
using HotJoes.Infrastructure.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer<VendorOpenApiSchemaTransformer>();
    options.AddOperationTransformer<VendorOpenApiOperationTransformer>();
});
builder.Services.AddExceptionHandler<VendorApiExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

builder.Services.AddSingleton<RegisterVendorRequestStructureValidator>();
builder.Services.AddSingleton<RegisterVendorRequestReader>();
builder.Services.AddSingleton<RegisterVendorRequestMapper>();
builder.Services.AddSingleton<RegisterVendorResponseMapper>();
builder.Services.AddSingleton<RegisteredVendorDetailsResponseMapper>();
builder.Services.AddSingleton<VendorApiErrorMapper>();

builder.Services.AddVendorApiComposition(
    builder.Configuration,
    builder.Environment.ContentRootPath);
string vendorDatabase = builder.Configuration.GetConnectionString(
    "VendorDatabase") ?? throw new InvalidOperationException(
        "ConnectionStrings:VendorDatabase is required.");
builder.Services.AddSingleton(new Epic1HealthEvaluator(
    [new PostgreSqlHealthDependencyProbe(
        HealthDependency.VendorPostgreSql,
        vendorDatabase)]));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapVendorEndpoints();
app.MapGet("/health/live", (Epic1HealthEvaluator evaluator) =>
    Results.Json(evaluator.EvaluateLiveness(Epic1Component.VendorApi)))
    .ExcludeFromDescription();
app.MapGet("/health/ready", async (
    Epic1HealthEvaluator evaluator,
    CancellationToken cancellationToken) =>
{
    OperationalHealthEvidence evidence = await evaluator.EvaluateReadinessAsync(
        Epic1Component.VendorApi,
        cancellationToken);
    return Results.Json(
        evidence,
        statusCode: evidence.Status == OperationalHealthStatus.Unhealthy
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status200OK);
})
    .ExcludeFromDescription();

app.Run();

public partial class Program;
