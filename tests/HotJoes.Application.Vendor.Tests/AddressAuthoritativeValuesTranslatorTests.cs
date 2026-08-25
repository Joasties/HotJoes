using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class AddressAuthoritativeValuesTranslatorTests
{
    [Fact]
    public void Translate_WithRequiredAuthoritativeValues_MapsVendorOwnedValuesExactly()
    {
        var snapshot = CreateSnapshot();

        var values = AddressAuthoritativeValuesTranslator.Translate(
            canonicalAddressId: "canonical-address-123",
            snapshot: snapshot,
            foodRegistrationAuthority: "Greenwich Borough Council",
            primaryTradingAuthority: null);

        Assert.Equal(new CanonicalAddressId("canonical-address-123"), values.CanonicalAddressId);
        Assert.Equal(snapshot, values.BusinessAddressSnapshot);
        Assert.Equal(
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            values.FoodRegistrationAuthority);
        Assert.Null(values.PrimaryTradingAuthority);
    }

    [Fact]
    public void Translate_WithPrimaryTradingAuthority_MapsVendorOwnedAuthorityExactly()
    {
        var values = AddressAuthoritativeValuesTranslator.Translate(
            canonicalAddressId: "canonical-address-456",
            snapshot: CreateSnapshot(),
            foodRegistrationAuthority: "Greenwich Borough Council",
            primaryTradingAuthority: "Greenwich Borough Council");

        Assert.Equal(
            new PrimaryTradingAuthority("Greenwich Borough Council"),
            values.PrimaryTradingAuthority);
    }

    private static BusinessAddressSnapshot CreateSnapshot()
    {
        return new BusinessAddressSnapshot(
            addressLine1: "10 Example Street",
            addressLine2: "Example Village",
            addressLine3: null,
            postTown: "GREENWICH",
            postcode: "SE10 8QY",
            county: null,
            recipientOrOrganisationName: "Example Foods");
    }
}
