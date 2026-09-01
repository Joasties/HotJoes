using Azure.Core;
using Azure.Security.KeyVault.Secrets;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class AzureKeyVaultSecretClientFactory
    : IAzureKeyVaultSecretClientFactory
{
    public IAzureKeyVaultSecretClient Create(
        Uri vaultUri,
        TokenCredential credential)
    {
        ArgumentNullException.ThrowIfNull(vaultUri);
        ArgumentNullException.ThrowIfNull(credential);

        if (!vaultUri.IsAbsoluteUri ||
            !string.Equals(
                vaultUri.Scheme,
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(vaultUri.Host))
        {
            throw new ArgumentException(
                "Key Vault URI must be an absolute HTTPS URI with a host.",
                nameof(vaultUri));
        }

        return new AzureKeyVaultSecretClientAdapter(
            new SecretClient(vaultUri, credential));
    }
}
