using System.Net;
using System.Text;
using System.Text.Json;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorApiStructuralValidationTests
{
    public static TheoryData<string> MalformedBodies => new()
    {
        { "{" },
        { VendorApiTestData.CompleteRequest.Replace("true", "\"true\"", StringComparison.Ordinal) },
        { VendorApiTestData.CompleteRequest.Replace("\"kitchen\"", "\"unknown\"", StringComparison.Ordinal) },
        { VendorApiTestData.CompleteRequest.Replace("\"17:00:00\"", "\"17:00\"", StringComparison.Ordinal) },
        { VendorApiTestData.CompleteRequest.Replace("\"serviceIncludesHotFood\": true,", string.Empty, StringComparison.Ordinal) },
        { VendorApiTestData.CompleteRequest.Replace("\"registrationDeclarations\": {", "\"canonicalAddressId\": \"caller-value\",\n  \"registrationDeclarations\": {", StringComparison.Ordinal) }
    };

    [Theory]
    [MemberData(nameof(MalformedBodies))]
    public async Task Post_MalformedOrStructurallyInvalidBody_ReturnsRequestMalformedWithoutApplicationCall(
        string body)
    {
        using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            new StringContent(body, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.Registration.InvocationCount);
        using JsonDocument json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal("requestMalformed", json.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Post_CompatibleUnknownMember_IsIgnoredAndApplicationIsInvoked()
    {
        using var factory = new VendorApiFactory();
        factory.Registration.NextResult = RegisterVendorResult.Succeeded(
            new VendorId(Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")));
        using HttpClient client = factory.CreateClient();
        string body = VendorApiTestData.CompleteRequest.Replace(
            "\"tradingName\":",
            "\"futureMember\": 42,\n  \"tradingName\":",
            StringComparison.Ordinal);

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            new StringContent(body, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, factory.Registration.InvocationCount);
    }

    [Fact]
    public async Task Get_MalformedVendorId_ReturnsRequestMalformedWithoutApplicationCall()
    {
        using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/vendors/not-a-uuid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.Retrieval.InvocationCount);
        using JsonDocument json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal("requestMalformed", json.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Post_UnsupportedContentType_IsRejectedBeforeApplicationCall()
    {
        using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            new StringContent(VendorApiTestData.CompleteRequest, Encoding.UTF8, "text/plain"));

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        Assert.Equal(0, factory.Registration.InvocationCount);
    }

    [Theory]
    [InlineData("/weatherforecast")]
    [InlineData("/api/v1/vendors")]
    [InlineData("/vendors/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/orders")]
    public async Task UnsupportedRoute_IsNotExposed(string route)
    {
        using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(route);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
