using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotJoes.Api.Vendor;

public sealed class CommunityOpenApiOperationTransformer
    : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        string path = "/" + context.Description.RelativePath?.TrimStart('/');
        string method = context.Description.HttpMethod ?? string.Empty;

        if (path == "/community-participations"
            && method.Equals("POST", StringComparison.OrdinalIgnoreCase)
            && operation.Responses is not null
            && operation.Responses.TryGetValue("201", out IOpenApiResponse? response)
            && response is OpenApiResponse concrete)
        {
            concrete.Headers ??=
                new Dictionary<string, IOpenApiHeader>(StringComparer.OrdinalIgnoreCase);
            concrete.Headers["Location"] = new OpenApiHeader
            {
                Description = "Identifies the recorded Community Participation.",
                Required = true,
                Schema = new OpenApiSchema { Type = JsonSchemaType.String }
            };
        }

        return Task.CompletedTask;
    }
}
