using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisteredVendorDetailsMapperTests
{
    [Fact]
    public void Map_CompleteVendor_ReturnsExactRegisteredVendorDetails()
    {
        VendorAggregate vendor = CreateCompleteVendor();
        var mapper = new RegisteredVendorDetailsMapper();
        int domainEventCount = vendor.DomainEvents.Count;

        RegisteredVendorDetails details = mapper.Map(vendor);

        Assert.Equal(vendor.Id, details.VendorId);
        Assert.Equal(vendor.RegisteredAt, details.RegisteredAt);
        Assert.Equal(VendorState.PendingActivation, details.VendorState);
        Assert.Equal(TradingPreference.Offline, details.TradingPreference);
        Assert.Equal(
            LegalOperatorType.LimitedCompany,
            details.LegalOperatorType);
        Assert.Equal("Mapper Operator Ltd", details.LegalOperatorName);
        Assert.Equal("SC123456", details.CompanyRegistrationNumber);
        Assert.Equal("Mapper Market Stall", details.TradingName);

        Assert.Equal(
            TradingLocation.Stall,
            details.TradingCharacteristics.TradingLocation);
        Assert.Equal(
            new TimeOnly(9, 30),
            details.TradingCharacteristics.OpeningHours.StartTime);
        Assert.Equal(
            new TimeOnly(18, 45),
            details.TradingCharacteristics.OpeningHours.EndTime);
        Assert.True(
            details.TradingCharacteristics.ServiceIncludesHotFood);
        Assert.True(details.TradingCharacteristics.AlcoholService);

        Assert.Equal("Jordan Smith", details.ContactName);
        Assert.Equal("jordan@example.test", details.ContactEmail);
        Assert.Equal("+44 20 7946 0123", details.ContactTelephone);
        Assert.Equal(
            "canonical-address-retrieval-mapper-complete",
            details.CanonicalAddressId);

        Assert.Equal(
            "Mapper Foods Ltd",
            details.BusinessAddress.RecipientOrOrganisationName);
        Assert.Equal("28 Example Street", details.BusinessAddress.AddressLine1);
        Assert.Equal("Unit 4", details.BusinessAddress.AddressLine2);
        Assert.Equal("Greenwich Market", details.BusinessAddress.AddressLine3);
        Assert.Equal("LONDON", details.BusinessAddress.PostTown);
        Assert.Equal("AB1 2CD", details.BusinessAddress.Postcode);
        Assert.Equal("Greater London", details.BusinessAddress.County);

        Assert.Equal(
            "Greenwich Borough Council",
            details.FoodRegistrationAuthority);
        Assert.Equal(
            "Greenwich Borough Council",
            details.PrimaryTradingAuthority);
        Assert.Equal(
            "https://mapper.example.test/vendor",
            details.Website);
        Assert.Equal(
            "Complete Registered Vendor Details mapping.",
            details.BusinessDescription);
        Assert.Equal(domainEventCount, vendor.DomainEvents.Count);
    }

    [Fact]
    public void Map_OptionalValuesAbsent_PreservesAbsence()
    {
        VendorAggregate vendor = CreateVendorWithoutOptionalValues();
        var mapper = new RegisteredVendorDetailsMapper();

        RegisteredVendorDetails details = mapper.Map(vendor);

        Assert.Null(details.CompanyRegistrationNumber);
        Assert.Null(details.PrimaryTradingAuthority);
        Assert.Null(details.Website);
        Assert.Null(details.BusinessDescription);
        Assert.Null(details.BusinessAddress.RecipientOrOrganisationName);
        Assert.Null(details.BusinessAddress.AddressLine2);
        Assert.Null(details.BusinessAddress.AddressLine3);
        Assert.Null(details.BusinessAddress.County);
    }

    [Fact]
    public void PublicSurface_IsImmutablePurposeSpecificAndExcludesInternalRepresentations()
    {
        Type[] representationTypes =
        [
            typeof(RegisteredVendorDetails),
            typeof(RegisteredVendorTradingCharacteristics),
            typeof(RegisteredVendorOpeningHours),
            typeof(RegisteredVendorBusinessAddress)
        ];

        Assert.All(
            representationTypes,
            type => Assert.All(
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public),
                property => Assert.True(property.SetMethod is null)));

        PropertyInfo[] detailsProperties = typeof(RegisteredVendorDetails)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);
        Assert.Equal(
            new[]
            {
                "BusinessAddress",
                "BusinessDescription",
                "CanonicalAddressId",
                "CompanyRegistrationNumber",
                "ContactEmail",
                "ContactName",
                "ContactTelephone",
                "FoodRegistrationAuthority",
                "LegalOperatorName",
                "LegalOperatorType",
                "PrimaryTradingAuthority",
                "RegisteredAt",
                "TradingCharacteristics",
                "TradingName",
                "TradingPreference",
                "VendorId",
                "VendorState",
                "Website"
            },
            detailsProperties
                .Select(property => property.Name)
                .Order(StringComparer.Ordinal)
                .ToArray());

        Type[] prohibitedDomainTypes =
        [
            typeof(VendorAggregate),
            typeof(VendorRegistrationInformation),
            typeof(BusinessAddressSnapshot),
            typeof(TradingCharacteristics),
            typeof(OpeningHours),
            typeof(PrimaryContact),
            typeof(VendorRegistered)
        ];

        Assert.All(
            representationTypes.SelectMany(type => type.GetProperties()),
            property => Assert.DoesNotContain(
                property.PropertyType,
                prohibitedDomainTypes));
        Assert.All(
            representationTypes,
            type => Assert.Equal(
                typeof(RegisteredVendorDetails).Namespace,
                type.Namespace));
        Assert.DoesNotContain(
            detailsProperties,
            property =>
                property.PropertyType.Namespace?.Contains(
                    "Infrastructure",
                    StringComparison.Ordinal) is true);
    }

    private static VendorAggregate CreateCompleteVendor()
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.LimitedCompany,
            new VendorName("Mapper Operator Ltd"),
            new VendorName("Mapper Market Stall"),
            new CompanyRegistrationNumber("SC123456"),
            new PrimaryContact(
                "Jordan Smith",
                new EmailAddress("jordan@example.test"),
                new TelephoneNumber("+44 20 7946 0123")),
            new CanonicalAddressId(
                "canonical-address-retrieval-mapper-complete"),
            new BusinessAddressSnapshot(
                "28 Example Street",
                "Unit 4",
                "Greenwich Market",
                "LONDON",
                "AB1 2CD",
                "Greater London",
                "Mapper Foods Ltd"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"),
            new TradingCharacteristics(
                TradingLocation.Stall,
                new OpeningHours(
                    new TimeOnly(9, 30),
                    new TimeOnly(18, 45)),
                serviceIncludesHotFood: true,
                alcoholService: true));

        return VendorAggregate.Register(
            new VendorId(
                Guid.Parse("b0fe1d5f-85f9-4ff9-b99c-e03b63a3ec65")),
            information,
            new Uri("https://mapper.example.test/vendor"),
            "Complete Registered Vendor Details mapping.",
            new DateTimeOffset(2026, 8, 25, 20, 0, 0, TimeSpan.Zero));
    }

    private static VendorAggregate CreateVendorWithoutOptionalValues()
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName("Optional Mapper Operator"),
            new VendorName("Optional Mapper Kitchen"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                "Alex Morgan",
                new EmailAddress("alex@example.test"),
                new TelephoneNumber("+44 20 7946 0456")),
            new CanonicalAddressId(
                "canonical-address-retrieval-mapper-optional"),
            new BusinessAddressSnapshot(
                "30 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(
                    new TimeOnly(17, 0),
                    new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(
                Guid.Parse("e8071454-e7cf-4ca1-a8f1-d1852c2e6ddd")),
            information,
            website: null,
            businessDescription: null,
            new DateTimeOffset(2026, 8, 25, 20, 30, 0, TimeSpan.Zero));
    }
}
