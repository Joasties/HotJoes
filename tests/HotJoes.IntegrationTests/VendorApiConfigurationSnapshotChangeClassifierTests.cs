using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiConfigurationSnapshotChangeClassifierTests
{
    [Fact]
    public void Classify_ValueEquivalentSnapshots_ReturnsReloadSafe()
    {
        var current = Snapshot(
            "production",
            "https://address.internal.example/vendors");
        var candidate = Snapshot(
            "production",
            "https://address.internal.example/vendors");
        var classifier =
            new VendorApiConfigurationSnapshotChangeClassifier();

        ConfigurationChangeClassification result = classifier.Classify(
            current,
            candidate);

        Assert.Equal(
            ConfigurationChangeClassification.ReloadSafe,
            result);
    }

    [Fact]
    public void Classify_EnvironmentCasingOnly_ReturnsReloadSafe()
    {
        var current = Snapshot(
            "Production",
            "https://address.internal.example");
        var candidate = Snapshot(
            "production",
            "https://address.internal.example");
        var classifier =
            new VendorApiConfigurationSnapshotChangeClassifier();

        ConfigurationChangeClassification result = classifier.Classify(
            current,
            candidate);

        Assert.Equal(
            ConfigurationChangeClassification.ReloadSafe,
            result);
    }

    [Fact]
    public void Classify_EquivalentUriCasingOnly_ReturnsReloadSafe()
    {
        var current = Snapshot(
            "production",
            "https://address.internal.example/vendors");
        var candidate = Snapshot(
            "production",
            "HTTPS://ADDRESS.INTERNAL.EXAMPLE/vendors");
        var classifier =
            new VendorApiConfigurationSnapshotChangeClassifier();

        ConfigurationChangeClassification result = classifier.Classify(
            current,
            candidate);

        Assert.Equal(
            ConfigurationChangeClassification.ReloadSafe,
            result);
    }

    [Fact]
    public void Classify_DifferentEnvironment_ReturnsRestartRequired()
    {
        var current = Snapshot(
            "production",
            "https://address.internal.example");
        var candidate = Snapshot(
            "staging",
            "https://address.internal.example");
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
    public void Classify_DifferentAddressEndpoint_ReturnsRestartRequired()
    {
        var current = Snapshot(
            "production",
            "https://address-a.internal.example");
        var candidate = Snapshot(
            "production",
            "https://address-b.internal.example");
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
    public void Classify_NullCurrentSnapshot_Throws()
    {
        var classifier =
            new VendorApiConfigurationSnapshotChangeClassifier();

        Assert.Throws<ArgumentNullException>(() => classifier.Classify(
            null!,
            Snapshot(
                "production",
                "https://address.internal.example")));
    }

    [Fact]
    public void Classify_NullCandidateSnapshot_Throws()
    {
        var classifier =
            new VendorApiConfigurationSnapshotChangeClassifier();

        Assert.Throws<ArgumentNullException>(() => classifier.Classify(
            Snapshot(
                "production",
                "https://address.internal.example"),
            null!));
    }

    [Fact]
    public async Task Refresh_ChangedAddressEndpoint_RetainsCurrentSnapshotWithoutActivation()
    {
        var current = Snapshot(
            "production",
            "https://address-a.internal.example");
        var candidate = Snapshot(
            "production",
            "https://address-b.internal.example");
        var activator = new RecordingActivator();
        var coordinator = new ConfigurationSnapshotRefreshCoordinator<
            VendorApiConfigurationSnapshot>(
                current,
                new VendorApiConfigurationSnapshotValidator("production"),
                new VendorApiConfigurationSnapshotChangeClassifier(),
                activator);

        bool applied = await coordinator.RefreshAsync(
            new StubReplica(candidate));

        Assert.False(applied);
        Assert.Same(current, coordinator.CurrentSnapshot);
        Assert.Empty(activator.ActivatedSnapshots);
    }

    private static VendorApiConfigurationSnapshot Snapshot(
        string environmentName,
        string addressServiceBaseUri)
    {
        return new VendorApiConfigurationSnapshot(
            environmentName,
            new Uri(addressServiceBaseUri),
            new Uri("https://hotjoes-production.vault.azure.net"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                "vendor-api-persistence",
                "9f59a476756e4fe8a9c816a9e58d80c7"));
    }

    private sealed class StubReplica
        : IConfigurationSnapshotReplica<VendorApiConfigurationSnapshot>
    {
        private readonly VendorApiConfigurationSnapshot _snapshot;

        public StubReplica(VendorApiConfigurationSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public string Name => "preferred";

        public Task<VendorApiConfigurationSnapshot> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_snapshot);
        }
    }

    private sealed class RecordingActivator
        : IConfigurationSnapshotActivator<VendorApiConfigurationSnapshot>
    {
        private readonly List<VendorApiConfigurationSnapshot>
            _activatedSnapshots = [];

        public IReadOnlyList<VendorApiConfigurationSnapshot>
            ActivatedSnapshots => _activatedSnapshots;

        public Task ActivateAsync(
            VendorApiConfigurationSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _activatedSnapshots.Add(snapshot);
            return Task.CompletedTask;
        }
    }
}
