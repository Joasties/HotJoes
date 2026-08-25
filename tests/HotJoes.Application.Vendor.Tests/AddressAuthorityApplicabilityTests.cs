using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class AddressAuthorityApplicabilityTests
{
    [Theory]
    [InlineData(TradingLocation.Stall, true, true, true)]
    [InlineData(TradingLocation.Stall, true, false, false)]
    [InlineData(TradingLocation.Stall, false, true, false)]
    [InlineData(TradingLocation.Stall, false, false, false)]
    [InlineData(TradingLocation.Restaurant, true, false, true)]
    [InlineData(TradingLocation.Restaurant, true, true, false)]
    [InlineData(TradingLocation.Restaurant, false, false, false)]
    [InlineData(TradingLocation.Restaurant, false, true, false)]
    [InlineData(TradingLocation.Kitchen, true, false, true)]
    [InlineData(TradingLocation.Kitchen, true, true, false)]
    [InlineData(TradingLocation.Kitchen, false, false, false)]
    [InlineData(TradingLocation.Kitchen, false, true, false)]
    public void IsSatisfiedBy_WithAuthorityCombination_ReturnsApplicability(
        TradingLocation tradingLocation,
        bool includeFoodRegistrationAuthority,
        bool includePrimaryTradingAuthority,
        bool expected)
    {
        var foodRegistrationAuthority = includeFoodRegistrationAuthority
            ? new FoodRegistrationAuthority("Greenwich Borough Council")
            : null;
        var primaryTradingAuthority = includePrimaryTradingAuthority
            ? new PrimaryTradingAuthority("Greenwich Borough Council")
            : null;

        var actual = AddressAuthorityApplicability.IsSatisfiedBy(
            tradingLocation,
            foodRegistrationAuthority,
            primaryTradingAuthority);

        Assert.Equal(expected, actual);
    }
}
