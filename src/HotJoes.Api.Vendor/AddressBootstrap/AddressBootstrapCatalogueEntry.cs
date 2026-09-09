using HotJoes.Application.Address;

namespace HotJoes.Api.Vendor.AddressBootstrap;

public sealed record AddressBootstrapCatalogueEntry(
    string Reference,
    TradingLocation TradingLocation,
    CompleteAddressResult Result);
