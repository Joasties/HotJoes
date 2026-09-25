using HotJoes.Application.Address;

namespace HotJoes.IntegrationTests;

public sealed class AddressSearchContractTests
{
    [Fact]
    public void Search_ReturnsOnlyMatchingLocationWithStableOpaqueReference()
    {
        var sut = new StubAddressApplication();
        sut.AddKnownResult(
            "addr-stall-hotjoes-001",
            TradingLocation.Stall,
            Result("Hot Joes Stall", "Greenwich Market", "SE10 9HZ", true));
        sut.AddKnownResult(
            "addr-restaurant-hotjoes-001",
            TradingLocation.Restaurant,
            Result("Hot Joes", "Blackheath Avenue", "SE10 8XJ", false));

        AddressSearchCandidate match = Assert.Single(
            sut.Search("greenwich", TradingLocation.Stall));

        Assert.Equal("addr-stall-hotjoes-001", match.AddressResolutionReference);
        Assert.Equal(
            ["Hot Joes Stall", "Greenwich Market", "London", "SE10 9HZ"],
            match.DisplayLines);
        Assert.Empty(sut.Search("greenwich", TradingLocation.Restaurant));
        Assert.IsType<AddressResolutionResult.Success>(
            sut.ResolveAddress(match.AddressResolutionReference, TradingLocation.Stall));
    }

    private static CompleteAddressResult Result(
        string organisation,
        string addressLine,
        string postcode,
        bool stall) =>
        new(
            $"canonical-{postcode}",
            organisation,
            addressLine,
            null,
            null,
            "London",
            postcode,
            null,
            "Royal Borough of Greenwich",
            stall ? "Royal Borough of Greenwich" : null);
}
