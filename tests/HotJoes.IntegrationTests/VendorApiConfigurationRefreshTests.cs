using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiConfigurationRefreshTests
{
    [Fact]
    public async Task Refresh_CompleteValidReloadSafeSnapshot_ReplacesWholeSnapshot()
    {
        var initial = CreateSnapshot("release-1", 10);
        var replacement = CreateSnapshot("release-2", 20);
        var activator = new RecordingActivator();
        var coordinator = CreateCoordinator(
            initial,
            isValid: true,
            ConfigurationChangeClassification.ReloadSafe,
            activator);

        bool applied = await coordinator.RefreshAsync(
            new StubReplica("preferred", replacement));

        Assert.True(applied);
        Assert.Same(replacement, coordinator.CurrentSnapshot);
        Assert.Equal(new[] { replacement }, activator.ActivatedSnapshots);
    }

    [Fact]
    public async Task Refresh_InvalidOrPartialSnapshot_RetainsLastValidSnapshot()
    {
        var initial = CreateSnapshot("release-1", 10);
        var partial = CreateSnapshot("", 20);
        var activator = new RecordingActivator();
        var coordinator = CreateCoordinator(
            initial,
            isValid: false,
            ConfigurationChangeClassification.ReloadSafe,
            activator);

        bool applied = await coordinator.RefreshAsync(
            new StubReplica("preferred", partial));

        Assert.False(applied);
        Assert.Same(initial, coordinator.CurrentSnapshot);
        Assert.Empty(activator.ActivatedSnapshots);
    }

    [Fact]
    public async Task Refresh_ProviderUnavailable_RetainsLastValidSnapshot()
    {
        var initial = CreateSnapshot("release-1", 10);
        var activator = new RecordingActivator();
        var coordinator = CreateCoordinator(
            initial,
            isValid: true,
            ConfigurationChangeClassification.ReloadSafe,
            activator);

        bool applied = await coordinator.RefreshAsync(
            new StubReplica(
                "preferred",
                new InvalidOperationException("Provider unavailable.")));

        Assert.False(applied);
        Assert.Same(initial, coordinator.CurrentSnapshot);
        Assert.Empty(activator.ActivatedSnapshots);
    }

    [Fact]
    public async Task Refresh_RestartRequiredChange_DoesNotHotReload()
    {
        var initial = CreateSnapshot("release-1", 10);
        var replacement = CreateSnapshot("release-2", 20);
        var activator = new RecordingActivator();
        var coordinator = CreateCoordinator(
            initial,
            isValid: true,
            ConfigurationChangeClassification.RestartRequired,
            activator);

        bool applied = await coordinator.RefreshAsync(
            new StubReplica("preferred", replacement));

        Assert.False(applied);
        Assert.Same(initial, coordinator.CurrentSnapshot);
        Assert.Empty(activator.ActivatedSnapshots);
    }

    [Fact]
    public async Task Refresh_ActivationFails_RetainsLastValidSnapshot()
    {
        var initial = CreateSnapshot("release-1", 10);
        var replacement = CreateSnapshot("release-2", 20);
        var activator = new RecordingActivator(
            new InvalidOperationException("Activation failed."));
        var coordinator = CreateCoordinator(
            initial,
            isValid: true,
            ConfigurationChangeClassification.ReloadSafe,
            activator);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            coordinator.RefreshAsync(
                new StubReplica("preferred", replacement)));

        Assert.Same(initial, coordinator.CurrentSnapshot);
        Assert.Empty(activator.ActivatedSnapshots);
    }

    [Fact]
    public async Task Refresh_PreviouslyValidSnapshot_RollsBackAtomically()
    {
        var initial = CreateSnapshot("release-1", 10);
        var replacement = CreateSnapshot("release-2", 20);
        var activator = new RecordingActivator();
        var coordinator = CreateCoordinator(
            initial,
            isValid: true,
            ConfigurationChangeClassification.ReloadSafe,
            activator);

        Assert.True(await coordinator.RefreshAsync(
            new StubReplica("preferred", replacement)));
        Assert.True(await coordinator.RefreshAsync(
            new StubReplica("preferred", initial)));

        Assert.Same(initial, coordinator.CurrentSnapshot);
        Assert.Equal(
            new[] { replacement, initial },
            activator.ActivatedSnapshots);
    }

    private static ConfigurationSnapshotRefreshCoordinator<
        VendorApiRefreshOptions> CreateCoordinator(
            VendorApiRefreshOptions initial,
            bool isValid,
            ConfigurationChangeClassification classification,
            RecordingActivator activator)
    {
        return new ConfigurationSnapshotRefreshCoordinator<
            VendorApiRefreshOptions>(
                initial,
                new StubValidator(isValid),
                new StubChangeClassifier(classification),
                activator);
    }

    private static VendorApiRefreshOptions CreateSnapshot(
        string releaseName,
        int addressTimeoutSeconds)
    {
        return new VendorApiRefreshOptions(
            releaseName,
            addressTimeoutSeconds);
    }

    private sealed record VendorApiRefreshOptions(
        string ReleaseName,
        int AddressTimeoutSeconds);

    private sealed class StubReplica
        : IConfigurationSnapshotReplica<VendorApiRefreshOptions>
    {
        private readonly VendorApiRefreshOptions? _snapshot;
        private readonly Exception? _failure;

        public StubReplica(
            string name,
            VendorApiRefreshOptions snapshot)
        {
            Name = name;
            _snapshot = snapshot;
        }

        public StubReplica(string name, Exception failure)
        {
            Name = name;
            _failure = failure;
        }

        public string Name { get; }

        public Task<VendorApiRefreshOptions> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_failure is not null)
            {
                return Task.FromException<VendorApiRefreshOptions>(_failure);
            }

            return Task.FromResult(_snapshot!);
        }
    }

    private sealed class StubValidator
        : IConfigurationSnapshotValidator<VendorApiRefreshOptions>
    {
        private readonly bool _isValid;

        public StubValidator(bool isValid)
        {
            _isValid = isValid;
        }

        public bool IsValid(VendorApiRefreshOptions snapshot)
        {
            return _isValid;
        }
    }

    private sealed class StubChangeClassifier
        : IConfigurationSnapshotChangeClassifier<VendorApiRefreshOptions>
    {
        private readonly ConfigurationChangeClassification _classification;

        public StubChangeClassifier(
            ConfigurationChangeClassification classification)
        {
            _classification = classification;
        }

        public ConfigurationChangeClassification Classify(
            VendorApiRefreshOptions currentSnapshot,
            VendorApiRefreshOptions candidateSnapshot)
        {
            return _classification;
        }
    }

    private sealed class RecordingActivator
        : IConfigurationSnapshotActivator<VendorApiRefreshOptions>
    {
        private readonly Exception? _failure;
        private readonly List<VendorApiRefreshOptions> _activatedSnapshots = [];

        public RecordingActivator(Exception? failure = null)
        {
            _failure = failure;
        }

        public IReadOnlyList<VendorApiRefreshOptions> ActivatedSnapshots =>
            _activatedSnapshots;

        public Task ActivateAsync(
            VendorApiRefreshOptions snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_failure is not null)
            {
                return Task.FromException(_failure);
            }

            _activatedSnapshots.Add(snapshot);
            return Task.CompletedTask;
        }
    }
}
