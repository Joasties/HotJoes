using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class DetermineRequiredLicenceTypesRequestTests
{
    [Fact]
    public void VR_DETERMINATION_001_Construction_RetainsExactlyTheSixControllingInputs()
    {
        RegisterVendorWeeklyOpeningHours hours =
            RegisterVendorWeeklyOpeningHours.EveryDay(
                new TimeOnly(9, 0),
                new TimeOnly(17, 0));

        var request = new DetermineRequiredLicenceTypesRequest(
            LegalOperatorType.LimitedCompany,
            TradingLocation.Stall,
            hours,
            serviceIncludesHotFood: true,
            alcoholService: false,
            "address-resolution-reference-001");

        Assert.Equal(LegalOperatorType.LimitedCompany, request.LegalOperatorType);
        Assert.Equal(TradingLocation.Stall, request.TradingLocation);
        Assert.Same(hours, request.WeeklyOpeningHours);
        Assert.True(request.ServiceIncludesHotFood);
        Assert.False(request.AlcoholService);
        Assert.Equal(
            "address-resolution-reference-001",
            request.AddressResolutionReference);
    }

    [Fact]
    public void AI_APP_005_PublicSurface_IsClosedImmutableAndTransportIndependent()
    {
        string[] expectedProperties =
        [
            "AddressResolutionReference",
            "AlcoholService",
            "LegalOperatorType",
            "ServiceIncludesHotFood",
            "TradingLocation",
            "WeeklyOpeningHours"
        ];

        Type requestType = typeof(DetermineRequiredLicenceTypesRequest);
        PropertyInfo[] properties = requestType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public);

        Assert.True(requestType.IsSealed);
        Assert.Equal(
            expectedProperties,
            properties.Select(property => property.Name).Order());
        Assert.All(properties, property => Assert.Null(property.SetMethod));
        Assert.DoesNotContain(properties, property =>
            property.PropertyType.Namespace?.StartsWith(
                "Microsoft.AspNetCore",
                StringComparison.Ordinal) is true);
    }
}
