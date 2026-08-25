namespace HotJoes.Application.Address;

public sealed record CompleteAddressResult
{
    public CompleteAddressResult(
        string canonicalAddressId,
        string? addressLine1,
        string addressLine2,
        string? addressLine3,
        string? addressLine4,
        string postTown,
        string postcode,
        string? county,
        string foodRegistrationAuthority,
        string? primaryTradingAuthority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(canonicalAddressId);
        ArgumentException.ThrowIfNullOrWhiteSpace(addressLine2);
        ArgumentException.ThrowIfNullOrWhiteSpace(postTown);
        ArgumentException.ThrowIfNullOrWhiteSpace(postcode);
        ArgumentException.ThrowIfNullOrWhiteSpace(foodRegistrationAuthority);

        CanonicalAddressId = canonicalAddressId;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        AddressLine4 = addressLine4;
        PostTown = postTown;
        Postcode = postcode;
        County = county;
        FoodRegistrationAuthority = foodRegistrationAuthority;
        PrimaryTradingAuthority = primaryTradingAuthority;
    }

    public string CanonicalAddressId { get; }

    public string? AddressLine1 { get; }

    public string AddressLine2 { get; }

    public string? AddressLine3 { get; }

    public string? AddressLine4 { get; }

    public string PostTown { get; }

    public string Postcode { get; }

    public string? County { get; }

    public string FoodRegistrationAuthority { get; }

    public string? PrimaryTradingAuthority { get; }
}
