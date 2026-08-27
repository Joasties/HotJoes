using System.Text.Json;
using HotJoes.Api.Vendor;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class RegisteredVendorDetailsResponseContractTests
{
    [Fact]
    public void Serialize_RepresentativeDetails_UsesApprovedNestedDeterministicRepresentation()
    {
        RegisteredVendorDetailsResponse response =
            new RegisteredVendorDetailsResponseMapper().Map(CreateDetails());

        string json = JsonSerializer.Serialize(
            response,
            VendorApiJsonOptions.Create());

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        Assert.Equal(
            "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            root.GetProperty("vendorId").GetString());
        Assert.Equal(
            "2026-08-25T10:30:00.0000000Z",
            root.GetProperty("registeredAt").GetString());
        Assert.Equal("pendingActivation", root.GetProperty("vendorState").GetString());
        Assert.Equal("offline", root.GetProperty("tradingPreference").GetString());
        Assert.Equal("limitedCompany", root.GetProperty("legalOperatorType").GetString());

        JsonElement trading = root.GetProperty("tradingCharacteristics");
        Assert.Equal("kitchen", trading.GetProperty("tradingLocation").GetString());
        Assert.Equal(
            "17:00:00",
            trading.GetProperty("openingHours").GetProperty("startTime").GetString());
        Assert.Equal(
            "02:00:00",
            trading.GetProperty("openingHours").GetProperty("endTime").GetString());

        JsonElement address = root.GetProperty("businessAddressSnapshot");
        Assert.Equal("10 Example Street", address.GetProperty("addressLine1").GetString());
        Assert.Equal("Example Village", address.GetProperty("addressLine2").GetString());
        Assert.Equal("LONDON", address.GetProperty("postTown").GetString());
        Assert.Equal("AB1 2CD", address.GetProperty("postcode").GetString());
    }

    [Fact]
    public void Serialize_AbsentOptionals_EmitsEveryOptionalMemberAsExplicitNull()
    {
        RegisteredVendorDetailsResponse response =
            new RegisteredVendorDetailsResponseMapper().Map(CreateDetails());

        string json = JsonSerializer.Serialize(
            response,
            VendorApiJsonOptions.Create());

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;
        JsonElement address = root.GetProperty("businessAddressSnapshot");

        Assert.Equal(JsonValueKind.Null, root.GetProperty("companyRegistrationNumber").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("primaryTradingAuthority").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("website").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("businessDescription").ValueKind);
        Assert.Equal(JsonValueKind.Null, address.GetProperty("addressLine3").ValueKind);
        Assert.Equal(JsonValueKind.Null, address.GetProperty("county").ValueKind);
        Assert.Equal(
            JsonValueKind.Null,
            address.GetProperty("recipientOrOrganisationName").ValueKind);
    }

    private static RegisteredVendorDetails CreateDetails()
    {
        return new RegisteredVendorDetails(
            new VendorId(Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE")),
            new DateTimeOffset(2026, 8, 25, 10, 30, 0, TimeSpan.Zero),
            VendorState.PendingActivation,
            TradingPreference.Offline,
            LegalOperatorType.LimitedCompany,
            "Hot Joe's Foods Limited",
            null,
            "Hot Joe's Kitchen",
            new RegisteredVendorTradingCharacteristics(
                TradingLocation.Kitchen,
                new RegisteredVendorOpeningHours(
                    new TimeOnly(17, 0),
                    new TimeOnly(2, 0)),
                true,
                false),
            "Jordan Smith",
            "jordan@example.test",
            "+442079460123",
            "address-example-id",
            new RegisteredVendorBusinessAddress(
                "10 Example Street",
                "Example Village",
                null,
                "LONDON",
                "AB1 2CD",
                null,
                null),
            "Example authority",
            null,
            null,
            null);
    }
}
