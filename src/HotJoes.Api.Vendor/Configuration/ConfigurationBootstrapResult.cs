namespace HotJoes.Api.Vendor.Configuration;

public sealed class ConfigurationBootstrapResult<TOptions>
    where TOptions : class
{
    internal ConfigurationBootstrapResult(
        bool isReady,
        TOptions? snapshot,
        string? authoritativeReplicaName)
    {
        IsReady = isReady;
        Snapshot = snapshot;
        AuthoritativeReplicaName = authoritativeReplicaName;
    }

    public bool IsReady { get; }

    public TOptions? Snapshot { get; }

    public string? AuthoritativeReplicaName { get; }
}
