using HotJoes.Application.Community;

namespace HotJoes.Api.Vendor;

public sealed class JoinCommunityResponseMapper
{
    public JoinCommunityResponse Map(CommunityParticipation participation)
    {
        ArgumentNullException.ThrowIfNull(participation);

        return new JoinCommunityResponse(
            participation.CommunityParticipationId,
            participation.VendorId,
            Map(participation.ContactPreference),
            participation.JoinedAt);
    }

    private static string Map(ContactPreference preference) => preference switch
    {
        ContactPreference.Email => "email",
        ContactPreference.Sms => "sms",
        ContactPreference.WhatsApp => "whatsApp",
        _ => throw new ArgumentOutOfRangeException(nameof(preference))
    };
}
