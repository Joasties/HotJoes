using HotJoes.Application.Community;

namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityParticipationRecordedIntegrationEventMapper
{
    public CommunityParticipationRecordedIntegrationEvent Map(
        CommunityParticipation participation,
        Guid eventId,
        DateTimeOffset occurredAt)
    {
        ArgumentNullException.ThrowIfNull(participation);

        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event ID must not be empty.",
                nameof(eventId));
        }

        if (occurredAt.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException(
                "Occurred At must use UTC.",
                nameof(occurredAt));
        }

        return new CommunityParticipationRecordedIntegrationEvent(
            eventId,
            "CommunityParticipationRecorded",
            1,
            occurredAt,
            new CommunityParticipationRecordedIntegrationEventPayload(
                participation.CommunityParticipationId,
                participation.VendorId,
                participation.JoinedAt,
                Map(participation.ContactPreference)));
    }

    private static CommunityIntegrationEventContactPreference Map(
        ContactPreference preference)
    {
        return preference switch
        {
            ContactPreference.Email =>
                CommunityIntegrationEventContactPreference.Email,
            ContactPreference.Sms =>
                CommunityIntegrationEventContactPreference.Sms,
            ContactPreference.WhatsApp =>
                CommunityIntegrationEventContactPreference.WhatsApp,
            _ => throw new ArgumentOutOfRangeException(nameof(preference))
        };
    }
}
