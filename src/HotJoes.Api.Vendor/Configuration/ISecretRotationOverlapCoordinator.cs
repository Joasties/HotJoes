namespace HotJoes.Api.Vendor.Configuration;

public interface ISecretRotationOverlapCoordinator
{
    Task<bool> EstablishOverlapAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);
}
