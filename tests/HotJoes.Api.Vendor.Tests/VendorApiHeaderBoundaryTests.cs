using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorApiHeaderBoundaryTests
{
    [Theory]
    [InlineData("application/json")]
    [InlineData("*/*")]
    [InlineData("application/json;q=0.9, */*;q=0.1")]
    public async Task Post_SupportedAcceptVariant_ReturnsJson(string accept)
    {
        using var factory = new VendorApiFactory();
        factory.Registration.NextResult = RegisterVendorResult.Succeeded(
            new VendorId(Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")));
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Accept.ParseAdd(accept);

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            VendorApiBoundaryTestHelpers.JsonContent(
                VendorApiTestData.CompleteRequest));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Post_CallerIdentityAndCorrelationHeaders_DoNotControlResultOrAppearInResponse()
    {
        using var factory = new VendorApiFactory();
        factory.Registration.NextResult = RegisterVendorResult.Succeeded(
            new VendorId(Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")));
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Idempotency-Key", "caller-authored-key");
        client.DefaultRequestHeaders.Add("X-Correlation-ID", "caller-correlation");

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            VendorApiBoundaryTestHelpers.JsonContent(
                VendorApiTestData.CompleteRequest));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, factory.Registration.InvocationCount);
        Assert.False(response.Headers.Contains("Idempotency-Key"));
        Assert.False(response.Headers.Contains("X-Correlation-ID"));

        using JsonDocument body =
            await VendorApiBoundaryTestHelpers.ReadJsonAsync(response);
        Assert.False(body.RootElement.TryGetProperty("idempotencyKey", out _));
        Assert.False(body.RootElement.TryGetProperty("correlationId", out _));
    }

    [Fact]
    public async Task Get_SupportedAccept_ReturnsJsonWithoutCustomContractHeaders()
    {
        using var factory = new VendorApiFactory();
        var vendorId = new VendorId(
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        factory.Retrieval.NextResult = RetrieveRegisteredVendorResult.VendorFound(
            VendorApiTestData.CreateDetails(vendorId));
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        using HttpResponseMessage response = await client.GetAsync(
            "/vendors/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.False(response.Headers.Contains("Idempotency-Key"));
        Assert.False(response.Headers.Contains("X-Correlation-ID"));
    }
}
