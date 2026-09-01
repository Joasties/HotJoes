using Azure.Core;
using Azure.Data.AppConfiguration;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class AzureAppConfigurationClientFactory
    : IAzureAppConfigurationClientFactory
{
    public ConfigurationClient Create(
        Uri endpoint,
        TokenCredential credential)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentNullException.ThrowIfNull(credential);

        return new ConfigurationClient(endpoint, credential);
    }
}
