using System.Net;
using System.Text.Json;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class RetrieveRegisteredVendorEndpointTests
{
    [Fact]
    public async Task Get_ExistingVendor_InvokesApplicationOnceAndReturnsCompleteDetails()
    {
        using var factory = new VendorApiFactory();
        var vendorId = new VendorId(
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        factory.Retrieval.NextResult = RetrieveRegisteredVendorResult.VendorFound(
            VendorApiTestData.CreateDetails(vendorId));
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/vendors/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, factory.Retrieval.InvocationCount);
        Assert.Equal(vendorId, factory.Retrieval.LastVendorId);
        Assert.True(factory.Retrieval.LastCancellationTokenCanBeCanceled);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using JsonDocument body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal(
            "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            body.RootElement.GetProperty("vendorId").GetString());
        Assert.Equal(
            "Hot Joe's Kitchen",
            body.RootElement.GetProperty("tradingName").GetString());
        Assert.Equal(
            JsonValueKind.Null,
            body.RootElement.GetProperty("website").ValueKind);
    }

    [Fact]
    public async Task Get_UnknownVendor_ReturnsApprovedNotFoundEnvelope()
    {
        using var factory = new VendorApiFactory();
        factory.Retrieval.NextResult =
            RetrieveRegisteredVendorResult.VendorNotFound();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/vendors/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using JsonDocument body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal("vendorNotFound", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            JsonValueKind.Null,
            body.RootElement.GetProperty("validationErrors").ValueKind);
    }
}
