namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed record CommunityParticipationRecordedMessage(Guid EventId,
    Guid CommunityParticipationId, Guid VendorId, DateTimeOffset JoinedAt,
    CommunityContactPreference ContactPreference);
