namespace HotJoes.Application.Address;

public interface IAddressSearchService
{
    IReadOnlyList<AddressSearchCandidate> Search(
        string query,
        TradingLocation tradingLocation);
}
