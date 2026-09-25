namespace HotJoes.Infrastructure.Community.Persistence;

public sealed record CommunityParticipationRecordedIntegrationEvent(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredAt,
    CommunityParticipationRecordedIntegrationEventPayload Payload);
