using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotJoes.Api.Vendor;

public sealed class VendorOpenApiOperationTransformer
    : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        string path = "/" + context.Description.RelativePath?.TrimStart('/');
        string method = context.Description.HttpMethod ?? string.Empty;

        if (path == "/vendors"
            && method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            AddLocationHeader(operation);
        }
        else if (path == "/vendors/{vendorId}"
                 && method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            SetVendorIdRouteFormat(operation);
        }

        return Task.CompletedTask;
    }

    private static void AddLocationHeader(OpenApiOperation operation)
    {
        if (operation.Responses is null
            || !operation.Responses.TryGetValue("201", out IOpenApiResponse? response)
            || response is not OpenApiResponse concreteResponse)
        {
            return;
        }

        concreteResponse.Headers ??=
            new Dictionary<string, IOpenApiHeader>(StringComparer.OrdinalIgnoreCase);
        concreteResponse.Headers["Location"] = new OpenApiHeader
        {
            Description = "Identifies the registered Vendor retrieval resource.",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            }
        };
    }

    private static void SetVendorIdRouteFormat(OpenApiOperation operation)
    {
        IOpenApiParameter? parameter = operation.Parameters?
            .SingleOrDefault(candidate =>
                candidate.Name == "vendorId"
                && candidate.In == ParameterLocation.Path);

        if (parameter?.Schema is OpenApiSchema schema)
        {
            schema.Format = "uuid";
        }
    }
}
