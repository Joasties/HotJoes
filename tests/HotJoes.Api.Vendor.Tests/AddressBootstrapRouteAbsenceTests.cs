namespace HotJoes.Api.Vendor.Tests;

public sealed class AddressBootstrapRouteAbsenceTests
{
    [Theory]
    [InlineData("/addresses")]
    [InlineData("/address-resolution")]
    [InlineData("/address-bootstrap-catalogue")]
    public async Task AI_API_005_DoesNotExposeAddressBootstrapRoutes(string route)
    {
        await using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(route);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
