namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class CommunityParticipationRecord
{
    public Guid CommunityParticipationId { get; set; }

    public Guid VendorId { get; set; }

    public string ContactPreference { get; set; } = null!;

    public DateTimeOffset JoinedAtUtc { get; set; }
}
