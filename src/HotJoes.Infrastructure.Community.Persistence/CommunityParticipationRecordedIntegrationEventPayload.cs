namespace HotJoes.Infrastructure.Community.Persistence;

public sealed record CommunityParticipationRecordedIntegrationEventPayload(
    Guid CommunityParticipationId,
    Guid VendorId,
    DateTimeOffset JoinedAt,
    CommunityIntegrationEventContactPreference ContactPreference);
