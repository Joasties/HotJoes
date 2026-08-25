namespace HotJoes.Application.Vendor;

public sealed record NewVendorRegistrationCommit
{
    public NewVendorRegistrationCommit(
        HotJoes.Domain.Vendor.Vendor vendor,
        VendorRegistrationIdentity identity,
        RegistrationSemanticFingerprint fingerprint,
        RegisterVendorResult.Success originalResult,
        VendorRegisteredIntegrationEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(vendor);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(fingerprint);
        ArgumentNullException.ThrowIfNull(originalResult);
        ArgumentNullException.ThrowIfNull(integrationEvent);

        Vendor = vendor;
        Identity = identity;
        Fingerprint = fingerprint;
        OriginalResult = originalResult;
        IntegrationEvent = integrationEvent;
    }

    public HotJoes.Domain.Vendor.Vendor Vendor { get; }

    public VendorRegistrationIdentity Identity { get; }

    public RegistrationSemanticFingerprint Fingerprint { get; }

    public RegisterVendorResult.Success OriginalResult { get; }

    public VendorRegisteredIntegrationEvent IntegrationEvent { get; }
}
