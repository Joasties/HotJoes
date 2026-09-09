using HotJoes.Api.Vendor.AddressBootstrap;
using HotJoes.Application.Address;

namespace HotJoes.IntegrationTests;

public sealed class SyntheticAddressBootstrapCatalogueTests
{
    [Fact]
    public void AI_ADDR_008_LoadsCompleteDeterministicCatalogue()
    {
        string path = WriteCatalogue(ValidCatalogue);

        AddressBootstrapCatalogue catalogue =
            AddressBootstrapCatalogue.Load(path);
        StubAddressApplication first = catalogue.CreateAddressApplication();
        StubAddressApplication second = catalogue.CreateAddressApplication();

        var firstResult = Assert.IsType<AddressResolutionResult.Success>(
            first.ResolveAddress("addr-stall-hotjoes-001", TradingLocation.Stall));
        var repeatedResult = Assert.IsType<AddressResolutionResult.Success>(
            second.ResolveAddress("addr-stall-hotjoes-001", TradingLocation.Stall));

        Assert.Equal("epic1-address-catalogue-v1", catalogue.Revision);
        Assert.Equal(firstResult.Result, repeatedResult.Result);
        Assert.Equal("canonical-hotjoes-001", firstResult.Result.CanonicalAddressId);
        Assert.Equal("Royal Borough of Greenwich", firstResult.Result.FoodRegistrationAuthority);
        Assert.Equal("Royal Borough of Greenwich", firstResult.Result.PrimaryTradingAuthority);
        Assert.IsType<AddressResolutionResult.InvalidAddressResult>(
            first.ResolveAddress(
                "addr-stall-hotjoes-001",
                TradingLocation.Restaurant));
        Assert.IsType<AddressResolutionResult.InvalidReference>(
            first.ResolveAddress("unknown-reference", TradingLocation.Stall));
    }

    [Theory]
    [InlineData("{\"revision\":\"v1\",\"entries\":[]}")]
    [InlineData("{\"revision\":\"v1\",\"entries\":[{\"reference\":\"duplicate\",\"tradingLocation\":\"stall\",\"canonicalAddressId\":\"one\",\"addressLine2\":\"1 Road\",\"postTown\":\"London\",\"postcode\":\"SE10 8XJ\",\"foodRegistrationAuthority\":\"Authority\",\"primaryTradingAuthority\":\"Authority\"},{\"reference\":\"duplicate\",\"tradingLocation\":\"stall\",\"canonicalAddressId\":\"two\",\"addressLine2\":\"2 Road\",\"postTown\":\"London\",\"postcode\":\"SE10 8XJ\",\"foodRegistrationAuthority\":\"Authority\",\"primaryTradingAuthority\":\"Authority\"}]}")]
    [InlineData("{\"revision\":\"v1\",\"entries\":[{\"reference\":\"incomplete\",\"tradingLocation\":\"stall\",\"canonicalAddressId\":\"one\",\"addressLine2\":\"1 Road\",\"postTown\":\"London\",\"postcode\":\"SE10 8XJ\",\"foodRegistrationAuthority\":\"Authority\"}]}")]
    public void AI_ADDR_008_RejectsInvalidCatalogue(string json)
    {
        string path = WriteCatalogue(json);

        Assert.Throws<InvalidOperationException>(
            () => AddressBootstrapCatalogue.Load(path));
    }

    private static string WriteCatalogue(string content)
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"hotjoes-address-{Guid.NewGuid():N}.json");
        File.WriteAllText(path, content);
        return path;
    }

    private const string ValidCatalogue = """
        {
          "revision": "epic1-address-catalogue-v1",
          "entries": [
            {
              "reference": "addr-stall-hotjoes-001",
              "tradingLocation": "stall",
              "canonicalAddressId": "canonical-hotjoes-001",
              "addressLine1": "Hot Joes",
              "addressLine2": "Blackheath Avenue",
              "postTown": "London",
              "postcode": "SE10 8XJ",
              "foodRegistrationAuthority": "Royal Borough of Greenwich",
              "primaryTradingAuthority": "Royal Borough of Greenwich"
            }
          ]
        }
        """;
}
