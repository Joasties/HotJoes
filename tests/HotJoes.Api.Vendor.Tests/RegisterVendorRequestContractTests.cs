using System.Text.Json;
using HotJoes.Api.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class RegisterVendorRequestContractTests
{
    private const string CompleteRequest = """
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
            "serviceIncludesHotFood": false,
            "alcoholService": false
          },
          "primaryContact": {
            "contactName": "Jordan Smith",
            "contactEmail": "jordan@example.test",
            "contactTelephone": "+442079460123"
          },
          "addressResolutionReference": "addr-resolution-example",
          "website": "https://example.test",
          "businessDescription": "Evening food delivery kitchen.",
          "registrationDeclarations": {
            "authorisedToRegisterBusiness": false,
            "informationAccurate": false,
            "acceptHotJoesPlatformTerms": false
          }
        }
        """;

    [Fact]
    public void Deserialize_CompleteShape_MapsEveryClientAuthoredMemberToCommand()
    {
        RegisterVendorRequest request = Deserialize(CompleteRequest);

        var command = new RegisterVendorRequestMapper().Map(request);

        Assert.Equal("Hot Joe's Kitchen", command.TradingName);
        Assert.Equal("Hot Joe's Foods Limited", command.LegalOperatorName);
        Assert.Equal(LegalOperatorType.LimitedCompany, command.LegalOperatorType);
        Assert.Equal("AB123456", command.CompanyRegistrationNumber);
        Assert.Equal(TradingLocation.Kitchen, command.TradingLocation);
        Assert.Equal(new TimeOnly(17, 0), command.OpeningHoursStartTime);
        Assert.Equal(new TimeOnly(2, 0), command.OpeningHoursEndTime);
        Assert.False(command.ServiceIncludesHotFood);
        Assert.False(command.AlcoholService);
        Assert.Equal("Jordan Smith", command.ContactName);
        Assert.Equal("jordan@example.test", command.ContactEmail);
        Assert.Equal("+442079460123", command.ContactTelephone);
        Assert.Equal("addr-resolution-example", command.AddressResolutionReference);
        Assert.Equal("https://example.test", command.Website);
        Assert.Equal("Evening food delivery kitchen.", command.BusinessDescription);
        Assert.False(command.AuthorisedToRegisterBusiness);
        Assert.False(command.InformationAccurate);
        Assert.False(command.AcceptHotJoesPlatformTerms);
    }

    [Fact]
    public void Deserialize_RequiredFalseBooleans_PreservesSuppliedPresenceAndFalseValue()
    {
        RegisterVendorRequest request = Deserialize(CompleteRequest);

        RegisterVendorTradingCharacteristicsRequest tradingCharacteristics =
            Assert.IsType<RegisterVendorTradingCharacteristicsRequest>(
                request.TradingCharacteristics);
        RegisterVendorRegistrationDeclarationsRequest registrationDeclarations =
            Assert.IsType<RegisterVendorRegistrationDeclarationsRequest>(
                request.RegistrationDeclarations);

        Assert.Equal(false, tradingCharacteristics.ServiceIncludesHotFood);
        Assert.Equal(false, tradingCharacteristics.AlcoholService);
        Assert.Equal(false, registrationDeclarations.AuthorisedToRegisterBusiness);
        Assert.Equal(false, registrationDeclarations.InformationAccurate);
        Assert.Equal(false, registrationDeclarations.AcceptHotJoesPlatformTerms);
    }

    [Fact]
    public void Deserialize_OmittedRequiredBoolean_PreservesAbsenceRatherThanDefaultingFalse()
    {
        string json = CompleteRequest.Replace(
            "\"serviceIncludesHotFood\": false,",
            string.Empty,
            StringComparison.Ordinal);

        RegisterVendorRequest request = Deserialize(json);

        RegisterVendorTradingCharacteristicsRequest tradingCharacteristics =
            Assert.IsType<RegisterVendorTradingCharacteristicsRequest>(
                request.TradingCharacteristics);

        Assert.Null(tradingCharacteristics.ServiceIncludesHotFood);
    }

    [Theory]
    [InlineData("\"website\": \"https://example.test\",", "\"website\": null,")]
    [InlineData("\"businessDescription\": \"Evening food delivery kitchen.\",", "\"businessDescription\": null,")]
    public void Deserialize_OptionalNull_RepresentsAbsence(
        string existingMember,
        string nullMember)
    {
        RegisterVendorRequest request = Deserialize(
            CompleteRequest.Replace(
                existingMember,
                nullMember,
                StringComparison.Ordinal));

        Assert.True(request.Website is null || request.BusinessDescription is null);
    }

    [Fact]
    public void Deserialize_UnknownMember_IgnoresItWithoutChangingKnownValues()
    {
        string json = CompleteRequest.Replace(
            "\"tradingName\": \"Hot Joe's Kitchen\",",
            "\"tradingName\": \"Hot Joe's Kitchen\",\n  \"futureCompatibleMember\": 42,",
            StringComparison.Ordinal);

        RegisterVendorRequest request = Deserialize(json);

        Assert.Equal("Hot Joe's Kitchen", request.TradingName);
    }

    [Fact]
    public void PublicRequestShape_ExcludesAddressOwnedAndServerOwnedValues()
    {
        string[] prohibitedProperties =
        [
            "CanonicalAddressId",
            "BusinessAddressSnapshot",
            "FoodRegistrationAuthority",
            "PrimaryTradingAuthority",
            "VendorId",
            "VendorState",
            "RegistrationIdentity",
            "SemanticFingerprint",
            "RegisteredAt"
        ];

        string[] publicProperties = typeof(RegisterVendorRequest)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain(
            publicProperties,
            property => prohibitedProperties.Contains(property, StringComparer.Ordinal));
    }

    private static RegisterVendorRequest Deserialize(string json)
    {
        return JsonSerializer.Deserialize<RegisterVendorRequest>(
                json,
                VendorApiJsonOptions.Create())
            ?? throw new InvalidOperationException("The request did not deserialize.");
    }
}
