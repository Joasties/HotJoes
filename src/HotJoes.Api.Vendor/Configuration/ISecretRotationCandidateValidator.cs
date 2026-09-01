namespace HotJoes.Api.Vendor.Configuration;

public interface ISecretRotationCandidateValidator
{
    Task<bool> ValidateReplacementAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default);
}
