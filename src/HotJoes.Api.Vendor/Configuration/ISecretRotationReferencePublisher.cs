namespace HotJoes.Api.Vendor.Configuration;

public interface ISecretRotationReferencePublisher
{
    Task<bool> PublishReplacementAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);

    Task RestoreCurrentAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);
}
