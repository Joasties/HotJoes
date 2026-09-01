namespace HotJoes.Api.Vendor.Configuration;

public sealed record VendorApiConfigurationSnapshot(
    string EnvironmentName,
    Uri AddressServiceBaseUri,
    Uri KeyVaultUri,
    RequiredSecretReference PersistenceConnectionSecretReference);
