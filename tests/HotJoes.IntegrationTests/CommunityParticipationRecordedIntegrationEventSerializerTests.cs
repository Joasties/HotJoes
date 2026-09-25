using System.Text.Json;
using HotJoes.Application.Community;
using HotJoes.Infrastructure.Community.Persistence;

namespace HotJoes.IntegrationTests;

public sealed class CommunityParticipationRecordedIntegrationEventSerializerTests
{
    private static readonly Guid ParticipationId =
        Guid.Parse("3a1db978-b3b6-44dd-ad45-564fba763ecc");
    private static readonly Guid VendorId =
        Guid.Parse("8b70dd10-f0ca-4af8-89c4-5d61ef89ea88");
    private static readonly Guid EventId =
        Guid.Parse("31fe1d28-4984-4d6d-95c0-46079db963ad");
    private static readonly DateTimeOffset JoinedAt =
        new(2026, 9, 22, 10, 11, 12, TimeSpan.Zero);

    [Theory]
    [InlineData(ContactPreference.Email, "email")]
    [InlineData(ContactPreference.Sms, "sms")]
    [InlineData(ContactPreference.WhatsApp, "whatsApp")]
    public void Serialize_ApprovedEvent_UsesExactVersionOneContract(
        ContactPreference preference,
        string expectedPreference)
    {
        CommunityParticipation participation = new(
            ParticipationId,
            VendorId,
            preference,
            JoinedAt);
        CommunityParticipationRecordedIntegrationEvent integrationEvent =
            new CommunityParticipationRecordedIntegrationEventMapper().Map(
                participation,
                EventId,
                JoinedAt);

        SerializedCommunityIntegrationEvent serialized =
            new CommunityParticipationRecordedIntegrationEventSerializer()
                .Serialize(integrationEvent);

        Assert.Equal(EventId, serialized.EventId);
        Assert.Equal(1, serialized.EventVersion);

        using JsonDocument document = JsonDocument.Parse(
            serialized.SerializedEvent);
        JsonElement root = document.RootElement;

        Assert.Equal(
            ["eventId", "eventType", "eventVersion", "occurredAt", "payload"],
            root.EnumerateObject().Select(property => property.Name));
        Assert.Equal(EventId.ToString("D"), root.GetProperty("eventId").GetString());
        Assert.Equal(
            "CommunityParticipationRecorded",
            root.GetProperty("eventType").GetString());
        Assert.Equal(1, root.GetProperty("eventVersion").GetInt32());
        Assert.Equal(
            "2026-09-22T10:11:12.0000000Z",
            root.GetProperty("occurredAt").GetString());

        JsonElement payload = root.GetProperty("payload");
        Assert.Equal(
            [
                "communityParticipationId",
                "vendorId",
                "joinedAt",
                "contactPreference"
            ],
            payload.EnumerateObject().Select(property => property.Name));
        Assert.Equal(
            ParticipationId.ToString("D"),
            payload.GetProperty("communityParticipationId").GetString());
        Assert.Equal(
            VendorId.ToString("D"),
            payload.GetProperty("vendorId").GetString());
        Assert.Equal(
            "2026-09-22T10:11:12.0000000Z",
            payload.GetProperty("joinedAt").GetString());
        Assert.Equal(
            expectedPreference,
            payload.GetProperty("contactPreference").GetString());

        string json = JsonSerializer.Serialize(root);
        Assert.DoesNotContain("primaryContact", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("emailAddress", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("telephone", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("consent", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("recipient", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("delivery", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("provider", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Serialize_SameEventTwice_ProducesIdenticalBytes()
    {
        CommunityParticipation participation = new(
            ParticipationId,
            VendorId,
            ContactPreference.Email,
            JoinedAt);
        CommunityParticipationRecordedIntegrationEvent integrationEvent =
            new CommunityParticipationRecordedIntegrationEventMapper().Map(
                participation,
                EventId,
                JoinedAt);
        var serializer =
            new CommunityParticipationRecordedIntegrationEventSerializer();

        byte[] first = serializer.Serialize(integrationEvent)
            .SerializedEvent.ToArray();
        byte[] second = serializer.Serialize(integrationEvent)
            .SerializedEvent.ToArray();

        Assert.Equal(first, second);
    }
}
