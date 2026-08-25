using HotJoes.Application.Address;

namespace HotJoes.IntegrationTests;

public sealed class AddressSemanticOutcomeContractTests
{
    [Fact]
    public void Resolve_WithUnknownOrFabricatedReference_ReturnsInvalidReference()
    {
        var sut = new StubAddressApplication();

        var actual = sut.ResolveAddress(
            "fabricated-address-resolution-reference",
            TradingLocation.Stall);

        Assert.IsType<AddressResolutionResult.InvalidReference>(actual);
    }

    [Fact]
    public void Resolve_WithKnownResultMissingCanonicalAddressId_ReturnsInvalidAddressResult()
    {
        var sut = new StubAddressApplication();
        var reference = sut.AddKnownResultWithoutCanonicalAddressId(
            TradingLocation.Stall);

        var actual = sut.ResolveAddress(reference, TradingLocation.Stall);

        Assert.IsType<AddressResolutionResult.InvalidAddressResult>(actual);
    }
}
