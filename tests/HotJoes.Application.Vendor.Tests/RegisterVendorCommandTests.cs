using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorCommandTests
{
    [Fact]
    public void Construction_WithCompleteClientAuthoredIntent_RetainsEverySuppliedValue()
    {
        var openingHoursStartTime = new TimeOnly(23, 0);
        var openingHoursEndTime = new TimeOnly(5, 0);

        var command = new RegisterVendorCommand(
            tradingName: "Hot Joes Greenwich",
            legalOperatorName: "Hot Joes Limited",
            legalOperatorType: LegalOperatorType.LimitedCompany,
            companyRegistrationNumber: "12345678",
            tradingLocation: TradingLocation.Stall,
            openingHoursStartTime: openingHoursStartTime,
            openingHoursEndTime: openingHoursEndTime,
            serviceIncludesHotFood: true,
            alcoholService: false,
            contactName: "Joseph Bloggs",
            contactEmail: "joe@hotjoes.example",
            contactTelephone: "020 7946 0123",
            addressResolutionReference: "address-resolution-reference-001",
            website: "https://hotjoes.example",
            businessDescription: "Hot food from our Greenwich market stall.",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);

        Assert.Equal("Hot Joes Greenwich", command.TradingName);
        Assert.Equal("Hot Joes Limited", command.LegalOperatorName);
        Assert.Equal(LegalOperatorType.LimitedCompany, command.LegalOperatorType);
        Assert.Equal("12345678", command.CompanyRegistrationNumber);
        Assert.Equal(TradingLocation.Stall, command.TradingLocation);
        Assert.Equal(openingHoursStartTime, command.OpeningHoursStartTime);
        Assert.Equal(openingHoursEndTime, command.OpeningHoursEndTime);
        Assert.True(command.ServiceIncludesHotFood);
        Assert.False(command.AlcoholService);
        Assert.Equal("Joseph Bloggs", command.ContactName);
        Assert.Equal("joe@hotjoes.example", command.ContactEmail);
        Assert.Equal("020 7946 0123", command.ContactTelephone);
        Assert.Equal(
            "address-resolution-reference-001",
            command.AddressResolutionReference);
        Assert.Equal("https://hotjoes.example", command.Website);
        Assert.Equal(
            "Hot food from our Greenwich market stall.",
            command.BusinessDescription);
        Assert.True(command.AuthorisedToRegisterBusiness);
        Assert.True(command.InformationAccurate);
        Assert.True(command.AcceptHotJoesPlatformTerms);
    }

    [Fact]
    public void PublicSurface_ContainsOnlyImmutableClientAuthoredRegistrationInformation()
    {
        var expectedPropertyNames = new[]
        {
            "AcceptHotJoesPlatformTerms",
            "AddressResolutionReference",
            "AlcoholService",
            "AuthorisedToRegisterBusiness",
            "BusinessDescription",
            "CompanyRegistrationNumber",
            "ContactEmail",
            "ContactName",
            "ContactTelephone",
            "InformationAccurate",
            "LegalOperatorName",
            "LegalOperatorType",
            "OpeningHoursEndTime",
            "OpeningHoursStartTime",
            "ServiceIncludesHotFood",
            "TradingLocation",
            "TradingName",
            "Website"
        };

        var commandType = typeof(RegisterVendorCommand);
        var publicProperties = commandType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public);

        Assert.True(commandType.IsSealed);
        Assert.Equal(
            expectedPropertyNames,
            publicProperties.Select(property => property.Name).Order());
        Assert.All(
            publicProperties,
            property => Assert.Null(property.SetMethod));
    }
}
