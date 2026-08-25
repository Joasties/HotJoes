using AddressApplication = HotJoes.Application.Address;
using VendorApplication = HotJoes.Application.Vendor;
using VendorDomain = HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Vendor.Address;

namespace HotJoes.IntegrationTests;

public sealed class AddressResolutionAdapterTests
{
    [Fact]
    public void Resolve_WhenAddressSucceeds_TranslatesCompleteResultIntoVendorOwnedValues()
    {
        const string reference = "address-resolution-reference-001";
        var addressResult = new AddressApplication.CompleteAddressResult(
            "canonical-address-001",
            "Hot Joes Limited",
            "10 Market Street",
            "Market Quarter",
            null,
            "GREENWICH",
            "SE10 9NN",
            "Greater London",
            "Greenwich Borough Council",
            "Greenwich Borough Council");
        var addressService = new RecordingAddressResolutionService(
            AddressApplication.AddressResolutionResult.Succeeded(addressResult));
        VendorApplication.IAddressResolver sut =
            new AddressResolutionAdapter(addressService);

        var actual = sut.Resolve(reference, VendorDomain.TradingLocation.Stall);

        var success = Assert.IsType<VendorApplication.AddressResolutionResult.Success>(actual);
        Assert.Equal("canonical-address-001", success.Values.CanonicalAddressId.Value);
        Assert.Equal("Hot Joes Limited", success.Values.BusinessAddressSnapshot.RecipientOrOrganisationName);
        Assert.Equal("10 Market Street", success.Values.BusinessAddressSnapshot.AddressLine1);
        Assert.Equal("Market Quarter", success.Values.BusinessAddressSnapshot.AddressLine2);
        Assert.Null(success.Values.BusinessAddressSnapshot.AddressLine3);
        Assert.Equal("GREENWICH", success.Values.BusinessAddressSnapshot.PostTown);
        Assert.Equal("SE10 9NN", success.Values.BusinessAddressSnapshot.Postcode);
        Assert.Equal("Greater London", success.Values.BusinessAddressSnapshot.County);
        Assert.Equal("Greenwich Borough Council", success.Values.FoodRegistrationAuthority.Value);
        Assert.Equal("Greenwich Borough Council", success.Values.PrimaryTradingAuthority?.Value);
        Assert.Equal(1, addressService.InvocationCount);
        Assert.Equal(reference, addressService.ReceivedReference);
        Assert.Equal(AddressApplication.TradingLocation.Stall, addressService.ReceivedTradingLocation);
    }

    [Theory]
    [InlineData(VendorDomain.TradingLocation.Restaurant, AddressApplication.TradingLocation.Restaurant)]
    [InlineData(VendorDomain.TradingLocation.Stall, AddressApplication.TradingLocation.Stall)]
    [InlineData(VendorDomain.TradingLocation.Kitchen, AddressApplication.TradingLocation.Kitchen)]
    public void Resolve_WithTradingLocation_MapsContextExactly(
        VendorDomain.TradingLocation vendorTradingLocation,
        AddressApplication.TradingLocation expectedAddressTradingLocation)
    {
        var addressService = new RecordingAddressResolutionService(
            AddressApplication.AddressResolutionResult.ReferenceIsInvalid());
        VendorApplication.IAddressResolver sut =
            new AddressResolutionAdapter(addressService);

        _ = sut.Resolve("address-resolution-reference-001", vendorTradingLocation);

        Assert.Equal(1, addressService.InvocationCount);
        Assert.Equal(expectedAddressTradingLocation, addressService.ReceivedTradingLocation);
    }

    [Fact]
    public void Resolve_WhenAddressReturnsInvalidReference_ReturnsVendorInvalidReference()
    {
        var addressService = new RecordingAddressResolutionService(
            AddressApplication.AddressResolutionResult.ReferenceIsInvalid());
        VendorApplication.IAddressResolver sut =
            new AddressResolutionAdapter(addressService);

        var actual = sut.Resolve(
            "fabricated-address-resolution-reference",
            VendorDomain.TradingLocation.Stall);

        Assert.IsType<VendorApplication.AddressResolutionResult.InvalidReference>(actual);
    }

    [Fact]
    public void Resolve_WhenAddressReturnsInvalidAddressResult_ReturnsVendorInvalidAddressResult()
    {
        var addressService = new RecordingAddressResolutionService(
            AddressApplication.AddressResolutionResult.InvalidAddress());
        VendorApplication.IAddressResolver sut =
            new AddressResolutionAdapter(addressService);

        var actual = sut.Resolve(
            "address-resolution-reference-001",
            VendorDomain.TradingLocation.Stall);

        Assert.IsType<VendorApplication.AddressResolutionResult.InvalidAddressResult>(actual);
    }

    private sealed class RecordingAddressResolutionService
        : AddressApplication.IAddressResolutionService
    {
        private readonly AddressApplication.AddressResolutionResult _result;

        public RecordingAddressResolutionService(
            AddressApplication.AddressResolutionResult result)
        {
            _result = result;
        }

        public int InvocationCount { get; private set; }

        public string? ReceivedReference { get; private set; }

        public AddressApplication.TradingLocation? ReceivedTradingLocation { get; private set; }

        public AddressApplication.AddressResolutionResult ResolveAddress(
            string addressResolutionReference,
            AddressApplication.TradingLocation tradingLocation)
        {
            InvocationCount++;
            ReceivedReference = addressResolutionReference;
            ReceivedTradingLocation = tradingLocation;

            return _result;
        }
    }
}
