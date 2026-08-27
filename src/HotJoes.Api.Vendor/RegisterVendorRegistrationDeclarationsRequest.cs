namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorRegistrationDeclarationsRequest
{
    public bool? AuthorisedToRegisterBusiness { get; init; }

    public bool? InformationAccurate { get; init; }

    public bool? AcceptHotJoesPlatformTerms { get; init; }
}
