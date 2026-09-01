namespace HotJoes.Api.Vendor.Configuration;

public interface ISecretRotationHealthGate
{
    Task<bool> ConfirmHealthyDrainedAndDurableAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);
}
