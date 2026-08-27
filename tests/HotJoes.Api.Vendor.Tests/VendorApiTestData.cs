using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public static class VendorApiTestData
{
    public const string CompleteRequest = """
        {
          "tradingName": "Hot Joe's Kitchen",
          "legalOperatorName": "Hot Joe's Foods Limited",
          "legalOperatorType": "limitedCompany",
          "companyRegistrationNumber": "AB123456",
          "tradingCharacteristics": {
            "tradingLocation": "kitchen",
            "openingHours": {
              "startTime": "17:00:00",
              "endTime": "02:00:00"
            },
            "serviceIncludesHotFood": true,
            "alcoholService": false
          },
          "primaryContact": {
            "contactName": "Jordan Smith",
            "contactEmail": "jordan@example.test",
            "contactTelephone": "+442079460123"
          },
          "addressResolutionReference": "addr-resolution-example",
          "website": null,
          "businessDescription": null,
          "registrationDeclarations": {
            "authorisedToRegisterBusiness": true,
            "informationAccurate": true,
            "acceptHotJoesPlatformTerms": true
          }
        }
        """;

    public static RegisteredVendorDetails CreateDetails(VendorId vendorId)
    {
        return new RegisteredVendorDetails(
            vendorId,
            new DateTimeOffset(2026, 8, 25, 10, 30, 0, TimeSpan.Zero),
            VendorState.PendingActivation,
            TradingPreference.Offline,
            LegalOperatorType.LimitedCompany,
            "Hot Joe's Foods Limited",
            "AB123456",
            "Hot Joe's Kitchen",
            new RegisteredVendorTradingCharacteristics(
                TradingLocation.Kitchen,
                new RegisteredVendorOpeningHours(
                    new TimeOnly(17, 0),
                    new TimeOnly(2, 0)),
                true,
                false),
            "Jordan Smith",
            "jordan@example.test",
            "+442079460123",
            "address-example-id",
            new RegisteredVendorBusinessAddress(
                "10 Example Street",
                null,
                null,
                "LONDON",
                "AB1 2CD",
                null,
                null),
            "Example authority",
            null,
            null,
            null);
    }
}
