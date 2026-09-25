namespace HotJoes.Api.Vendor;

public sealed record AddressSearchResponse(
    IReadOnlyList<AddressSearchResultResponse> Results);

public sealed record AddressSearchResultResponse(
    string AddressResolutionReference,
    IReadOnlyList<string> DisplayLines);
