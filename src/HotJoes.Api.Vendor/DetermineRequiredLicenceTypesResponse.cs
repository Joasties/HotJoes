namespace HotJoes.Api.Vendor;

public sealed record DetermineRequiredLicenceTypesResponse(
    string RuleSetVersion,
    IReadOnlyList<RequiredLicenceTypeResponse> Items);

public sealed record RequiredLicenceTypeResponse(
    string RequiredLicenceType,
    bool IsRequired);
