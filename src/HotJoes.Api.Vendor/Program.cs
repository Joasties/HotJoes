using System.Text.Json;
using System.Text.Json.Serialization;
using HotJoes.Api.Vendor;
using HotJoes.Application.Vendor;
using HotJoes.Api.Vendor.Configuration;

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

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapVendorEndpoints();

app.Run();

public partial class Program;
