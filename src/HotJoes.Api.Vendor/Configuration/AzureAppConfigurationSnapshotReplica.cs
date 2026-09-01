using Azure.Data.AppConfiguration;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class AzureAppConfigurationSnapshotReplica<TOptions>
    : IConfigurationSnapshotReplica<TOptions>
    where TOptions : class
{
    private readonly string _snapshotName;
    private readonly ConfigurationClient _client;
    private readonly IConfigurationSnapshotBinder<TOptions> _binder;

    public AzureAppConfigurationSnapshotReplica(
        string name,
        string snapshotName,
        ConfigurationClient client,
        IConfigurationSnapshotBinder<TOptions> binder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshotName);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(binder);

        Name = name;
        _snapshotName = snapshotName;
        _client = client;
        _binder = binder;
    }

    public string Name { get; }

    public async Task<TOptions> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = new Dictionary<string, string?>(
            StringComparer.Ordinal);

        await foreach (ConfigurationSetting setting in
            _client.GetConfigurationSettingsForSnapshotAsync(
                    _snapshotName,
                    cancellationToken)
                .WithCancellation(cancellationToken))
        {
            settings.Add(setting.Key, setting.Value);
        }

        return _binder.Bind(settings);
    }
}
