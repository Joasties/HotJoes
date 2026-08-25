namespace HotJoes.Domain.Vendor;

public sealed record BusinessAddressSnapshot
{
    public BusinessAddressSnapshot(
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string postTown,
        string postcode,
        string? county,
        string? recipientOrOrganisationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addressLine1);
        ArgumentException.ThrowIfNullOrWhiteSpace(postTown);
        ArgumentException.ThrowIfNullOrWhiteSpace(postcode);

        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        PostTown = postTown;
        Postcode = postcode;
        County = county;
        RecipientOrOrganisationName = recipientOrOrganisationName;
    }

    public string AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string? AddressLine3 { get; }
    public string PostTown { get; }
    public string Postcode { get; }
    public string? County { get; }
    public string? RecipientOrOrganisationName { get; }
}
