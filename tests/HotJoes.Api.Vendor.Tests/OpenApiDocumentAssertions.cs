using System.Text.Json;

namespace HotJoes.Api.Vendor.Tests;

public static class OpenApiDocumentAssertions
{
    public static JsonElement Operation(
        JsonElement document,
        string path,
        string method)
    {
        return document.GetProperty("paths")
            .GetProperty(path)
            .GetProperty(method);
    }

    public static JsonElement RequestSchema(
        JsonElement document,
        JsonElement operation)
    {
        JsonElement body = operation.GetProperty("requestBody");
        Assert.True(body.GetProperty("required").GetBoolean());
        return Resolve(
            document,
            body.GetProperty("content")
                .GetProperty("application/json")
                .GetProperty("schema"));
    }

    public static JsonElement ResponseSchema(
        JsonElement document,
        JsonElement operation,
        string status)
    {
        return Resolve(
            document,
            operation.GetProperty("responses")
                .GetProperty(status)
                .GetProperty("content")
                .GetProperty("application/json")
                .GetProperty("schema"));
    }

    public static JsonElement PropertySchema(
        JsonElement document,
        JsonElement containingSchema,
        string property)
    {
        JsonElement schema = Resolve(document, containingSchema);
        return Resolve(
            document,
            schema.GetProperty("properties").GetProperty(property));
    }

    public static void HasExactRequiredMembers(
        JsonElement schema,
        params string[] expected)
    {
        string[] actual = schema.GetProperty("required")
            .EnumerateArray()
            .Select(item => item.GetString()!)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            expected.Order(StringComparer.Ordinal).ToArray(),
            actual);
    }

    public static void HasEnum(JsonElement schema, params string[] expected)
    {
        string[] actual = schema.GetProperty("enum")
            .EnumerateArray()
            .Select(item => item.GetString()!)
            .ToArray();

        Assert.Equal(expected, actual);
    }

    public static void IsNullable(JsonElement schema)
    {
        if (schema.TryGetProperty("nullable", out JsonElement nullable))
        {
            Assert.True(nullable.GetBoolean());
            return;
        }

        if (schema.TryGetProperty("type", out JsonElement type)
            && type.ValueKind == JsonValueKind.Array)
        {
            Assert.Contains(
                type.EnumerateArray(),
                item => item.GetString() == "null");
            return;
        }

        foreach (string union in new[] { "anyOf", "oneOf" })
        {
            if (schema.TryGetProperty(union, out JsonElement alternatives)
                && alternatives.EnumerateArray().Any(IsNullSchema))
            {
                return;
            }
        }

        Assert.Fail("The OpenAPI schema is not explicitly nullable.");
    }

    public static JsonElement Resolve(
        JsonElement document,
        JsonElement schema)
    {
        if (schema.TryGetProperty("$ref", out JsonElement reference))
        {
            string name = reference.GetString()!.Split('/')[^1];
            return document.GetProperty("components")
                .GetProperty("schemas")
                .GetProperty(name);
        }

        foreach (string union in new[] { "oneOf", "anyOf" })
        {
            if (!schema.TryGetProperty(union, out JsonElement alternatives))
            {
                continue;
            }

            JsonElement nonNull = alternatives
                .EnumerateArray()
                .Single(alternative => !IsNullSchema(alternative));
            return Resolve(document, nonNull);
        }

        return schema;
    }

    private static bool IsNullSchema(JsonElement schema)
    {
        return schema.TryGetProperty("type", out JsonElement type)
            && type.ValueKind == JsonValueKind.String
            && type.GetString() == "null";
    }
}
