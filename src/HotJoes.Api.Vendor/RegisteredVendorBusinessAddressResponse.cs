namespace HotJoes.Api.Vendor;

public sealed record RegisteredVendorBusinessAddressResponse(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string PostTown,
    string Postcode,
    string? County,
    string? RecipientOrOrganisationName);
