using HotJoes.Application.Community;
using ApplicationRequest = HotJoes.Application.Community.JoinCommunityRequest;

namespace HotJoes.Api.Vendor;

public sealed class JoinCommunityRequestMapper
{
    public ApplicationRequest Map(JoinCommunityRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Guid vendorId = request.VendorId
            ?? throw new ArgumentException(
                "Vendor ID must be structurally valid before mapping.",
                nameof(request));

        ContactPreference preference = request.ContactPreference switch
        {
            "email" => ContactPreference.Email,
            "sms" => ContactPreference.Sms,
            "whatsApp" => ContactPreference.WhatsApp,
            _ => throw new ArgumentException(
                "Contact Preference must be structurally valid before mapping.",
                nameof(request))
        };

        return new ApplicationRequest(vendorId, preference);
    }
}
