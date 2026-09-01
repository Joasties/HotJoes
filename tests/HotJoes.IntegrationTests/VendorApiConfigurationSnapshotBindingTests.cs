using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiConfigurationSnapshotBindingTests
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

    [Fact]
    public void Bind_CompleteComponentSettings_ReturnsTypedSnapshot()
    {
        var settings = CompleteSettings();
        settings["AnotherComponent:Setting"] = "not-owned";
        var binder = new VendorApiConfigurationSnapshotBinder();

        VendorApiConfigurationSnapshot snapshot = binder.Bind(settings);

        Assert.Equal("production", snapshot.EnvironmentName);
        Assert.Equal(
            new Uri("https://address.internal.example"),
            snapshot.AddressServiceBaseUri);
    }

    [Theory]
    [InlineData(EnvironmentNameKey)]
    [InlineData(AddressServiceBaseUriKey)]
    [InlineData(KeyVaultUriKey)]
    [InlineData(PersistenceSecretNameKey)]
    [InlineData(PersistenceSecretVersionKey)]
    public void Bind_RequiredSettingMissing_RejectsSnapshot(
        string missingKey)
    {
        var settings = CompleteSettings();
        settings.Remove(missingKey);
        var binder = new VendorApiConfigurationSnapshotBinder();

        Assert.Throws<InvalidOperationException>(
            () => binder.Bind(settings));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Bind_RequiredSettingHasNoValue_RejectsSnapshot(
        string? invalidValue)
    {
        var binder = new VendorApiConfigurationSnapshotBinder();

        foreach (string key in new[]
            {
                EnvironmentNameKey,
                AddressServiceBaseUriKey,
                KeyVaultUriKey,
                PersistenceSecretNameKey,
                PersistenceSecretVersionKey
            })
        {
            var settings = CompleteSettings();
            settings[key] = invalidValue;

            Assert.Throws<InvalidOperationException>(
                () => binder.Bind(settings));
        }
    }

    [Theory]
    [InlineData("address.internal.example")]
    [InlineData("/relative/address")]
    [InlineData("not a uri")]
    public void Bind_AddressEndpointIsNotAbsoluteUri_RejectsSnapshot(
        string invalidAddressEndpoint)
    {
        var settings = CompleteSettings();
        settings[AddressServiceBaseUriKey] = invalidAddressEndpoint;
        var binder = new VendorApiConfigurationSnapshotBinder();

        Assert.Throws<InvalidOperationException>(
            () => binder.Bind(settings));
    }

    [Fact]
    public void IsValid_CompleteSnapshotForExpectedEnvironment_ReturnsTrue()
    {
        var validator = new VendorApiConfigurationSnapshotValidator(
            "Production");
        var snapshot = new VendorApiConfigurationSnapshot(
            "production",
            new Uri("https://address.internal.example"),
            new Uri("https://hotjoes-production.vault.azure.net"),
            SecretReference());

        bool result = validator.IsValid(snapshot);

        Assert.True(result);
    }

    [Fact]
    public void IsValid_SnapshotForDifferentEnvironment_ReturnsFalse()
    {
        var validator = new VendorApiConfigurationSnapshotValidator(
            "production");
        var snapshot = new VendorApiConfigurationSnapshot(
            "staging",
            new Uri("https://address.internal.example"),
            new Uri("https://hotjoes-production.vault.azure.net"),
            SecretReference());

        bool result = validator.IsValid(snapshot);

        Assert.False(result);
    }

    [Theory]
    [InlineData("ftp://address.internal.example")]
    [InlineData("file:///address-service")]
    public void IsValid_AddressEndpointIsNotHttpTransport_ReturnsFalse(
        string addressEndpoint)
    {
        var validator = new VendorApiConfigurationSnapshotValidator(
            "production");
        var snapshot = new VendorApiConfigurationSnapshot(
            "production",
            new Uri(addressEndpoint),
            new Uri("https://hotjoes-production.vault.azure.net"),
            SecretReference());

        bool result = validator.IsValid(snapshot);

        Assert.False(result);
    }

    [Fact]
    public void Snapshot_PublicShape_OwnsOnlyApprovedNonSecretSettings()
    {
        string[] propertyNames = typeof(VendorApiConfigurationSnapshot)
            .GetProperties()
            .Select(property => property.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[]
            {
                "AddressServiceBaseUri",
                "EnvironmentName",
                "KeyVaultUri",
                "PersistenceConnectionSecretReference"
            },
            propertyNames);
        Assert.All(
            typeof(VendorApiConfigurationSnapshot).GetProperties(),
            property =>
            {
                string assemblyName =
                    property.PropertyType.Assembly.GetName().Name!;

                Assert.False(
                    assemblyName.StartsWith(
                        "Azure.",
                        StringComparison.Ordinal));
                Assert.DoesNotContain(
                    "Infrastructure",
                    property.PropertyType.FullName ?? string.Empty,
                    StringComparison.Ordinal);
            });
    }

    private static Dictionary<string, string?> CompleteSettings()
    {
        return new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [EnvironmentNameKey] = "production",
            [AddressServiceBaseUriKey] =
                "https://address.internal.example",
            [KeyVaultUriKey] =
                "https://hotjoes-production.vault.azure.net",
            [PersistenceSecretNameKey] = "vendor-api-persistence",
            [PersistenceSecretVersionKey] =
                "9f59a476756e4fe8a9c816a9e58d80c7"
        };
    }

    private static RequiredSecretReference SecretReference()
    {
        return new RequiredSecretReference(
            "vendor-persistence-connection",
            "vendor-api-persistence",
            "9f59a476756e4fe8a9c816a9e58d80c7");
    }
}
