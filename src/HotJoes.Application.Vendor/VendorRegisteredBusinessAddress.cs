namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredBusinessAddress(
    string CanonicalAddressId,
    string? RecipientOrOrganisationName,
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string PostTown,
    string Postcode,
    string? County);
