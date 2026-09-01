namespace HotJoes.Api.Vendor.Configuration;

public interface ISecretRotationConsumerCutover
{
    Task<bool> IsStrategyVerifiedAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> CutOverAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);
}
