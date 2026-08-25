namespace HotJoes.Application.Address;

public interface IAddressResolutionService
{
    AddressResolutionResult ResolveAddress(
        string addressResolutionReference,
        TradingLocation tradingLocation);
}
