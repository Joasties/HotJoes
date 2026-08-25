using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class AddressResolutionInvokerTests
{
    [Fact]
    public void Resolve_WithReferenceAndTradingLocation_InvokesResolverOnceAndReturnsItsResult()
    {
        const string addressResolutionReference = "address-resolution-reference-001";
        var expectedResult = AddressResolutionResult.Succeeded(CreateAuthoritativeValues());
        var resolver = new RecordingAddressResolver(expectedResult);
        var sut = new AddressResolutionInvoker(resolver);

        var actual = sut.Resolve(addressResolutionReference, TradingLocation.Stall);

        Assert.Same(expectedResult, actual);
        Assert.Equal(1, resolver.InvocationCount);
        Assert.Equal(addressResolutionReference, resolver.ReceivedReference);
        Assert.Equal(TradingLocation.Stall, resolver.ReceivedTradingLocation);
    }

    private static AddressAuthoritativeValues CreateAuthoritativeValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId("canonical-address-001"),
            new BusinessAddressSnapshot(
                "10 Market Street",
                null,
                null,
                "GREENWICH",
                "SE10 9NN",
                null,
                "Hot Joes Limited"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
    }

    private sealed class RecordingAddressResolver : IAddressResolver
    {
        private readonly AddressResolutionResult _result;

        public RecordingAddressResolver(AddressResolutionResult result)
        {
            _result = result;
        }

        public int InvocationCount { get; private set; }

        public string? ReceivedReference { get; private set; }

        public TradingLocation? ReceivedTradingLocation { get; private set; }

        public AddressResolutionResult Resolve(
            string addressResolutionReference,
            TradingLocation tradingLocation)
        {
            InvocationCount++;
            ReceivedReference = addressResolutionReference;
            ReceivedTradingLocation = tradingLocation;

            return _result;
        }
    }
}
