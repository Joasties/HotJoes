namespace HotJoes.Application.Vendor;

public interface IRegistrationOutcomeDeterminer
{
    Task<RegistrationOutcomeDetermination> DetermineAsync(
        VendorRegistrationIdentity identity,
        RegistrationSemanticFingerprint fingerprint,
        CancellationToken cancellationToken);
}
