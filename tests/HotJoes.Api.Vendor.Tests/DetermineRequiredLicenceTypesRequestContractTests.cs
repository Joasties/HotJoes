using System.Text.Json;
using HotJoes.Api.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class DetermineRequiredLicenceTypesRequestContractTests
{
    [Fact]
    public void Deserialize_ApprovedShape_MapsExactlySixControllingInputs()
    {
        DetermineRequiredLicenceTypesRequest request = JsonSerializer.Deserialize<
            DetermineRequiredLicenceTypesRequest>(
                ValidRequest,
                VendorApiJsonOptions.Create())!;

        var mapped = new DetermineRequiredLicenceTypesRequestMapper().Map(request);

        Assert.Equal(LegalOperatorType.LimitedCompany, mapped.LegalOperatorType);
        Assert.Equal(TradingLocation.Kitchen, mapped.TradingLocation);
        Assert.Equal(7, mapped.WeeklyOpeningHours.Days.Count);
        Assert.True(mapped.ServiceIncludesHotFood);
        Assert.False(mapped.AlcoholService);
        Assert.Equal("addr-resolution-example", mapped.AddressResolutionReference);
        Assert.Equal(
            [
                "LegalOperatorType",
                "TradingLocation",
                "WeeklyOpeningHours",
                "ServiceIncludesHotFood",
                "AlcoholService",
                "AddressResolutionReference"
            ],
            typeof(DetermineRequiredLicenceTypesRequest)
                .GetProperties()
                .Select(property => property.Name)
                .ToArray());
    }

    [Fact]
    public void Deserialize_RequiredFalseBooleans_PreservesPresenceAndValue()
    {
        DetermineRequiredLicenceTypesRequest request = JsonSerializer.Deserialize<
            DetermineRequiredLicenceTypesRequest>(
                ValidRequest.Replace(
                    "\"serviceIncludesHotFood\": true",
                    "\"serviceIncludesHotFood\": false",
                    StringComparison.Ordinal),
                VendorApiJsonOptions.Create())!;

        Assert.Equal(false, request.ServiceIncludesHotFood);
        Assert.Equal(false, request.AlcoholService);
    }

    [Fact]
    public void Deserialize_UnknownAddressAndDeterminationMembers_GainNoAuthority()
    {
        string json = ValidRequest.Replace(
            "\"addressResolutionReference\": \"addr-resolution-example\"",
            """
            "addressResolutionReference": "addr-resolution-example",
            "canonicalAddressId": "caller-authored",
            "jurisdiction": "caller-authored",
            "ruleSetVersion": "caller-selected",
            "items": []
            """,
            StringComparison.Ordinal);

        DetermineRequiredLicenceTypesRequest request = JsonSerializer.Deserialize<
            DetermineRequiredLicenceTypesRequest>(
                json,
                VendorApiJsonOptions.Create())!;

        Assert.Equal("addr-resolution-example", request.AddressResolutionReference);
        Assert.DoesNotContain(
            typeof(DetermineRequiredLicenceTypesRequest).GetProperties(),
            property => new[]
            {
                "CanonicalAddressId", "Jurisdiction", "RuleSetVersion", "Items"
            }.Contains(property.Name, StringComparer.Ordinal));
    }

    internal const string ValidRequest = """
        {
          "legalOperatorType": "limitedCompany",
          "tradingLocation": "kitchen",
          "weeklyOpeningHours": {
            "days": [
              { "day": "monday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
              { "day": "tuesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
              { "day": "wednesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
              { "day": "thursday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
              { "day": "friday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
              { "day": "saturday", "isClosed": false, "isOpenAllDay": true, "startTime": null, "endTime": null },
              { "day": "sunday", "isClosed": true, "isOpenAllDay": false, "startTime": null, "endTime": null }
            ]
          },
          "serviceIncludesHotFood": true,
          "alcoholService": false,
          "addressResolutionReference": "addr-resolution-example"
        }
        """;
}
