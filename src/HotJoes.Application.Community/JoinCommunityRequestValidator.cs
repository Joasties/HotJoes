namespace HotJoes.Application.Community;

public sealed class JoinCommunityRequestValidator
{
    public JoinCommunityRequestValidationResult Validate(
        JoinCommunityRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var errors = new List<CommunityValidationError>();

        if (request.VendorId == Guid.Empty)
        {
            errors.Add(new CommunityValidationError(
                "vendorId",
                "invalidValue",
                "Vendor ID must identify a successfully registered Vendor."));
        }

        if (!Enum.IsDefined(request.ContactPreference))
        {
            errors.Add(new CommunityValidationError(
                "contactPreference",
                "invalidValue",
                "Contact Preference must be Email, SMS or WhatsApp."));
        }

        return errors.Count == 0
            ? JoinCommunityRequestValidationResult.Accept(request)
            : JoinCommunityRequestValidationResult.Reject(errors);
    }
}
