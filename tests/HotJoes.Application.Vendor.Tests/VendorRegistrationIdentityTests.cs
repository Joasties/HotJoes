using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class VendorRegistrationIdentityTests
{
    [Fact]
    public void Create_TrimsNamesAndRetainsCanonicalAddressId()
    {
        var canonicalAddressId = new CanonicalAddressId("canonical-address-001");

        var identity = VendorRegistrationIdentity.Create(
            CreateCommand(
                tradingName: "  Hot Joes Greenwich  ",
                legalOperatorName: "  Hot Joes Limited  "),
            CreateAddressValues(canonicalAddressId));

        Assert.Equal("Hot Joes Greenwich", identity.NormalizedTradingName);
        Assert.Equal("Hot Joes Limited", identity.NormalizedLegalOperatorName);
        Assert.Equal(canonicalAddressId, identity.CanonicalAddressId);
    }

    [Fact]
    public void Create_WhenNamesDifferOnlyByCaseAndOuterWhitespace_ReturnsEqualValues()
    {
        var canonicalAddressId = new CanonicalAddressId("canonical-address-001");

        var first = VendorRegistrationIdentity.Create(
            CreateCommand(
                tradingName: "Hot Joes Greenwich",
                legalOperatorName: "Hot Joes Limited"),
            CreateAddressValues(canonicalAddressId));
        var second = VendorRegistrationIdentity.Create(
            CreateCommand(
                tradingName: "  HOT JOES GREENWICH ",
                legalOperatorName: " hot joes limited  "),
            CreateAddressValues(canonicalAddressId));

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Theory]
    [InlineData("Another Trading Name", "Hot Joes Limited", "canonical-address-001")]
    [InlineData("Hot Joes Greenwich", "Another Legal Operator", "canonical-address-001")]
    [InlineData("Hot Joes Greenwich", "Hot Joes Limited", "canonical-address-002")]
    public void Create_WhenAnyIdentityComponentDiffers_ReturnsUnequalValues(
        string tradingName,
        string legalOperatorName,
        string canonicalAddressId)
    {
        var baseline = VendorRegistrationIdentity.Create(
            CreateCommand(),
            CreateAddressValues(new CanonicalAddressId("canonical-address-001")));
        var candidate = VendorRegistrationIdentity.Create(
            CreateCommand(tradingName, legalOperatorName),
            CreateAddressValues(new CanonicalAddressId(canonicalAddressId)));

        Assert.NotEqual(baseline, candidate);
    }

    [Fact]
    public void Create_WhenNonIdentityRegistrationInformationDiffers_ReturnsEqualValues()
    {
        var canonicalAddressId = new CanonicalAddressId("canonical-address-001");
        var firstCommand = CreateCommand();
        var secondCommand = new RegisterVendorCommand(
            tradingName: "Hot Joes Greenwich",
            legalOperatorName: "Hot Joes Limited",
            legalOperatorType: LegalOperatorType.SoleTrader,
            companyRegistrationNumber: null,
            tradingLocation: TradingLocation.Kitchen,
            openingHoursStartTime: new TimeOnly(9, 0),
            openingHoursEndTime: new TimeOnly(17, 0),
            serviceIncludesHotFood: false,
            alcoholService: true,
            contactName: "Another Contact",
            contactEmail: "another@example.test",
            contactTelephone: "07000 000000",
            addressResolutionReference: "another-address-reference",
            website: null,
            businessDescription: "Different information.",
            authorisedToRegisterBusiness: false,
            informationAccurate: false,
            acceptHotJoesPlatformTerms: false);
        var firstAddressValues = CreateAddressValues(canonicalAddressId);
        var secondAddressValues = new AddressAuthoritativeValues(
            canonicalAddressId,
            new BusinessAddressSnapshot(
                addressLine1: "99 Different Street",
                addressLine2: "Different Locality",
                addressLine3: null,
                postTown: "LONDON",
                postcode: "SW1A 1AA",
                county: null,
                recipientOrOrganisationName: "Different Organisation"),
            new FoodRegistrationAuthority("Different Food Authority"),
            primaryTradingAuthority: null);

        var first = VendorRegistrationIdentity.Create(firstCommand, firstAddressValues);
        var second = VendorRegistrationIdentity.Create(secondCommand, secondAddressValues);

        Assert.Equal(first, second);
    }

    [Fact]
    public void PublicSurface_ContainsExactlyThreeImmutableIdentityComponents()
    {
        var expectedPropertyNames = new[]
        {
            "CanonicalAddressId",
            "NormalizedLegalOperatorName",
            "NormalizedTradingName"
        };
        var identityType = typeof(VendorRegistrationIdentity);
        var publicProperties = identityType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public);
        var createMethod = identityType.GetMethod(
            "Create",
            BindingFlags.Static | BindingFlags.Public);

        Assert.True(identityType.IsSealed);
        Assert.Equal(
            expectedPropertyNames,
            publicProperties.Select(property => property.Name).Order());
        Assert.All(publicProperties, property => Assert.Null(property.SetMethod));
        Assert.NotNull(createMethod);
        Assert.Equal(identityType, createMethod.ReturnType);
        Assert.Equal(
            new[] { typeof(RegisterVendorCommand), typeof(AddressAuthoritativeValues) },
            createMethod.GetParameters().Select(parameter => parameter.ParameterType));
        Assert.DoesNotContain(
            publicProperties,
            property => IsTransportOrInfrastructureType(property.PropertyType));
    }

    private static bool IsTransportOrInfrastructureType(Type type)
    {
        var namespaceName = type.Namespace ?? string.Empty;

        return namespaceName.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)
            || namespaceName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
            || namespaceName.StartsWith("Npgsql", StringComparison.Ordinal);
    }

    private static RegisterVendorCommand CreateCommand(
        string tradingName = "Hot Joes Greenwich",
        string legalOperatorName = "Hot Joes Limited")
    {
        return new RegisterVendorCommand(
            tradingName: tradingName,
            legalOperatorName: legalOperatorName,
            legalOperatorType: LegalOperatorType.LimitedCompany,
            companyRegistrationNumber: "12345678",
            tradingLocation: TradingLocation.Stall,
            openingHoursStartTime: new TimeOnly(23, 0),
            openingHoursEndTime: new TimeOnly(5, 0),
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
    }

    private static AddressAuthoritativeValues CreateAddressValues(
        CanonicalAddressId canonicalAddressId)
    {
        return new AddressAuthoritativeValues(
            canonicalAddressId,
            new BusinessAddressSnapshot(
                addressLine1: "2 Example Street",
                addressLine2: null,
                addressLine3: null,
                postTown: "GREENWICH",
                postcode: "SE10 8AA",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
    }
}
