using System.Net;
using System.Text.Json;

namespace HotJoes.Api.Vendor.Tests;

public sealed class AddressSearchEndpointTests
{
    [Fact]
    public async Task Search_ReturnsSelectableAddressOwnedResult()
    {
        await using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/address-search?query=Greenwich&tradingLocation=stall");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        JsonElement result = Assert.Single(
            document.RootElement.GetProperty("results").EnumerateArray());
        Assert.Equal(
            "addr-stall-hotjoes-001",
            result.GetProperty("addressResolutionReference").GetString());
        Assert.Equal(
            ["Hot Joes Stall", "Greenwich Market", "London", "SE10 9HZ"],
            result.GetProperty("displayLines")
                .EnumerateArray()
                .Select(line => line.GetString()!)
                .ToArray());
        Assert.False(result.TryGetProperty("canonicalAddressId", out _));
        Assert.False(result.TryGetProperty("foodRegistrationAuthority", out _));
        Assert.False(result.TryGetProperty("primaryTradingAuthority", out _));
    }

    [Theory]
    [InlineData("/address-search?query=&tradingLocation=stall")]
    [InlineData("/address-search?query=Greenwich&tradingLocation=van")]
    [InlineData("/address-search?query=aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa&tradingLocation=stall")]
    public async Task InvalidSearchRequest_ReturnsBadRequest(string path)
    {
        await using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
