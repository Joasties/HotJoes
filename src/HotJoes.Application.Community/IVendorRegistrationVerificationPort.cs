namespace HotJoes.Application.Community;

public interface IVendorRegistrationVerificationPort
{
    ValueTask<VendorRegistrationVerificationResult> VerifyAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default);
}
