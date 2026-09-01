namespace HotJoes.Api.Vendor.Configuration;

public interface ISecretRotationCredentialRevoker
{
    Task<bool> RevokeCurrentCredentialAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);
}
