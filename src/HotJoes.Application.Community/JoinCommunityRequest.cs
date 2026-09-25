namespace HotJoes.Application.Community;

public sealed class JoinCommunityRequest
{
    public JoinCommunityRequest(
        Guid vendorId,
        ContactPreference contactPreference)
    {
        VendorId = vendorId;
        ContactPreference = contactPreference;
    }

    public Guid VendorId { get; }

    public ContactPreference ContactPreference { get; }
}
