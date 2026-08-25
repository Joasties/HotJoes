using System.Text;
using HotJoes.Application.Vendor;
using HotJoes.Infrastructure.Persistence;

namespace HotJoes.IntegrationTests;

public sealed class VendorRegisteredIntegrationEventSerializationTests
{
    [Fact]
    public void Serialize_RepresentativeContract_ProducesExactDeterministicUtf8Staging()
    {
        VendorRegisteredIntegrationEvent integrationEvent = CreateIntegrationEvent(
            new VendorRegisteredBusinessAddress(
                "canonical-address-001",
                null,
                "2 High Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null),
            "Greenwich Borough Council");
        var serializer = new VendorRegisteredIntegrationEventSerializer();

        SerializedIntegrationEvent first = serializer.Serialize(integrationEvent);
        SerializedIntegrationEvent second = serializer.Serialize(integrationEvent);

        const string expectedJson =
            "{\"eventId\":\"3ac04798-ef01-4bca-b070-dde8dac9502d\","
            + "\"eventType\":\"VendorRegistered\","
            + "\"eventVersion\":1,"
            + "\"occurredAt\":\"2026-08-23T10:15:30.0000000Z\","
            + "\"payload\":{"
            + "\"vendorId\":\"f10734bd-81e4-4b07-bbdd-520e29124dd3\","
            + "\"registeredAt\":\"2026-08-23T10:15:30.0000000Z\","
            + "\"vendorState\":\"pendingActivation\","
            + "\"tradingPreference\":\"offline\","
            + "\"legalOperatorType\":\"limitedCompany\","
            + "\"tradingCharacteristics\":{"
            + "\"tradingLocation\":\"stall\","
            + "\"openingHours\":{"
            + "\"startTime\":\"09:00:00\","
            + "\"endTime\":\"17:00:00\"},"
            + "\"serviceIncludesHotFood\":true,"
            + "\"alcoholService\":false},"
            + "\"businessAddress\":{"
            + "\"canonicalAddressId\":\"canonical-address-001\","
            + "\"recipientOrOrganisationName\":null,"
            + "\"addressLine1\":\"2 High Street\","
            + "\"addressLine2\":null,"
            + "\"addressLine3\":null,"
            + "\"postTown\":\"GREENWICH\","
            + "\"postcode\":\"SE10 8AA\","
            + "\"county\":null},"
            + "\"foodRegistrationAuthority\":\"Greenwich Borough Council\","
            + "\"primaryTradingAuthority\":\"Greenwich Borough Council\"}}";
        byte[] expectedBytes = Encoding.UTF8.GetBytes(expectedJson);

        Assert.Equal(integrationEvent.EventId, first.EventId);
        Assert.Equal(integrationEvent.EventVersion, first.EventVersion);
        Assert.Equal(expectedBytes, first.SerializedEvent.ToArray());
        Assert.Equal(first.SerializedEvent.ToArray(), second.SerializedEvent.ToArray());
        Assert.False(first.SerializedEvent.Span.StartsWith(Encoding.UTF8.Preamble));
    }

    [Fact]
    public void Serialize_AbsentOptionalContractValues_WritesEveryMemberAsExplicitNull()
    {
        VendorRegisteredIntegrationEvent integrationEvent = CreateIntegrationEvent(
            new VendorRegisteredBusinessAddress(
                "canonical-address-001",
                null,
                "2 High Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null),
            null);
        var serializer = new VendorRegisteredIntegrationEventSerializer();

        SerializedIntegrationEvent result = serializer.Serialize(integrationEvent);
        string json = Encoding.UTF8.GetString(result.SerializedEvent.Span);

        Assert.Contains("\"recipientOrOrganisationName\":null", json);
        Assert.Contains("\"addressLine2\":null", json);
        Assert.Contains("\"addressLine3\":null", json);
        Assert.Contains("\"county\":null", json);
        Assert.Contains("\"primaryTradingAuthority\":null", json);
    }

    private static VendorRegisteredIntegrationEvent CreateIntegrationEvent(
        VendorRegisteredBusinessAddress businessAddress,
        string? primaryTradingAuthority)
    {
        return new VendorRegisteredIntegrationEvent(
            new Guid("3ac04798-ef01-4bca-b070-dde8dac9502d"),
            "VendorRegistered",
            1,
            new DateTimeOffset(
                2026,
                8,
                23,
                11,
                15,
                30,
                TimeSpan.FromHours(1)),
            new VendorRegisteredIntegrationEventPayload(
                new Guid("f10734bd-81e4-4b07-bbdd-520e29124dd3"),
                new DateTimeOffset(
                    2026,
                    8,
                    23,
                    12,
                    15,
                    30,
                    TimeSpan.FromHours(2)),
                "pendingActivation",
                "offline",
                "limitedCompany",
                new VendorRegisteredTradingCharacteristics(
                    "stall",
                    new VendorRegisteredOpeningHours(
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0)),
                    ServiceIncludesHotFood: true,
                    AlcoholService: false),
                businessAddress,
                "Greenwich Borough Council",
                primaryTradingAuthority));
    }
}
