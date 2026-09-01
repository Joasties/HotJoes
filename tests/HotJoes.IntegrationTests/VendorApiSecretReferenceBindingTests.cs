using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiSecretReferenceBindingTests
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
    public void Bind_CompleteSecretReferences_ReturnsNonSecretMetadata()
    {
        var binder = new VendorApiConfigurationSnapshotBinder();

        VendorApiConfigurationSnapshot snapshot = binder.Bind(
            CompleteSettings());

        Assert.Equal(
            new Uri("https://hotjoes-production.vault.azure.net"),
            snapshot.KeyVaultUri);
        Assert.Equal(
            "vendor-persistence-connection",
            snapshot.PersistenceConnectionSecretReference.Purpose);
        Assert.Equal(
            "vendor-api-persistence",
            snapshot.PersistenceConnectionSecretReference.Name);
        Assert.Equal(
            "9f59a476756e4fe8a9c816a9e58d80c7",
            snapshot.PersistenceConnectionSecretReference.Version);
    }

    [Theory]
    [InlineData(KeyVaultUriKey)]
    [InlineData(PersistenceSecretNameKey)]
    [InlineData(PersistenceSecretVersionKey)]
    public void Bind_RequiredSecretReferenceMetadataMissing_RejectsSnapshot(
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
    public void Bind_RequiredSecretReferenceMetadataBlank_RejectsSnapshot(
        string? invalidValue)
    {
        var binder = new VendorApiConfigurationSnapshotBinder();

        foreach (string key in new[]
            {
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
    [InlineData("http://hotjoes-production.vault.azure.net")]
    [InlineData("/relative-vault")]
    [InlineData("not a uri")]
    public void Bind_KeyVaultUriIsNotAbsoluteHttps_RejectsSnapshot(
        string invalidVaultUri)
    {
        var settings = CompleteSettings();
        settings[KeyVaultUriKey] = invalidVaultUri;
        var binder = new VendorApiConfigurationSnapshotBinder();

        Assert.Throws<InvalidOperationException>(
            () => binder.Bind(settings));
    }

    [Fact]
    public void IsValid_CompleteVersionedSecretReference_ReturnsTrue()
    {
        var validator = new VendorApiConfigurationSnapshotValidator(
            "production");

        bool result = validator.IsValid(Snapshot());

        Assert.True(result);
    }

    [Theory]
    [InlineData("", "version")]
    [InlineData("secret-name", "")]
    public void IsValid_IncompleteSecretReference_ReturnsFalse(
        string name,
        string version)
    {
        var validator = new VendorApiConfigurationSnapshotValidator(
            "production");
        var snapshot = new VendorApiConfigurationSnapshot(
            "production",
            new Uri("https://address.internal.example"),
            new Uri("https://hotjoes-production.vault.azure.net"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                name,
                version));

        bool result = validator.IsValid(snapshot);

        Assert.False(result);
    }

    [Fact]
    public void Classify_SelectedSecretVersionChanges_RequiresRestart()
    {
        VendorApiConfigurationSnapshot current = Snapshot("version-a");
        VendorApiConfigurationSnapshot candidate = Snapshot("version-b");
        var classifier =
            new VendorApiConfigurationSnapshotChangeClassifier();

        ConfigurationChangeClassification result = classifier.Classify(
            current,
            candidate);

        Assert.Equal(
            ConfigurationChangeClassification.RestartRequired,
            result);
    }

    [Fact]
    public void Classify_KeyVaultUriChanges_RequiresRestart()
    {
        VendorApiConfigurationSnapshot current = Snapshot();
        var candidate = current with
        {
            KeyVaultUri =
                new Uri("https://hotjoes-recovery.vault.azure.net")
        };
        var classifier =
            new VendorApiConfigurationSnapshotChangeClassifier();

        ConfigurationChangeClassification result = classifier.Classify(
            current,
            candidate);

        Assert.Equal(
            ConfigurationChangeClassification.RestartRequired,
            result);
    }

    [Fact]
    public void Snapshot_SecretMetadataShape_ContainsNoSecretValueMember()
    {
        string[] referencePropertyNames = typeof(RequiredSecretReference)
            .GetProperties()
            .Select(property => property.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[] { "Name", "Purpose", "Version" },
            referencePropertyNames);
        Assert.DoesNotContain(
            typeof(VendorApiConfigurationSnapshot).GetProperties(),
            property => property.Name.Contains(
                "Value",
                StringComparison.OrdinalIgnoreCase));
    }

    private static VendorApiConfigurationSnapshot Snapshot(
        string version = "9f59a476756e4fe8a9c816a9e58d80c7")
    {
        return new VendorApiConfigurationSnapshot(
            "production",
            new Uri("https://address.internal.example"),
            new Uri("https://hotjoes-production.vault.azure.net"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                "vendor-api-persistence",
                version));
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
}
