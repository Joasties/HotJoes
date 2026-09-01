namespace HotJoes.Api.Vendor.Configuration;

public interface IConfigurationSnapshotActivator<TOptions>
    where TOptions : class
{
    Task ActivateAsync(
        TOptions snapshot,
        CancellationToken cancellationToken = default);
}
