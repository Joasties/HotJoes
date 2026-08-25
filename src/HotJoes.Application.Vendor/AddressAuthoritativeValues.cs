using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed record AddressAuthoritativeValues
{
    public AddressAuthoritativeValues(
        CanonicalAddressId canonicalAddressId,
        BusinessAddressSnapshot businessAddressSnapshot,
        FoodRegistrationAuthority foodRegistrationAuthority,
        PrimaryTradingAuthority? primaryTradingAuthority)
    {
        ArgumentNullException.ThrowIfNull(canonicalAddressId);
        ArgumentNullException.ThrowIfNull(businessAddressSnapshot);
        ArgumentNullException.ThrowIfNull(foodRegistrationAuthority);

        CanonicalAddressId = canonicalAddressId;
        BusinessAddressSnapshot = businessAddressSnapshot;
        FoodRegistrationAuthority = foodRegistrationAuthority;
        PrimaryTradingAuthority = primaryTradingAuthority;
    }

    public CanonicalAddressId CanonicalAddressId { get; }

    public BusinessAddressSnapshot BusinessAddressSnapshot { get; }

    public FoodRegistrationAuthority FoodRegistrationAuthority { get; }

    public PrimaryTradingAuthority? PrimaryTradingAuthority { get; }
}
