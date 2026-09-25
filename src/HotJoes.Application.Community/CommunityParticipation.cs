namespace HotJoes.Application.Community;

public sealed class CommunityParticipation
{
    public CommunityParticipation(
        Guid communityParticipationId,
        Guid vendorId,
        ContactPreference contactPreference,
        DateTimeOffset joinedAt)
    {
        if (communityParticipationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Community Participation ID must not be empty.",
                nameof(communityParticipationId));
        }

        if (vendorId == Guid.Empty)
        {
            throw new ArgumentException(
                "Vendor ID must not be empty.",
                nameof(vendorId));
        }

        if (!Enum.IsDefined(contactPreference))
        {
            throw new ArgumentOutOfRangeException(
                nameof(contactPreference));
        }

        if (joinedAt.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException(
                "Joined At must use UTC.",
                nameof(joinedAt));
        }

        CommunityParticipationId = communityParticipationId;
        VendorId = vendorId;
        ContactPreference = contactPreference;
        JoinedAt = joinedAt;
    }

    public Guid CommunityParticipationId { get; }

    public Guid VendorId { get; }

    public ContactPreference ContactPreference { get; }

    public DateTimeOffset JoinedAt { get; }
}
