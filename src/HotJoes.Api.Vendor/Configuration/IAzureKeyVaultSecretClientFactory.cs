using Azure.Core;

namespace HotJoes.Api.Vendor.Configuration;

public interface IAzureKeyVaultSecretClientFactory
{
    IAzureKeyVaultSecretClient Create(
        Uri vaultUri,
        TokenCredential credential);
}
