using Azure.Core;
using Azure.Data.AppConfiguration;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class VendorApiAzureAppConfigurationReplicaTopology
{
    public VendorApiAzureAppConfigurationReplicaTopology(
        IEnumerable<AzureAppConfigurationReplicaDefinition> definitions,
        string snapshotName,
        TokenCredential credential,
        IConfigurationSnapshotBinder<VendorApiConfigurationSnapshot> binder,
        IAzureAppConfigurationClientFactory clientFactory)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshotName);
        ArgumentNullException.ThrowIfNull(credential);
        ArgumentNullException.ThrowIfNull(binder);
        ArgumentNullException.ThrowIfNull(clientFactory);

        AzureAppConfigurationReplicaDefinition[] definitionArray =
            definitions.ToArray();

        ValidateDefinitions(definitionArray);

        var replicas = new List<IConfigurationSnapshotReplica<
            VendorApiConfigurationSnapshot>>(definitionArray.Length);

        foreach (AzureAppConfigurationReplicaDefinition definition in
            definitionArray)
        {
            ConfigurationClient client = clientFactory.Create(
                definition.Endpoint,
                credential);

            if (client is null)
            {
                throw new InvalidOperationException(
                    $"Azure App Configuration client factory returned no " +
                    $"client for replica '{definition.Name}'.");
            }

            replicas.Add(
                new AzureAppConfigurationSnapshotReplica<
                    VendorApiConfigurationSnapshot>(
                        definition.Name,
                        snapshotName,
                        client,
                        binder));
        }

        Replicas = replicas.AsReadOnly();
    }

    public IReadOnlyList<IConfigurationSnapshotReplica<VendorApiConfigurationSnapshot>>
    Replicas
    { get; }

    private static void ValidateDefinitions(
        AzureAppConfigurationReplicaDefinition[] definitions)
    {
        if (definitions.Length < 2)
        {
            throw new ArgumentException(
                "Preferred and cross-region App Configuration replicas " +
                "are required.",
                nameof(definitions));
        }

        var names = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);
        var endpoints = new HashSet<Uri>();

        foreach (AzureAppConfigurationReplicaDefinition? definition in
            definitions)
        {
            if (definition is null)
            {
                throw new ArgumentException(
                    "An App Configuration replica definition cannot be " +
                    "null.",
                    nameof(definitions));
            }

            if (string.IsNullOrWhiteSpace(definition.Name))
            {
                throw new ArgumentException(
                    "An App Configuration replica name cannot be empty.",
                    nameof(definitions));
            }

            Uri? endpoint = definition.Endpoint;

            if (endpoint is null ||
                !endpoint.IsAbsoluteUri ||
                !string.Equals(
                    endpoint.Scheme,
                    Uri.UriSchemeHttps,
                    StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(endpoint.Host))
            {
                throw new ArgumentException(
                    $"App Configuration replica '{definition.Name}' must " +
                    $"use an absolute HTTPS endpoint with a host.",
                    nameof(definitions));
            }

            if (!names.Add(definition.Name))
            {
                throw new ArgumentException(
                    $"App Configuration replica name '{definition.Name}' " +
                    $"is duplicated.",
                    nameof(definitions));
            }

            if (!endpoints.Add(endpoint))
            {
                throw new ArgumentException(
                    $"App Configuration replica endpoint '{endpoint}' is " +
                    $"duplicated.",
                    nameof(definitions));
            }
        }
    }
}
