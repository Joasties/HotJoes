using Azure;
using Azure.Security.KeyVault.Secrets;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class AzureKeyVaultSecretClientAdapter
    : IAzureKeyVaultSecretClient
{
    private readonly SecretClient _client;

    public AzureKeyVaultSecretClientAdapter(SecretClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
    }

    public async Task<AzureKeyVaultResolvedSecret?> GetSecretVersionAsync(
        string name,
        string version,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);

        Response<KeyVaultSecret> response = await _client.GetSecretAsync(
            name,
            version,
            cancellationToken);
        KeyVaultSecret secret = response.Value;

        return new AzureKeyVaultResolvedSecret(
            secret.Name,
            secret.Properties.Version,
            secret.Value,
            secret.Properties.Enabled == true);
    }

    public async Task<bool> DisableSecretVersionAsync(
        string name,
        string version,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);

        Response<KeyVaultSecret> currentResponse =
            await _client.GetSecretAsync(
                name,
                version,
                cancellationToken);
        SecretProperties properties = currentResponse.Value.Properties;

        if (!string.Equals(
                properties.Version,
                version,
                StringComparison.Ordinal))
        {
            return false;
        }

        properties.Enabled = false;
        Response<SecretProperties> updateResponse =
            await _client.UpdateSecretPropertiesAsync(
                properties,
                cancellationToken);

        return updateResponse.Value.Enabled == false &&
            string.Equals(
                updateResponse.Value.Version,
                version,
                StringComparison.Ordinal);
    }
}
