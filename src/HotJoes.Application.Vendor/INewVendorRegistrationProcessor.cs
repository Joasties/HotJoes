namespace HotJoes.Application.Vendor;

public interface INewVendorRegistrationProcessor
{
    Task<RegisterVendorResult> ProcessAsync(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        VendorRegistrationIdentity identity,
        RegistrationSemanticFingerprint fingerprint,
        CancellationToken cancellationToken);
}
