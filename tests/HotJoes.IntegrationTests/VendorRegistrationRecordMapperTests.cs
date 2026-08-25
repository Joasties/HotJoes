using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

public sealed class VendorRegistrationRecordMapperTests
{
    [Fact]
    public void ToRecord_CompleteRegisteredVendor_PreservesApprovedPersistenceValues()
    {
        VendorAggregate vendor = CreateVendor();

        VendorRegistrationRecord record =
            VendorRegistrationRecordMapper.ToRecord(vendor);

        Assert.Equal(vendor.Id.Value, record.VendorId);
        Assert.Equal("pendingActivation", record.VendorState);
        Assert.Equal("offline", record.TradingPreference);
        Assert.Equal(vendor.RegisteredAt, record.RegisteredAtUtc);
        Assert.Equal("limitedCompany", record.LegalOperatorType);
        Assert.Equal("  Example Operator Ltd  ", record.LegalOperatorName);
        Assert.Equal("example operator ltd", record.NormalizedLegalOperatorName);
        Assert.Equal("  Example Kitchen  ", record.TradingName);
        Assert.Equal("example kitchen", record.NormalizedTradingName);
        Assert.Equal("SC123456", record.CompanyRegistrationNumber);
        Assert.Equal("Jordan Smith", record.ContactName);
        Assert.Equal("jordan@example.test", record.ContactEmail);
        Assert.Equal("+44 20 7946 0123", record.ContactTelephone);
        Assert.Equal("address-example-id", record.CanonicalAddressId);
        Assert.Equal("Example Foods", record.RecipientOrOrganisationName);
        Assert.Equal("10 Example Street", record.AddressLine1);
        Assert.Equal("Example Village", record.AddressLine2);
        Assert.Null(record.AddressLine3);
        Assert.Equal("LONDON", record.PostTown);
        Assert.Equal("AB1 2CD", record.Postcode);
        Assert.Equal("Greater London", record.County);
        Assert.Equal("Greenwich Borough Council", record.FoodRegistrationAuthority);
        Assert.Null(record.PrimaryTradingAuthority);
        Assert.Equal("kitchen", record.TradingLocation);
        Assert.Equal(new TimeOnly(17, 0), record.OpeningHoursStart);
        Assert.Equal(new TimeOnly(2, 0), record.OpeningHoursEnd);
        Assert.True(record.ServiceIncludesHotFood);
        Assert.False(record.AlcoholService);
        Assert.Equal("https://example.test/", record.Website);
        Assert.Equal("Evening food delivery kitchen.", record.BusinessDescription);
    }

    [Fact]
    public void ToDomain_CompleteRegistrationRecord_RehydratesWholeVendorWithoutDomainEvent()
    {
        var record = new VendorRegistrationRecord
        {
            VendorId = Guid.Parse("6e81f63c-9f92-4694-8461-1a760a50528c"),
            VendorState = "pendingActivation",
            TradingPreference = "offline",
            RegisteredAtUtc = new DateTimeOffset(
                2026,
                8,
                17,
                12,
                0,
                0,
                TimeSpan.Zero),
            LegalOperatorType = "limitedCompany",
            LegalOperatorName = "Example Operator Ltd",
            NormalizedLegalOperatorName = "example operator ltd",
            TradingName = "Example Kitchen",
            NormalizedTradingName = "example kitchen",
            CompanyRegistrationNumber = "SC123456",
            ContactName = "Jordan Smith",
            ContactEmail = "jordan@example.test",
            ContactTelephone = "+44 20 7946 0123",
            CanonicalAddressId = "address-example-id",
            RecipientOrOrganisationName = "Example Foods",
            AddressLine1 = "10 Example Street",
            AddressLine2 = "Example Village",
            AddressLine3 = null,
            PostTown = "LONDON",
            Postcode = "AB1 2CD",
            County = "Greater London",
            FoodRegistrationAuthority = "Greenwich Borough Council",
            PrimaryTradingAuthority = null,
            TradingLocation = "kitchen",
            OpeningHoursStart = new TimeOnly(17, 0),
            OpeningHoursEnd = new TimeOnly(2, 0),
            ServiceIncludesHotFood = true,
            AlcoholService = false,
            Website = "https://example.test/",
            BusinessDescription = "Evening food delivery kitchen."
        };

        VendorAggregate vendor = VendorRegistrationRecordMapper.ToDomain(record);

        Assert.Equal(new VendorId(record.VendorId), vendor.Id);
        Assert.Equal(VendorState.PendingActivation, vendor.State);
        Assert.Equal(TradingPreference.Offline, vendor.TradingPreference);
        Assert.Equal(record.RegisteredAtUtc, vendor.RegisteredAt);
        Assert.Equal(
            LegalOperatorType.LimitedCompany,
            vendor.RegisteredInformation.LegalOperatorType);
        Assert.Equal(
            record.LegalOperatorName,
            vendor.RegisteredInformation.LegalOperatorName.Value);
        Assert.Equal(
            record.TradingName,
            vendor.RegisteredInformation.TradingName.Value);
        Assert.Equal(
            record.CompanyRegistrationNumber,
            vendor.RegisteredInformation.CompanyRegistrationNumber?.Value);
        Assert.Equal(
            record.ContactName,
            vendor.RegisteredInformation.PrimaryContact.ContactName);
        Assert.Equal(
            record.ContactEmail,
            vendor.RegisteredInformation.PrimaryContact.EmailAddress.Value);
        Assert.Equal(
            record.ContactTelephone,
            vendor.RegisteredInformation.PrimaryContact.TelephoneNumber.Value);
        Assert.Equal(
            record.CanonicalAddressId,
            vendor.RegisteredInformation.CanonicalAddressId.Value);
        Assert.Equal(
            record.RecipientOrOrganisationName,
            vendor.RegisteredInformation.BusinessAddressSnapshot
                .RecipientOrOrganisationName);
        Assert.Equal(
            record.AddressLine1,
            vendor.RegisteredInformation.BusinessAddressSnapshot.AddressLine1);
        Assert.Equal(
            record.AddressLine2,
            vendor.RegisteredInformation.BusinessAddressSnapshot.AddressLine2);
        Assert.Null(
            vendor.RegisteredInformation.BusinessAddressSnapshot.AddressLine3);
        Assert.Equal(
            record.PostTown,
            vendor.RegisteredInformation.BusinessAddressSnapshot.PostTown);
        Assert.Equal(
            record.Postcode,
            vendor.RegisteredInformation.BusinessAddressSnapshot.Postcode);
        Assert.Equal(
            record.County,
            vendor.RegisteredInformation.BusinessAddressSnapshot.County);
        Assert.Equal(
            record.FoodRegistrationAuthority,
            vendor.RegisteredInformation.FoodRegistrationAuthority.Value);
        Assert.Null(vendor.RegisteredInformation.PrimaryTradingAuthority);
        Assert.Equal(
            TradingLocation.Kitchen,
            vendor.RegisteredInformation.TradingCharacteristics.TradingLocation);
        Assert.Equal(
            record.OpeningHoursStart,
            vendor.RegisteredInformation.TradingCharacteristics.OpeningHours.StartTime);
        Assert.Equal(
            record.OpeningHoursEnd,
            vendor.RegisteredInformation.TradingCharacteristics.OpeningHours.EndTime);
        Assert.True(
            vendor.RegisteredInformation.TradingCharacteristics.ServiceIncludesHotFood);
        Assert.False(
            vendor.RegisteredInformation.TradingCharacteristics.AlcoholService);
        Assert.Equal(new Uri(record.Website), vendor.Website);
        Assert.Equal(record.BusinessDescription, vendor.BusinessDescription);
        Assert.Empty(vendor.DomainEvents);
    }

    [Fact]
    public void Vendor_PublicSurface_ExposesNoPersistenceRehydrationOperation()
    {
        Assert.DoesNotContain(
            typeof(VendorAggregate).GetMethods(),
            method => method.Name.Contains(
                "Rehydrat",
                StringComparison.OrdinalIgnoreCase));
    }

    private static VendorAggregate CreateVendor()
    {
        var registrationInformation = new VendorRegistrationInformation(
            LegalOperatorType.LimitedCompany,
            new VendorName("  Example Operator Ltd  "),
            new VendorName("  Example Kitchen  "),
            new CompanyRegistrationNumber("SC123456"),
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
                "Greater London",
                "Example Foods"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(new TimeOnly(17, 0), new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(Guid.Parse("6e81f63c-9f92-4694-8461-1a760a50528c")),
            registrationInformation,
            new Uri("https://example.test"),
            "Evening food delivery kitchen.",
            new DateTimeOffset(2026, 8, 17, 12, 0, 0, TimeSpan.Zero));
    }
}
