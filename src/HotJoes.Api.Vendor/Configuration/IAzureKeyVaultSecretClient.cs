namespace HotJoes.Api.Vendor.Configuration;

public interface IAzureKeyVaultSecretClient
{
    Task<AzureKeyVaultResolvedSecret?> GetSecretVersionAsync(
        string name,
        string version,
        CancellationToken cancellationToken = default);

    Task<bool> DisableSecretVersionAsync(
        string name,
        string version,
        CancellationToken cancellationToken = default);
}
