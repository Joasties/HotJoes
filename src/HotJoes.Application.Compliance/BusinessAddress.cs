namespace HotJoes.Application.Compliance;

public sealed record BusinessAddress(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string PostTown,
    string Postcode,
    string? County,
    string? RecipientOrOrganisationName);
