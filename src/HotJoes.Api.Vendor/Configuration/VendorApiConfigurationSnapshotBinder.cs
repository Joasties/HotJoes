namespace HotJoes.Api.Vendor.Configuration;

public sealed class VendorApiConfigurationSnapshotBinder
    : IConfigurationSnapshotBinder<VendorApiConfigurationSnapshot>
{
    private const string EnvironmentNameKey =
        "VendorApi:EnvironmentName";
    private const string AddressServiceBaseUriKey =
        "VendorApi:AddressServiceBaseUri";
    private const string KeyVaultUriKey =
        "VendorApi:KeyVaultUri";
    private const string PersistenceSecretNameKey =
        "VendorApi:PersistenceConnectionSecretName";
    private const string PersistenceSecretVersionKey =
        "VendorApi:PersistenceConnectionSecretVersion";
    private const string PersistenceSecretPurpose =
        "vendor-persistence-connection";

    public VendorApiConfigurationSnapshot Bind(
        IReadOnlyDictionary<string, string?> settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        string environmentName = RequireValue(
            settings,
            EnvironmentNameKey);
        Uri addressServiceBaseUri = RequireAbsoluteUri(
            settings,
            AddressServiceBaseUriKey);
        Uri keyVaultUri = RequireAbsoluteHttpsUri(
            settings,
            KeyVaultUriKey);
        string persistenceSecretName = RequireValue(
            settings,
            PersistenceSecretNameKey);
        string persistenceSecretVersion = RequireValue(
            settings,
            PersistenceSecretVersionKey);

        return new VendorApiConfigurationSnapshot(
            environmentName,
            addressServiceBaseUri,
            keyVaultUri,
            new RequiredSecretReference(
                PersistenceSecretPurpose,
                persistenceSecretName,
                persistenceSecretVersion));
    }

    private static Uri RequireAbsoluteUri(
        IReadOnlyDictionary<string, string?> settings,
        string key)
    {
        string value = RequireValue(settings, key);

        if (!Uri.TryCreate(
                value,
                UriKind.RelativeOrAbsolute,
                out Uri? uri) ||
            !uri.IsAbsoluteUri)
        {
            throw new InvalidOperationException(
                $"Required configuration setting '{key}' must be an " +
                    "absolute URI.");
        }

        return uri;
    }

    private static Uri RequireAbsoluteHttpsUri(
        IReadOnlyDictionary<string, string?> settings,
        string key)
    {
        Uri uri = RequireAbsoluteUri(settings, key);

        if (!string.Equals(
                uri.Scheme,
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new InvalidOperationException(
                $"Required configuration setting '{key}' must be an " +
                    "absolute HTTPS URI with a host.");
        }

        return uri;
    }

    private static string RequireValue(
        IReadOnlyDictionary<string, string?> settings,
        string key)
    {
        if (!settings.TryGetValue(key, out string? value) ||
            string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Required configuration setting '{key}' is unavailable.");
        }

        return value;
    }
}
