using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegistrationSemanticFingerprintPersistenceRepresentationTests
{
    [Fact]
    public void Create_RepresentativeRegistration_ProducesExactVersionedSha256Digest()
    {
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();

        RegistrationSemanticFingerprint result =
            RegistrationSemanticFingerprint.Create(command, addressValues);

        Assert.Equal(1, result.Version);
        Assert.Equal(
            "06136046449514b1f748178ae7b2a5f2ad6ebed357a6549d15efcfc60fd351be",
            result.Sha256Digest);
        Assert.Matches("^[0-9a-f]{64}$", result.Sha256Digest);
    }

    [Fact]
    public void Create_EquivalentCanonicalValuesAndExcludedInputs_ProducesSameDigest()
    {
        RegisterVendorCommand firstCommand = CreateCommand(
            tradingName: "  HOT JOES GREENWICH  ",
            legalOperatorName: "Hot Joes Limited",
            companyRegistrationNumber: "12345678",
            addressResolutionReference: "address-reference-one",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
        RegisterVendorCommand secondCommand = CreateCommand(
            tradingName: "hot joes greenwich",
            legalOperatorName: "  HOT JOES LIMITED ",
            companyRegistrationNumber: "12345678",
            addressResolutionReference: "different-address-reference",
            authorisedToRegisterBusiness: false,
            informationAccurate: false,
            acceptHotJoesPlatformTerms: false);
        AddressAuthoritativeValues addressValues = CreateAddressValues();

        RegistrationSemanticFingerprint first =
            RegistrationSemanticFingerprint.Create(firstCommand, addressValues);
        RegistrationSemanticFingerprint second =
            RegistrationSemanticFingerprint.Create(secondCommand, addressValues);

        Assert.Equal(first, second);
        Assert.Equal(first.Version, second.Version);
        Assert.Equal(first.Sha256Digest, second.Sha256Digest);
    }

    [Fact]
    public void Create_MaterialRegistrationValueChanges_ProducesDifferentDigest()
    {
        RegisterVendorCommand firstCommand = CreateCommand(
            businessDescription: "Hot food from our Greenwich market stall.");
        RegisterVendorCommand secondCommand = CreateCommand(
            businessDescription: "A materially different business description.");
        AddressAuthoritativeValues addressValues = CreateAddressValues();

        RegistrationSemanticFingerprint first =
            RegistrationSemanticFingerprint.Create(firstCommand, addressValues);
        RegistrationSemanticFingerprint second =
            RegistrationSemanticFingerprint.Create(secondCommand, addressValues);

        Assert.NotEqual(first, second);
        Assert.NotEqual(first.Sha256Digest, second.Sha256Digest);
    }

    [Fact]
    public void PublicSurface_ExposesOnlyImmutableVersionAndDigestRepresentation()
    {
        PropertyInfo[] publicProperties = typeof(RegistrationSemanticFingerprint)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        Assert.Equal(
            new[] { "Sha256Digest", "Version" },
            publicProperties.Select(property => property.Name).Order());
        Assert.All(publicProperties, property => Assert.Null(property.SetMethod));
        Assert.Equal(typeof(string), publicProperties.Single(
            property => property.Name == "Sha256Digest").PropertyType);
        Assert.Equal(typeof(short), publicProperties.Single(
            property => property.Name == "Version").PropertyType);
    }

    private static RegisterVendorCommand CreateCommand(
        string tradingName = "Hot Joes Greenwich",
        string legalOperatorName = "Hot Joes Limited",
        string? companyRegistrationNumber = "12345678",
        string addressResolutionReference = "address-resolution-reference-001",
        string? businessDescription = "Hot food from our Greenwich market stall.",
        bool authorisedToRegisterBusiness = true,
        bool informationAccurate = true,
        bool acceptHotJoesPlatformTerms = true)
    {
        return new RegisterVendorCommand(
            tradingName,
            legalOperatorName,
            LegalOperatorType.LimitedCompany,
            companyRegistrationNumber,
            TradingLocation.Stall,
            new TimeOnly(23, 0),
            new TimeOnly(5, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Joseph Bloggs",
            "joe@hotjoes.example",
            "020 7946 0123",
            addressResolutionReference,
            "https://hotjoes.example",
            businessDescription,
            authorisedToRegisterBusiness,
            informationAccurate,
            acceptHotJoesPlatformTerms);
    }

    private static AddressAuthoritativeValues CreateAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId("canonical-address-001"),
            new BusinessAddressSnapshot(
                "2 Example Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null,
                null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
    }
}
