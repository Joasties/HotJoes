using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiConfigurationBootstrapTests
{
    [Fact]
    public async Task Bootstrap_PreferredReplicaProvidesValidSnapshot_ActivatesWithoutFallback()
    {
        var calls = new List<string>();
        var preferredSnapshot = CreateSnapshot("production");
        var preferred = new StubReplica(
            "preferred",
            preferredSnapshot,
            calls);
        var fallback = new StubReplica(
            "fallback",
            CreateSnapshot("fallback-production"),
            calls);
        var validator = new StubValidator(isValid: true, calls);
        var secretResolver = new StubSecretResolver(
            requiredSecretsResolved: true,
            calls);
        var activator = new RecordingActivator(calls);
        var bootstrapper = CreateBootstrapper(
            preferred,
            fallback,
            validator,
            secretResolver,
            activator);

        ConfigurationBootstrapResult<VendorApiBootstrapOptions> result =
            await bootstrapper.BootstrapAsync();

        Assert.True(result.IsReady);
        Assert.Same(preferredSnapshot, result.Snapshot);
        Assert.Equal("preferred", result.AuthoritativeReplicaName);
        Assert.Equal(
            new[]
            {
                "load:preferred",
                "validate:production",
                "resolve-secrets:production",
                "activate:production"
            },
            calls);
        Assert.Equal(0, fallback.LoadCount);
    }

    [Fact]
    public async Task Bootstrap_PreferredReplicaUnavailable_UsesApprovedFallback()
    {
        var calls = new List<string>();
        var preferred = new StubReplica(
            "preferred",
            new InvalidOperationException("Provider unavailable."),
            calls);
        var fallbackSnapshot = CreateSnapshot("production");
        var fallback = new StubReplica(
            "fallback",
            fallbackSnapshot,
            calls);
        var bootstrapper = CreateBootstrapper(
            preferred,
            fallback,
            new StubValidator(isValid: true, calls),
            new StubSecretResolver(requiredSecretsResolved: true, calls),
            new RecordingActivator(calls));

        ConfigurationBootstrapResult<VendorApiBootstrapOptions> result =
            await bootstrapper.BootstrapAsync();

        Assert.True(result.IsReady);
        Assert.Same(fallbackSnapshot, result.Snapshot);
        Assert.Equal("fallback", result.AuthoritativeReplicaName);
        Assert.Equal(
            new[]
            {
                "load:preferred",
                "load:fallback",
                "validate:production",
                "resolve-secrets:production",
                "activate:production"
            },
            calls);
    }

    [Fact]
    public async Task Bootstrap_AllReplicasUnavailable_RemainsUnreadyAndDoesNotActivate()
    {
        var calls = new List<string>();
        var bootstrapper = CreateBootstrapper(
            new StubReplica(
                "preferred",
                new InvalidOperationException("Preferred unavailable."),
                calls),
            new StubReplica(
                "fallback",
                new InvalidOperationException("Fallback unavailable."),
                calls),
            new StubValidator(isValid: true, calls),
            new StubSecretResolver(requiredSecretsResolved: true, calls),
            new RecordingActivator(calls));

        ConfigurationBootstrapResult<VendorApiBootstrapOptions> result =
            await bootstrapper.BootstrapAsync();

        Assert.False(result.IsReady);
        Assert.Null(result.Snapshot);
        Assert.Null(result.AuthoritativeReplicaName);
        Assert.Equal(
            new[] { "load:preferred", "load:fallback" },
            calls);
    }

    [Fact]
    public async Task Bootstrap_AvailableSnapshotsInvalid_RemainsUnreadyAndDoesNotActivate()
    {
        var calls = new List<string>();
        var bootstrapper = CreateBootstrapper(
            new StubReplica(
                "preferred",
                CreateSnapshot("invalid-preferred"),
                calls),
            new StubReplica(
                "fallback",
                CreateSnapshot("invalid-fallback"),
                calls),
            new StubValidator(isValid: false, calls),
            new StubSecretResolver(requiredSecretsResolved: true, calls),
            new RecordingActivator(calls));

        ConfigurationBootstrapResult<VendorApiBootstrapOptions> result =
            await bootstrapper.BootstrapAsync();

        Assert.False(result.IsReady);
        Assert.Null(result.Snapshot);
        Assert.Null(result.AuthoritativeReplicaName);
        Assert.Equal(
            new[]
            {
                "load:preferred",
                "validate:invalid-preferred",
                "load:fallback",
                "validate:invalid-fallback"
            },
            calls);
    }

    [Fact]
    public async Task Bootstrap_RequiredSecretUnavailable_RemainsUnreadyAndDoesNotActivate()
    {
        var calls = new List<string>();
        var snapshot = CreateSnapshot("production");
        var fallback = new StubReplica(
            "fallback",
            CreateSnapshot("fallback-production"),
            calls);
        var bootstrapper = CreateBootstrapper(
            new StubReplica("preferred", snapshot, calls),
            fallback,
            new StubValidator(isValid: true, calls),
            new StubSecretResolver(requiredSecretsResolved: false, calls),
            new RecordingActivator(calls));

        ConfigurationBootstrapResult<VendorApiBootstrapOptions> result =
            await bootstrapper.BootstrapAsync();

        Assert.False(result.IsReady);
        Assert.Null(result.Snapshot);
        Assert.Null(result.AuthoritativeReplicaName);
        Assert.Equal(
            new[]
            {
                "load:preferred",
                "validate:production",
                "resolve-secrets:production"
            },
            calls.Take(3));
        Assert.DoesNotContain(
            calls,
            call => call.StartsWith(
                "activate:",
                StringComparison.Ordinal));
    }

    private static ConfigurationSnapshotBootstrapper<VendorApiBootstrapOptions>
        CreateBootstrapper(
            IConfigurationSnapshotReplica<VendorApiBootstrapOptions> preferred,
            IConfigurationSnapshotReplica<VendorApiBootstrapOptions> fallback,
            IConfigurationSnapshotValidator<VendorApiBootstrapOptions> validator,
            IRequiredSecretResolver<VendorApiBootstrapOptions> secretResolver,
            IConfigurationSnapshotActivator<VendorApiBootstrapOptions> activator)
    {
        return new ConfigurationSnapshotBootstrapper<VendorApiBootstrapOptions>(
            new[] { preferred, fallback },
            validator,
            secretResolver,
            activator);
    }

    private static VendorApiBootstrapOptions CreateSnapshot(
        string environmentName)
    {
        return new VendorApiBootstrapOptions(
            environmentName,
            new Uri("https://address.internal.example"));
    }

    private sealed record VendorApiBootstrapOptions(
        string EnvironmentName,
        Uri AddressServiceBaseUri);

    private sealed class StubReplica
        : IConfigurationSnapshotReplica<VendorApiBootstrapOptions>
    {
        private readonly VendorApiBootstrapOptions? _snapshot;
        private readonly Exception? _failure;
        private readonly List<string> _calls;

        public StubReplica(
            string name,
            VendorApiBootstrapOptions snapshot,
            List<string> calls)
        {
            Name = name;
            _snapshot = snapshot;
            _calls = calls;
        }

        public StubReplica(
            string name,
            Exception failure,
            List<string> calls)
        {
            Name = name;
            _failure = failure;
            _calls = calls;
        }

        public string Name { get; }

        public int LoadCount { get; private set; }

        public Task<VendorApiBootstrapOptions> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LoadCount++;
            _calls.Add($"load:{Name}");

            if (_failure is not null)
            {
                return Task.FromException<VendorApiBootstrapOptions>(_failure);
            }

            return Task.FromResult(_snapshot!);
        }
    }

    private sealed class StubValidator
        : IConfigurationSnapshotValidator<VendorApiBootstrapOptions>
    {
        private readonly bool _isValid;
        private readonly List<string> _calls;

        public StubValidator(bool isValid, List<string> calls)
        {
            _isValid = isValid;
            _calls = calls;
        }

        public bool IsValid(VendorApiBootstrapOptions snapshot)
        {
            _calls.Add($"validate:{snapshot.EnvironmentName}");
            return _isValid;
        }
    }

    private sealed class StubSecretResolver
        : IRequiredSecretResolver<VendorApiBootstrapOptions>
    {
        private readonly bool _requiredSecretsResolved;
        private readonly List<string> _calls;

        public StubSecretResolver(
            bool requiredSecretsResolved,
            List<string> calls)
        {
            _requiredSecretsResolved = requiredSecretsResolved;
            _calls = calls;
        }

        public Task<bool> ResolveRequiredSecretsAsync(
            VendorApiBootstrapOptions snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _calls.Add($"resolve-secrets:{snapshot.EnvironmentName}");
            return Task.FromResult(_requiredSecretsResolved);
        }
    }

    private sealed class RecordingActivator
        : IConfigurationSnapshotActivator<VendorApiBootstrapOptions>
    {
        private readonly List<string> _calls;

        public RecordingActivator(List<string> calls)
        {
            _calls = calls;
        }

        public Task ActivateAsync(
            VendorApiBootstrapOptions snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _calls.Add($"activate:{snapshot.EnvironmentName}");
            return Task.CompletedTask;
        }
    }
}
