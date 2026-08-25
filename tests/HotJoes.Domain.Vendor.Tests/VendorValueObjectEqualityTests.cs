using HotJoes.Domain.Vendor;

namespace HotJoes.Domain.Vendor.Tests;

public sealed class VendorValueObjectEqualityTests
{
    [Theory]
    [InlineData("VendorId")]
    [InlineData("TradingCharacteristics")]
    [InlineData("OpeningHours")]
    [InlineData("VendorName")]
    [InlineData("CompanyRegistrationNumber")]
    [InlineData("PrimaryContact")]
    [InlineData("EmailAddress")]
    [InlineData("TelephoneNumber")]
    [InlineData("CanonicalAddressId")]
    [InlineData("BusinessAddressSnapshot.AddressLine1")]
    [InlineData("BusinessAddressSnapshot.AddressLine2")]
    [InlineData("BusinessAddressSnapshot.AddressLine3")]
    [InlineData("BusinessAddressSnapshot.PostTown")]
    [InlineData("BusinessAddressSnapshot.Postcode")]
    [InlineData("BusinessAddressSnapshot.County")]
    [InlineData("BusinessAddressSnapshot.RecipientOrOrganisationName")]
    public void Equality_WithEqualAndDifferentDefiningValues_UsesValueSemantics(
        string valueObjectCase)
    {
        var (first, equal, different) = CreateEqualityCase(valueObjectCase);

        Assert.Equal(first, equal);
        Assert.True(first.Equals(equal));
        Assert.Equal(first.GetHashCode(), equal.GetHashCode());
        Assert.NotEqual(first, different);
        Assert.False(first.Equals(different));
    }

    private static (object First, object Equal, object Different) CreateEqualityCase(
        string valueObjectCase)
    {
        return valueObjectCase switch
        {
            "VendorId" => (
                new VendorId(Guid.Parse("6e81f63c-9f92-4694-8461-1a760a50528c")),
                new VendorId(Guid.Parse("6e81f63c-9f92-4694-8461-1a760a50528c")),
                new VendorId(Guid.Parse("a1726526-1d7c-4a44-b943-416e24ef6be7"))),
            "TradingCharacteristics" => (
                CreateTradingCharacteristics(TradingLocation.Kitchen, true, false),
                CreateTradingCharacteristics(TradingLocation.Kitchen, true, false),
                CreateTradingCharacteristics(TradingLocation.Restaurant, true, false)),
            "OpeningHours" => (
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                new OpeningHours(new TimeOnly(18, 0), new TimeOnly(2, 0))),
            "VendorName" => (
                new VendorName("Jordan's Evening Kitchen"),
                new VendorName("Jordan's Evening Kitchen"),
                new VendorName("Jordan's Morning Kitchen")),
            "CompanyRegistrationNumber" => (
                new CompanyRegistrationNumber("ab123456"),
                new CompanyRegistrationNumber("AB123456"),
                new CompanyRegistrationNumber("AB123457")),
            "PrimaryContact" => (
                CreatePrimaryContact("jordan@example.test"),
                CreatePrimaryContact("jordan@example.test"),
                CreatePrimaryContact("alex@example.test")),
            "EmailAddress" => (
                new EmailAddress("jordan@example.test"),
                new EmailAddress("jordan@example.test"),
                new EmailAddress("alex@example.test")),
            "TelephoneNumber" => (
                new TelephoneNumber("+44 20 7946 0123"),
                new TelephoneNumber("+44 20 7946 0123"),
                new TelephoneNumber("+44 20 7946 0456")),
            "CanonicalAddressId" => (
                new CanonicalAddressId("address-example-id"),
                new CanonicalAddressId("address-example-id"),
                new CanonicalAddressId("another-address-id")),
            "BusinessAddressSnapshot.AddressLine1" => CreateAddressCase(
                addressLine1: "11 Example Street"),
            "BusinessAddressSnapshot.AddressLine2" => CreateAddressCase(
                addressLine2: "Different Village"),
            "BusinessAddressSnapshot.AddressLine3" => CreateAddressCase(
                addressLine3: "Different District"),
            "BusinessAddressSnapshot.PostTown" => CreateAddressCase(
                postTown: "BRISTOL"),
            "BusinessAddressSnapshot.Postcode" => CreateAddressCase(
                postcode: "CD3 4EF"),
            "BusinessAddressSnapshot.County" => CreateAddressCase(
                county: "Kent"),
            "BusinessAddressSnapshot.RecipientOrOrganisationName" => CreateAddressCase(
                recipientOrOrganisationName: "Another Organisation"),
            _ => throw new ArgumentOutOfRangeException(nameof(valueObjectCase))
        };
    }

    private static TradingCharacteristics CreateTradingCharacteristics(
        TradingLocation tradingLocation,
        bool serviceIncludesHotFood,
        bool alcoholService)
    {
        return new TradingCharacteristics(
            tradingLocation,
            new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
            serviceIncludesHotFood,
            alcoholService);
    }

    private static PrimaryContact CreatePrimaryContact(string emailAddress)
    {
        return new PrimaryContact(
            "Jordan Smith",
            new EmailAddress(emailAddress),
            new TelephoneNumber("+44 20 7946 0123"));
    }

    private static (object First, object Equal, object Different) CreateAddressCase(
        string addressLine1 = "10 Example Street",
        string addressLine2 = "Example Village",
        string addressLine3 = "Example District",
        string postTown = "LONDON",
        string postcode = "AB1 2CD",
        string county = "Greater London",
        string recipientOrOrganisationName = "Example Foods")
    {
        var first = CreateBusinessAddressSnapshot();
        var equal = CreateBusinessAddressSnapshot();
        var different = new BusinessAddressSnapshot(
            addressLine1,
            addressLine2,
            addressLine3,
            postTown,
            postcode,
            county,
            recipientOrOrganisationName);

        return (first, equal, different);
    }

    private static BusinessAddressSnapshot CreateBusinessAddressSnapshot()
    {
        return new BusinessAddressSnapshot(
            "10 Example Street",
            "Example Village",
            "Example District",
            "LONDON",
            "AB1 2CD",
            "Greater London",
            "Example Foods");
    }

}
