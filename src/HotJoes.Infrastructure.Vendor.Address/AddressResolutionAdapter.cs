using AddressApplication = HotJoes.Application.Address;
using VendorApplication = HotJoes.Application.Vendor;
using VendorDomain = HotJoes.Domain.Vendor;

namespace HotJoes.Infrastructure.Vendor.Address;

public sealed class AddressResolutionAdapter : VendorApplication.IAddressResolver
{
    private readonly AddressApplication.IAddressResolutionService _addressService;

    public AddressResolutionAdapter(
        AddressApplication.IAddressResolutionService addressService)
    {
        ArgumentNullException.ThrowIfNull(addressService);

        _addressService = addressService;
    }

    public VendorApplication.AddressResolutionResult Resolve(
        string addressResolutionReference,
        VendorDomain.TradingLocation tradingLocation)
    {
        var addressResult = _addressService.ResolveAddress(
            addressResolutionReference,
            MapTradingLocation(tradingLocation));

        return addressResult switch
        {
            AddressApplication.AddressResolutionResult.Success success =>
                TranslateSuccess(success.Result),
            AddressApplication.AddressResolutionResult.InvalidReference =>
                VendorApplication.AddressResolutionResult.ReferenceIsInvalid(),
            AddressApplication.AddressResolutionResult.InvalidAddressResult =>
                VendorApplication.AddressResolutionResult.InvalidAddress(),
            AddressApplication.AddressResolutionResult.TechnicalFailure =>
                VendorApplication.AddressResolutionResult.TemporarilyUnavailable(),
            _ => throw new InvalidOperationException(
                "The Address Resolution result is unsupported.")
        };
    }

    private static AddressApplication.TradingLocation MapTradingLocation(
        VendorDomain.TradingLocation tradingLocation)
    {
        return tradingLocation switch
        {
            VendorDomain.TradingLocation.Restaurant =>
                AddressApplication.TradingLocation.Restaurant,
            VendorDomain.TradingLocation.Stall =>
                AddressApplication.TradingLocation.Stall,
            VendorDomain.TradingLocation.Kitchen =>
                AddressApplication.TradingLocation.Kitchen,
            _ => throw new ArgumentOutOfRangeException(nameof(tradingLocation))
        };
    }

    private static VendorApplication.AddressResolutionResult TranslateSuccess(
        AddressApplication.CompleteAddressResult result)
    {
        var snapshot = VendorApplication.BusinessAddressSnapshotTranslator.Translate(
            result.AddressLine1,
            result.AddressLine2,
            result.AddressLine3,
            result.AddressLine4,
            result.PostTown,
            result.Postcode,
            result.County);

        var values = VendorApplication.AddressAuthoritativeValuesTranslator.Translate(
            result.CanonicalAddressId,
            snapshot,
            result.FoodRegistrationAuthority,
            result.PrimaryTradingAuthority);

        return VendorApplication.AddressResolutionResult.Succeeded(values);
    }
}
