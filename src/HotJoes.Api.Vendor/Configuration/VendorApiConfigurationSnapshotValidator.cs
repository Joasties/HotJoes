namespace HotJoes.Api.Vendor.Configuration;

public sealed class VendorApiConfigurationSnapshotValidator
    : IConfigurationSnapshotValidator<VendorApiConfigurationSnapshot>
{
    private readonly string _expectedEnvironmentName;

    public VendorApiConfigurationSnapshotValidator(
        string expectedEnvironmentName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            expectedEnvironmentName);

        _expectedEnvironmentName = expectedEnvironmentName;
    }

    public bool IsValid(VendorApiConfigurationSnapshot snapshot)
    {
        if (snapshot is null ||
            string.IsNullOrWhiteSpace(snapshot.EnvironmentName) ||
            !IsAbsoluteHttpUri(snapshot.AddressServiceBaseUri) ||
            !IsAbsoluteHttpsUri(snapshot.KeyVaultUri) ||
            !IsValidReference(
                snapshot.PersistenceConnectionSecretReference))
        {
            return false;
        }

        return string.Equals(
            snapshot.EnvironmentName,
            _expectedEnvironmentName,
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAbsoluteHttpUri(Uri? uri)
    {
        return uri is not null &&
            uri.IsAbsoluteUri &&
            (string.Equals(
                    uri.Scheme,
                    Uri.UriSchemeHttp,
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    uri.Scheme,
                    Uri.UriSchemeHttps,
                    StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsAbsoluteHttpsUri(Uri? uri)
    {
        return uri is not null &&
            uri.IsAbsoluteUri &&
            string.Equals(
                uri.Scheme,
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(uri.Host);
    }

    private static bool IsValidReference(
        RequiredSecretReference? reference)
    {
        return reference is not null &&
            !string.IsNullOrWhiteSpace(reference.Purpose) &&
            !string.IsNullOrWhiteSpace(reference.Name) &&
            !string.IsNullOrWhiteSpace(reference.Version);
    }
}
