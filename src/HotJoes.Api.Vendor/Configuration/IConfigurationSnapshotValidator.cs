namespace HotJoes.Api.Vendor.Configuration;

public interface IConfigurationSnapshotValidator<TOptions>
    where TOptions : class
{
    bool IsValid(TOptions snapshot);
}
