using System.Net;
using System.Text;
using System.Text.Json;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorApiUnexpectedFailureTests
{
    [Fact]
    public async Task UnhandledApplicationException_ReturnsSafeCentralUnexpectedFailure()
    {
        using var factory = new VendorApiFactory();
        factory.Registration.NextException = new InvalidOperationException(
            "Npgsql database secret stack detail");
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            new StringContent(
                VendorApiTestData.CompleteRequest,
                Encoding.UTF8,
                "application/json"));

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        string content = await response.Content.ReadAsStringAsync();

        using JsonDocument body = JsonDocument.Parse(content);
        Assert.Equal("unexpectedFailure", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            JsonValueKind.Null,
            body.RootElement.GetProperty("validationErrors").ValueKind);
        Assert.DoesNotContain("Npgsql", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("database", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stack", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("InvalidOperationException", content, StringComparison.Ordinal);
    }
}
