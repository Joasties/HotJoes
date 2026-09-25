using System.Text.Json;
using System.Text.Json.Serialization;
using HotJoes.Api.Vendor;
using HotJoes.Api.Vendor.Configuration;
using HotJoes.Infrastructure.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer<VendorOpenApiSchemaTransformer>();
    options.AddSchemaTransformer<CommunityOpenApiSchemaTransformer>();
    options.AddOperationTransformer<VendorOpenApiOperationTransformer>();
    options.AddOperationTransformer<CommunityOpenApiOperationTransformer>();
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
builder.Services.AddSingleton<DetermineRequiredLicenceTypesRequestStructureValidator>();
builder.Services.AddSingleton<DetermineRequiredLicenceTypesRequestReader>();
builder.Services.AddSingleton<DetermineRequiredLicenceTypesRequestMapper>();
builder.Services.AddSingleton<DetermineRequiredLicenceTypesResponseMapper>();
builder.Services.AddSingleton<RegisteredVendorDetailsResponseMapper>();
builder.Services.AddSingleton<JoinCommunityRequestStructureValidator>();
builder.Services.AddSingleton<JoinCommunityRequestReader>();
builder.Services.AddSingleton<JoinCommunityRequestMapper>();
builder.Services.AddSingleton<JoinCommunityResponseMapper>();
builder.Services.AddSingleton<CommunityApiErrorMapper>();
builder.Services.AddSingleton<VendorApiErrorMapper>();

builder.Services.AddVendorApiComposition(
    builder.Configuration,
    builder.Environment.ContentRootPath);
string vendorDatabase = builder.Configuration.GetConnectionString(
    "VendorDatabase") ?? throw new InvalidOperationException(
        "ConnectionStrings:VendorDatabase is required.");
builder.Services.AddSingleton(new OperationalHealthEvaluator(
    [new PostgreSqlHealthDependencyProbe(
        HealthDependency.VendorPostgreSql,
        vendorDatabase)]));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapAddressSearchEndpoint();
app.MapVendorEndpoints();
app.MapCommunityEndpoints();
app.MapGet("/health/live", (OperationalHealthEvaluator evaluator) =>
    Results.Json(evaluator.EvaluateLiveness(OperationalComponent.VendorApi)))
    .ExcludeFromDescription();
app.MapGet("/health/ready", async (
    OperationalHealthEvaluator evaluator,
    CancellationToken cancellationToken) =>
{
    OperationalHealthEvidence evidence = await evaluator.EvaluateReadinessAsync(
        OperationalComponent.VendorApi,
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
