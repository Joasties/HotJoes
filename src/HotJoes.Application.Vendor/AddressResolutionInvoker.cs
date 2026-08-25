using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class AddressResolutionInvoker
{
    private readonly IAddressResolver _resolver;

    public AddressResolutionInvoker(IAddressResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);

        _resolver = resolver;
    }

    public AddressResolutionResult Resolve(
        string addressResolutionReference,
        TradingLocation tradingLocation)
    {
        return _resolver.Resolve(addressResolutionReference, tradingLocation);
    }
}
