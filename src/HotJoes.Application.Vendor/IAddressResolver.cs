using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public interface IAddressResolver
{
    AddressResolutionResult Resolve(
        string addressResolutionReference,
        TradingLocation tradingLocation);
}
