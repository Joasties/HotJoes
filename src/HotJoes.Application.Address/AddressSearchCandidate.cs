namespace HotJoes.Application.Address;

public sealed record AddressSearchCandidate(
    string AddressResolutionReference,
    IReadOnlyList<string> DisplayLines);
