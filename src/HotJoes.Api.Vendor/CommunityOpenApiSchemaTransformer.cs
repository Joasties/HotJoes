using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotJoes.Api.Vendor;

public sealed class CommunityOpenApiSchemaTransformer
    : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        Type type = context.JsonPropertyInfo?.PropertyType
            ?? context.JsonTypeInfo.Type;

        if (type == typeof(JoinCommunityRequest))
        {
            AddRequired(schema, "vendorId", "contactPreference");
        }
        else if (type == typeof(JoinCommunityResponse))
        {
            AddRequired(
                schema,
                "communityParticipationId",
                "vendorId",
                "contactPreference",
                "joinedAt");
        }

        if (context.JsonPropertyInfo?.AttributeProvider is PropertyInfo property)
        {
            if (property.DeclaringType is not null
                && (property.DeclaringType == typeof(JoinCommunityRequest)
                    || property.DeclaringType == typeof(JoinCommunityResponse)))
            {
                if (property.Name is "VendorId" or "CommunityParticipationId")
                {
                    schema.Format = "uuid";
                }
                else if (property.Name == "JoinedAt")
                {
                    schema.Format = "date-time";
                }
                else if (property.Name == "ContactPreference")
                {
                    schema.Enum = new List<JsonNode>
                    {
                        JsonValue.Create("email")!,
                        JsonValue.Create("sms")!,
                        JsonValue.Create("whatsApp")!
                    };
                }
            }
        }

        return Task.CompletedTask;
    }

    private static void AddRequired(
        OpenApiSchema schema,
        params string[] members)
    {
        schema.Required ??= new HashSet<string>(StringComparer.Ordinal);
        foreach (string member in members)
        {
            schema.Required.Add(member);
        }
    }
}
