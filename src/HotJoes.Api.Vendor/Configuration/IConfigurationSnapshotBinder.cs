namespace HotJoes.Api.Vendor.Configuration;

public interface IConfigurationSnapshotBinder<TOptions>
    where TOptions : class
{
    TOptions Bind(IReadOnlyDictionary<string, string?> settings);
}
