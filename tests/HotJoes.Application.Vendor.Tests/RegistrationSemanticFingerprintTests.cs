using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegistrationSemanticFingerprintTests
{
    [Fact]
    public void Create_WithTheSameMaterialRegistrationInformation_ReturnsEqualValues()
    {
        var command = CreateCommand();
        var addressValues = CreateAddressValues();

        var first = RegistrationSemanticFingerprint.Create(command, addressValues);
        var second = RegistrationSemanticFingerprint.Create(command, addressValues);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Create_WhenOnlyTransientDeclarationsAndAddressReferenceDiffer_ReturnsEqualValues()
    {
        var firstCommand = CreateCommand(
            addressResolutionReference: "address-resolution-reference-001",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
        var secondCommand = CreateCommand(
            addressResolutionReference: "a-different-address-resolution-reference",
            authorisedToRegisterBusiness: false,
            informationAccurate: false,
            acceptHotJoesPlatformTerms: false);
        var addressValues = CreateAddressValues();

        var first = RegistrationSemanticFingerprint.Create(firstCommand, addressValues);
        var second = RegistrationSemanticFingerprint.Create(secondCommand, addressValues);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Create_WhenMaterialClientAuthoredRegistrationInformationDiffers_ReturnsUnequalValues()
    {
        var firstCommand = CreateCommand(
            businessDescription: "Hot food from our Greenwich market stall.");
        var secondCommand = CreateCommand(
            businessDescription: "A materially different business description.");
        var addressValues = CreateAddressValues();

        var first = RegistrationSemanticFingerprint.Create(firstCommand, addressValues);
        var second = RegistrationSemanticFingerprint.Create(secondCommand, addressValues);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Create_WhenAuthoritativeAddressInformationDiffers_ReturnsUnequalValues()
    {
        var command = CreateCommand();
        var firstAddressValues = CreateAddressValues("canonical-address-001");
        var secondAddressValues = CreateAddressValues("canonical-address-002");

        var first = RegistrationSemanticFingerprint.Create(command, firstAddressValues);
        var second = RegistrationSemanticFingerprint.Create(command, secondAddressValues);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void PublicSurface_IsImmutableAndTransportIndependent()
    {
        var fingerprintType = typeof(RegistrationSemanticFingerprint);
        var publicProperties = fingerprintType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public);
        var createMethod = fingerprintType.GetMethod(
            "Create",
            BindingFlags.Static | BindingFlags.Public);

        Assert.True(fingerprintType.IsSealed);
        Assert.NotNull(createMethod);
        Assert.Equal(fingerprintType, createMethod.ReturnType);
        Assert.Equal(
            new[] { typeof(RegisterVendorCommand), typeof(AddressAuthoritativeValues) },
            createMethod.GetParameters().Select(parameter => parameter.ParameterType));
        Assert.All(publicProperties, property => Assert.Null(property.SetMethod));
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
        string addressResolutionReference = "address-resolution-reference-001",
        string? businessDescription = "Hot food from our Greenwich market stall.",
        bool authorisedToRegisterBusiness = true,
        bool informationAccurate = true,
        bool acceptHotJoesPlatformTerms = true)
    {
        return new RegisterVendorCommand(
            tradingName: "Hot Joes Greenwich",
            legalOperatorName: "Hot Joes Limited",
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
            addressResolutionReference: addressResolutionReference,
            website: "https://hotjoes.example",
            businessDescription: businessDescription,
            authorisedToRegisterBusiness: authorisedToRegisterBusiness,
            informationAccurate: informationAccurate,
            acceptHotJoesPlatformTerms: acceptHotJoesPlatformTerms);
    }

    private static AddressAuthoritativeValues CreateAddressValues(
        string canonicalAddressId = "canonical-address-001")
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId(canonicalAddressId),
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
