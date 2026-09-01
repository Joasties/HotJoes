namespace HotJoes.Api.Vendor.Configuration;

public sealed class VendorApiConfigurationSnapshotChangeClassifier
    : IConfigurationSnapshotChangeClassifier<VendorApiConfigurationSnapshot>
{
    public ConfigurationChangeClassification Classify(
        VendorApiConfigurationSnapshot currentSnapshot,
        VendorApiConfigurationSnapshot candidateSnapshot)
    {
        ArgumentNullException.ThrowIfNull(currentSnapshot);
        ArgumentNullException.ThrowIfNull(candidateSnapshot);

        bool environmentIsEquivalent = string.Equals(
            currentSnapshot.EnvironmentName,
            candidateSnapshot.EnvironmentName,
            StringComparison.OrdinalIgnoreCase);
        bool addressEndpointIsEquivalent =
            currentSnapshot.AddressServiceBaseUri.Equals(
                candidateSnapshot.AddressServiceBaseUri);
        bool keyVaultIsEquivalent = currentSnapshot.KeyVaultUri.Equals(
            candidateSnapshot.KeyVaultUri);
        bool secretReferenceIsEquivalent =
            currentSnapshot.PersistenceConnectionSecretReference ==
                candidateSnapshot.PersistenceConnectionSecretReference;

        return environmentIsEquivalent &&
            addressEndpointIsEquivalent &&
            keyVaultIsEquivalent &&
            secretReferenceIsEquivalent
            ? ConfigurationChangeClassification.ReloadSafe
            : ConfigurationChangeClassification.RestartRequired;
    }
}
