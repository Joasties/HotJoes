using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HotJoes.Api.Vendor.Tests;

public static class VendorApiBoundaryTestHelpers
{
    public static StringContent JsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static string RemoveMember(string json, string path)
    {
        JsonObject root = ParseObject(json);
        (JsonObject parent, string member) = ResolveParent(root, path);
        Assert.True(parent.Remove(member), $"Member '{path}' was not present.");
        return root.ToJsonString();
    }

    public static string ReplaceMemberWithWrongToken(string json, string path)
    {
        JsonObject root = ParseObject(json);
        (JsonObject parent, string member) = ResolveParent(root, path);
        JsonNode? existing = parent[member];
        Assert.NotNull(existing);

        parent[member] = existing switch
        {
            JsonObject => JsonValue.Create("not-an-object"),
            JsonValue value when value.TryGetValue<bool>(out _) =>
                JsonValue.Create("not-a-boolean"),
            _ => JsonValue.Create(42)
        };

        return root.ToJsonString();
    }

    public static string SetOptionalNull(string json, string path)
    {
        JsonObject root = ParseObject(json);
        (JsonObject parent, string member) = ResolveParent(root, path);
        parent[member] = null;
        return root.ToJsonString();
    }

    public static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }

    private static JsonObject ParseObject(string json)
    {
        return Assert.IsType<JsonObject>(JsonNode.Parse(json));
    }

    private static (JsonObject Parent, string Member) ResolveParent(
        JsonObject root,
        string path)
    {
        string[] segments = path.Split('.');
        JsonObject parent = root;

        foreach (string segment in segments[..^1])
        {
            parent = Assert.IsType<JsonObject>(parent[segment]);
        }

        return (parent, segments[^1]);
    }
}
