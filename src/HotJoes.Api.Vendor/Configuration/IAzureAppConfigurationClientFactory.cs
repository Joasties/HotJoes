using Azure.Core;
using Azure.Data.AppConfiguration;

namespace HotJoes.Api.Vendor.Configuration;

public interface IAzureAppConfigurationClientFactory
{
    ConfigurationClient Create(
        Uri endpoint,
        TokenCredential credential);
}
