using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public static class AddressAuthoritativeValuesTranslator
{
    public static AddressAuthoritativeValues Translate(
        string canonicalAddressId,
        BusinessAddressSnapshot snapshot,
        string foodRegistrationAuthority,
        string? primaryTradingAuthority)
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId(canonicalAddressId),
            snapshot,
            new FoodRegistrationAuthority(foodRegistrationAuthority),
            primaryTradingAuthority is null
                ? null
                : new PrimaryTradingAuthority(primaryTradingAuthority));
    }
}
