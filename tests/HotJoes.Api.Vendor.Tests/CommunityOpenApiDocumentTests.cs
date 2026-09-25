using System.Text.Json;

namespace HotJoes.Api.Vendor.Tests;

public sealed class CommunityOpenApiDocumentTests
{
    [Fact]
    public async Task AI_API_007_CommunityOperationDescribesExactApprovedContract()
    {
        await using VendorOpenApiFactory factory = new();
        using HttpClient client = factory.CreateClient();
        using JsonDocument document = JsonDocument.Parse(
            await client.GetStringAsync("/openapi/v1.json"));
        JsonElement root = document.RootElement;
        JsonElement operation = OpenApiDocumentAssertions.Operation(
            root,
            "/community-participations",
            "post");

        JsonElement request = OpenApiDocumentAssertions.RequestSchema(root, operation);
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            request,
            "vendorId",
            "contactPreference");
        Assert.Equal("uuid", OpenApiDocumentAssertions.PropertySchema(
            root, request, "vendorId").GetProperty("format").GetString());
        OpenApiDocumentAssertions.HasEnum(
            OpenApiDocumentAssertions.PropertySchema(
                root, request, "contactPreference"),
            "email",
            "sms",
            "whatsApp");

        Assert.Equal(
            new[] { "201", "400", "404", "409", "503" },
            operation.GetProperty("responses").EnumerateObject()
                .Select(response => response.Name)
                .Order(StringComparer.Ordinal));
        Assert.True(operation.GetProperty("responses").GetProperty("201")
            .GetProperty("headers").TryGetProperty("Location", out _));

        JsonElement success = OpenApiDocumentAssertions.ResponseSchema(
            root,
            operation,
            "201");
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            success,
            "communityParticipationId",
            "vendorId",
            "contactPreference",
            "joinedAt");
        Assert.Equal("uuid", OpenApiDocumentAssertions.PropertySchema(
            root, success, "communityParticipationId")
            .GetProperty("format").GetString());
        Assert.Equal("uuid", OpenApiDocumentAssertions.PropertySchema(
            root, success, "vendorId").GetProperty("format").GetString());
        Assert.Equal("date-time", OpenApiDocumentAssertions.PropertySchema(
            root, success, "joinedAt").GetProperty("format").GetString());
    }
}
