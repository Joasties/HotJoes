using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class VendorRegisteredIntegrationEventMapperTests
{
    [Fact]
    public void Map_CompletedRegistration_MapsExactContractOwnedRepresentation()
    {
        var eventId = new Guid("3ac04798-ef01-4bca-b070-dde8dac9502d");
        var occurredAt = new DateTimeOffset(
            2026,
            8,
            23,
            10,
            15,
            30,
            TimeSpan.FromHours(1));
        HotJoes.Domain.Vendor.Vendor vendor = CreateVendor(
            TradingLocation.Stall,
            new BusinessAddressSnapshot(
                "2 High Street",
                "Greenwich Market",
                "Unit 4",
                "GREENWICH",
                "SE10 8AA",
                "Greater London",
                "Hot Joes Limited"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
        var completedFact = Assert.IsType<VendorRegistered>(
            Assert.Single(vendor.DomainEvents));
        var mapper = new VendorRegisteredIntegrationEventMapper();

        VendorRegisteredIntegrationEvent result = mapper.Map(
            completedFact,
            vendor,
            eventId,
            occurredAt);

        Assert.Equal(eventId, result.EventId);
        Assert.Equal("VendorRegistered", result.EventType);
        Assert.Equal(1, result.EventVersion);
        Assert.Equal(occurredAt, result.OccurredAt);

        Assert.Equal(vendor.Id.Value, result.Payload.VendorId);
        Assert.Equal(vendor.RegisteredAt, result.Payload.RegisteredAt);
        Assert.Equal("pendingActivation", result.Payload.VendorState);
        Assert.Equal("offline", result.Payload.TradingPreference);
        Assert.Equal("limitedCompany", result.Payload.LegalOperatorType);

        Assert.Equal("stall", result.Payload.TradingCharacteristics.TradingLocation);
        Assert.Equal(
            new TimeOnly(9, 0),
            result.Payload.TradingCharacteristics.OpeningHours.StartTime);
        Assert.Equal(
            new TimeOnly(17, 0),
            result.Payload.TradingCharacteristics.OpeningHours.EndTime);
        Assert.True(result.Payload.TradingCharacteristics.ServiceIncludesHotFood);
        Assert.False(result.Payload.TradingCharacteristics.AlcoholService);

        Assert.Equal(
            "canonical-address-001",
            result.Payload.BusinessAddress.CanonicalAddressId);
        Assert.Equal(
            "Hot Joes Limited",
            result.Payload.BusinessAddress.RecipientOrOrganisationName);
        Assert.Equal("2 High Street", result.Payload.BusinessAddress.AddressLine1);
        Assert.Equal("Greenwich Market", result.Payload.BusinessAddress.AddressLine2);
        Assert.Equal("Unit 4", result.Payload.BusinessAddress.AddressLine3);
        Assert.Equal("GREENWICH", result.Payload.BusinessAddress.PostTown);
        Assert.Equal("SE10 8AA", result.Payload.BusinessAddress.Postcode);
        Assert.Equal("Greater London", result.Payload.BusinessAddress.County);
        Assert.Equal(
            "Greenwich Borough Council",
            result.Payload.FoodRegistrationAuthority);
        Assert.Equal(
            "Greenwich Borough Council",
            result.Payload.PrimaryTradingAuthority);
    }

    [Fact]
    public void Map_AbsentOptionalValues_PreservesTheirAbsenceInTheContract()
    {
        HotJoes.Domain.Vendor.Vendor vendor = CreateVendor(
            TradingLocation.Kitchen,
            new BusinessAddressSnapshot(
                "2 High Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null,
                null),
            null);
        var completedFact = Assert.IsType<VendorRegistered>(
            Assert.Single(vendor.DomainEvents));
        var mapper = new VendorRegisteredIntegrationEventMapper();

        VendorRegisteredIntegrationEvent result = mapper.Map(
            completedFact,
            vendor,
            new Guid("ca66930f-dd11-49d5-832e-919181462388"),
            new DateTimeOffset(2026, 8, 23, 10, 15, 30, TimeSpan.Zero));

        Assert.Equal("kitchen", result.Payload.TradingCharacteristics.TradingLocation);
        Assert.Null(result.Payload.BusinessAddress.RecipientOrOrganisationName);
        Assert.Null(result.Payload.BusinessAddress.AddressLine2);
        Assert.Null(result.Payload.BusinessAddress.AddressLine3);
        Assert.Null(result.Payload.BusinessAddress.County);
        Assert.Null(result.Payload.PrimaryTradingAuthority);
    }

    private static HotJoes.Domain.Vendor.Vendor CreateVendor(
        TradingLocation tradingLocation,
        BusinessAddressSnapshot businessAddress,
        PrimaryTradingAuthority? primaryTradingAuthority)
    {
        var registeredAt = new DateTimeOffset(
            2026,
            8,
            23,
            10,
            15,
            29,
            TimeSpan.FromHours(1));
        var information = new VendorRegistrationInformation(
            LegalOperatorType.LimitedCompany,
            new VendorName("Hot Joes Limited"),
            new VendorName("Hot Joes Greenwich"),
            new CompanyRegistrationNumber("12345678"),
            new PrimaryContact(
                "Joseph Bloggs",
                new EmailAddress("joe@hotjoes.example"),
                new TelephoneNumber("020 7946 0123")),
            new CanonicalAddressId("canonical-address-001"),
            businessAddress,
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority,
            new TradingCharacteristics(
                tradingLocation,
                new OpeningHours(new TimeOnly(9, 0), new TimeOnly(17, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return HotJoes.Domain.Vendor.Vendor.Register(
            new VendorId(new Guid("f10734bd-81e4-4b07-bbdd-520e29124dd3")),
            information,
            new Uri("https://hotjoes.example"),
            "Hot food from our Greenwich trading location.",
            registeredAt);
    }
}
