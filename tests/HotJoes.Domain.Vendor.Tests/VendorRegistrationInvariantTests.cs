using HotJoes.Domain.Vendor;

namespace HotJoes.Domain.Vendor.Tests;

public sealed class VendorRegistrationInvariantTests
{
    [Theory]
    [InlineData(LegalOperatorType.LimitedCompany)]
    [InlineData(LegalOperatorType.LimitedLiabilityPartnership)]
    [InlineData(LegalOperatorType.CharitableIncorporatedOrganisation)]
    public void CreateRegistrationInformation_WhenLegalOperatorRequiresRegistrationNumberAndItIsAbsent_ThrowsArgumentException(
        LegalOperatorType legalOperatorType)
    {
        Assert.Throws<ArgumentException>(() => CreateRegistrationInformation(
            legalOperatorType,
            companyRegistrationNumber: null,
            TradingLocation.Kitchen,
            primaryTradingAuthority: null));
    }

    [Theory]
    [InlineData(LegalOperatorType.SoleTrader)]
    [InlineData(LegalOperatorType.GeneralPartnership)]
    [InlineData(LegalOperatorType.CharitableCommunityGroup)]
    public void CreateRegistrationInformation_WhenLegalOperatorProhibitsRegistrationNumberAndItIsPresent_ThrowsArgumentException(
        LegalOperatorType legalOperatorType)
    {
        Assert.Throws<ArgumentException>(() => CreateRegistrationInformation(
            legalOperatorType,
            new CompanyRegistrationNumber("12345678"),
            TradingLocation.Kitchen,
            primaryTradingAuthority: null));
    }

    [Fact]
    public void CreateRegistrationInformation_WhenTradingLocationIsStallAndPrimaryTradingAuthorityIsAbsent_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CreateRegistrationInformation(
            LegalOperatorType.SoleTrader,
            companyRegistrationNumber: null,
            TradingLocation.Stall,
            primaryTradingAuthority: null));
    }

    [Theory]
    [InlineData(TradingLocation.Restaurant)]
    [InlineData(TradingLocation.Kitchen)]
    public void CreateRegistrationInformation_WhenTradingLocationIsNotStallAndPrimaryTradingAuthorityIsPresent_ThrowsArgumentException(
        TradingLocation tradingLocation)
    {
        Assert.Throws<ArgumentException>(() => CreateRegistrationInformation(
            LegalOperatorType.SoleTrader,
            companyRegistrationNumber: null,
            tradingLocation,
            new PrimaryTradingAuthority("Greenwich Borough Council")));
    }

    private static VendorRegistrationInformation CreateRegistrationInformation(
        LegalOperatorType legalOperatorType,
        CompanyRegistrationNumber? companyRegistrationNumber,
        TradingLocation tradingLocation,
        PrimaryTradingAuthority? primaryTradingAuthority)
    {
        return new VendorRegistrationInformation(
            legalOperatorType,
            new VendorName("Jordan Smith"),
            new VendorName("Jordan's Evening Kitchen"),
            companyRegistrationNumber,
            new PrimaryContact(
                "Jordan Smith",
                new EmailAddress("jordan@example.test"),
                new TelephoneNumber("+44 20 7946 0123")),
            new CanonicalAddressId("address-example-id"),
            new BusinessAddressSnapshot(
                "10 Example Street",
                "Example Village",
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                "Example Foods"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority,
            new TradingCharacteristics(
                tradingLocation,
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));
    }
}
