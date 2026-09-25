namespace HotJoes.Application.Community;

public abstract class VendorRegistrationVerificationResult
{
    private VendorRegistrationVerificationResult()
    {
    }

    public static VendorRegistrationVerificationResult Registered() =>
        new SuccessfullyRegistered();

    public static VendorRegistrationVerificationResult NotFound() =>
        new VendorNotFound();

    public static VendorRegistrationVerificationResult
        TemporarilyUnavailable() => new VerificationUnavailable();

    public sealed class SuccessfullyRegistered
        : VendorRegistrationVerificationResult
    {
        internal SuccessfullyRegistered()
        {
        }
    }

    public sealed class VendorNotFound : VendorRegistrationVerificationResult
    {
        internal VendorNotFound()
        {
        }
    }

    public sealed class VerificationUnavailable
        : VendorRegistrationVerificationResult
    {
        internal VerificationUnavailable()
        {
        }
    }
}
