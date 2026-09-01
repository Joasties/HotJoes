namespace HotJoes.Api.Vendor.Configuration;

public sealed class AzureKeyVaultResolvedSecret
{
    public AzureKeyVaultResolvedSecret(
        string name,
        string version,
        string value,
        bool isEnabled)
    {
        Name = name;
        Version = version;
        Value = value;
        IsEnabled = isEnabled;
    }

    public string Name { get; }

    public string Version { get; }

    public string Value { get; }

    public bool IsEnabled { get; }
}
