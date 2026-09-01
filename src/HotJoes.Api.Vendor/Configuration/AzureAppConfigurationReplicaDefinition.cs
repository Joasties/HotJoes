namespace HotJoes.Api.Vendor.Configuration;

public sealed record AzureAppConfigurationReplicaDefinition(
    string Name,
    Uri Endpoint);
