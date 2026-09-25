namespace HotJoes.Api.Vendor;

public sealed record JoinCommunityResponse(
    Guid CommunityParticipationId,
    Guid VendorId,
    string ContactPreference,
    DateTimeOffset JoinedAt);
