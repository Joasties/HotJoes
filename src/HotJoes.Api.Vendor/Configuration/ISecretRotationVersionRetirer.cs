namespace HotJoes.Api.Vendor.Configuration;

public interface ISecretRotationVersionRetirer
{
    Task<bool> RetireCurrentVersionAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);
}
