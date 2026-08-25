using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public static class AddressAuthorityApplicability
{
    public static bool IsSatisfiedBy(
        TradingLocation tradingLocation,
        FoodRegistrationAuthority? foodRegistrationAuthority,
        PrimaryTradingAuthority? primaryTradingAuthority)
    {
        if (foodRegistrationAuthority is null)
        {
            return false;
        }

        return tradingLocation switch
        {
            TradingLocation.Stall => primaryTradingAuthority is not null,
            TradingLocation.Restaurant or TradingLocation.Kitchen =>
                primaryTradingAuthority is null,
            _ => false
        };
    }
}
