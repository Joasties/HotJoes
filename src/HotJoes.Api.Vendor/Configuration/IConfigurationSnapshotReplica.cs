namespace HotJoes.Api.Vendor.Configuration;

public interface IConfigurationSnapshotReplica<TOptions>
    where TOptions : class
{
    string Name { get; }

    Task<TOptions> LoadAsync(
        CancellationToken cancellationToken = default);
}
