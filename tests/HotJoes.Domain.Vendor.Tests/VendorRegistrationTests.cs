using HotJoes.Domain.Vendor;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.Domain.Vendor.Tests;

public sealed class VendorRegistrationTests
{
    [Fact]
    public void Register_WithCompleteValidAuthoritativeInformation_CreatesPendingActivationVendor()
    {
        var vendorId = new VendorId(Guid.Parse("6e81f63c-9f92-4694-8461-1a760a50528c"));
        var registeredAt = new DateTimeOffset(2026, 8, 17, 12, 0, 0, TimeSpan.Zero);
        var registrationInformation = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName("Jordan Smith"),
            new VendorName("Jordan's Evening Kitchen"),
            companyRegistrationNumber: null,
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
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        var vendor = VendorAggregate.Register(
            vendorId,
            registrationInformation,
            website: new Uri("https://example.test"),
            businessDescription: "Evening food delivery kitchen.",
            registeredAt);

        Assert.Equal(vendorId, vendor.Id);
        Assert.Equal(VendorState.PendingActivation, vendor.State);
        Assert.Equal(TradingPreference.Offline, vendor.TradingPreference);
        Assert.Equal(registeredAt, vendor.RegisteredAt);
        Assert.Equal(registrationInformation, vendor.RegisteredInformation);
        Assert.Equal(new Uri("https://example.test"), vendor.Website);
        Assert.Equal("Evening food delivery kitchen.", vendor.BusinessDescription);
        Assert.Collection(vendor.DomainEvents, domainEvent => Assert.IsType<VendorRegistered>(domainEvent));
    }
}
