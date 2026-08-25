using HotJoes.Application.Address;

namespace HotJoes.IntegrationTests;

public sealed class AddressResolutionContractTests
{
    [Fact]
    public void Resolve_RepeatedlyWithBoundTradingLocation_ReturnsOriginalImmutableResult()
    {
        var originalResult = CreateCompleteAddressResult("10 Market Street", "SE10 9NN");
        var changedResult = CreateCompleteAddressResult("99 Changed Street", "SE10 8ZZ");
        var sut = new StubAddressApplication();
        var reference = sut.SelectAddress(TradingLocation.Stall, originalResult);

        var first = sut.ResolveAddress(reference, TradingLocation.Stall);
        sut.SimulateCurrentAddressDataChange(
            originalResult.CanonicalAddressId,
            changedResult);
        var second = sut.ResolveAddress(reference, TradingLocation.Stall);

        var firstSuccess = Assert.IsType<AddressResolutionResult.Success>(first);
        var secondSuccess = Assert.IsType<AddressResolutionResult.Success>(second);
        Assert.Same(originalResult, firstSuccess.Result);
        Assert.Same(originalResult, secondSuccess.Result);
    }

    [Fact]
    public void Resolve_WithTradingLocationDifferentFromBoundContext_ReturnsInvalidAddressResult()
    {
        var sut = new StubAddressApplication();
        var reference = sut.SelectAddress(
            TradingLocation.Stall,
            CreateCompleteAddressResult("10 Market Street", "SE10 9NN"));

        var actual = sut.ResolveAddress(reference, TradingLocation.Restaurant);

        Assert.IsType<AddressResolutionResult.InvalidAddressResult>(actual);
    }

    private static CompleteAddressResult CreateCompleteAddressResult(
        string addressLine2,
        string postcode)
    {
        return new CompleteAddressResult(
            "canonical-address-001",
            "Hot Joes Limited",
            addressLine2,
            null,
            null,
            "GREENWICH",
            postcode,
            null,
            "Greenwich Borough Council",
            "Greenwich Borough Council");
    }
}
