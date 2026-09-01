using Azure.Core;
using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiAzureKeyVaultRequiredSecretResolverTests
{
    [Fact]
    public async Task Resolve_RequiredVersionExists_RequestsExactVersion()
    {
        VendorApiConfigurationSnapshot snapshot = Snapshot();
        string value = "synthetic-" + "secret-canary";
        var client = new StubSecretClient(
            new AzureKeyVaultResolvedSecret(
                snapshot.PersistenceConnectionSecretReference.Name,
                snapshot.PersistenceConnectionSecretReference.Version,
                value,
                isEnabled: true));
        var credential = new StubTokenCredential();
        var factory = new RecordingSecretClientFactory(client);
        var resolver = new VendorApiAzureKeyVaultRequiredSecretResolver(
            credential,
            factory);

        bool result = await resolver.ResolveRequiredSecretsAsync(snapshot);

        Assert.True(result);
        Assert.Equal(snapshot.KeyVaultUri, factory.VaultUri);
        Assert.Same(credential, factory.Credential);
        Assert.Equal(
            new[]
            {
                new SecretVersionRequest(
                    "vendor-api-persistence",
                    "9f59a476756e4fe8a9c816a9e58d80c7")
            },
            client.Requests);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("synthetic-value", false)]
    public async Task Resolve_RequiredVersionUnavailableOrInvalid_FailsClosed(
        string? value,
        bool isEnabled)
    {
        VendorApiConfigurationSnapshot snapshot = Snapshot();
        AzureKeyVaultResolvedSecret? resolved = value is null
            ? null
            : new AzureKeyVaultResolvedSecret(
                snapshot.PersistenceConnectionSecretReference.Name,
                snapshot.PersistenceConnectionSecretReference.Version,
                value,
                isEnabled);
        var resolver = Resolver(new StubSecretClient(resolved));

        bool result = await resolver.ResolveRequiredSecretsAsync(snapshot);

        Assert.False(result);
    }

    [Theory]
    [InlineData("different-name", "9f59a476756e4fe8a9c816a9e58d80c7")]
    [InlineData("vendor-api-persistence", "different-version")]
    public async Task Resolve_ReturnedIdentityDoesNotMatchReference_FailsClosed(
        string returnedName,
        string returnedVersion)
    {
        var client = new StubSecretClient(
            new AzureKeyVaultResolvedSecret(
                returnedName,
                returnedVersion,
                "synthetic-value",
                isEnabled: true));
        var resolver = Resolver(client);

        bool result = await resolver.ResolveRequiredSecretsAsync(Snapshot());

        Assert.False(result);
        Assert.Single(client.Requests);
        Assert.False(client.LatestVersionWasRequested);
    }

    [Fact]
    public async Task Resolve_KeyVaultUnavailable_FailsClosedWithoutProviderException()
    {
        string providerMessage = "synthetic-" + "secret-canary";
        var resolver = Resolver(
            new StubSecretClient(
                new InvalidOperationException(providerMessage)));

        bool result = await resolver.ResolveRequiredSecretsAsync(Snapshot());

        Assert.False(result);
    }

    [Fact]
    public async Task Resolve_CallerCancellation_Propagates()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var resolver = Resolver(
            new StubSecretClient(
                new AzureKeyVaultResolvedSecret(
                    "vendor-api-persistence",
                    "9f59a476756e4fe8a9c816a9e58d80c7",
                    "synthetic-value",
                    isEnabled: true)));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => resolver.ResolveRequiredSecretsAsync(
                Snapshot(),
                cancellation.Token));
    }

    [Fact]
    public async Task Bootstrap_KeyVaultResolutionFails_DoesNotActivateSnapshot()
    {
        VendorApiConfigurationSnapshot snapshot = Snapshot();
        var activator = new RecordingActivator();
        var bootstrapper = new ConfigurationSnapshotBootstrapper<
            VendorApiConfigurationSnapshot>(
                [new StubReplica(snapshot)],
                new VendorApiConfigurationSnapshotValidator("production"),
                Resolver(new StubSecretClient(resolvedSecret: null)),
                activator);

        ConfigurationBootstrapResult<VendorApiConfigurationSnapshot> result =
            await bootstrapper.BootstrapAsync();

        Assert.False(result.IsReady);
        Assert.Null(result.Snapshot);
        Assert.Empty(activator.Snapshots);
    }

    private static VendorApiAzureKeyVaultRequiredSecretResolver Resolver(
        IAzureKeyVaultSecretClient client)
    {
        return new VendorApiAzureKeyVaultRequiredSecretResolver(
            new StubTokenCredential(),
            new RecordingSecretClientFactory(client));
    }

    private static VendorApiConfigurationSnapshot Snapshot()
    {
        return new VendorApiConfigurationSnapshot(
            "production",
            new Uri("https://address.internal.example"),
            new Uri("https://hotjoes-production.vault.azure.net"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                "vendor-api-persistence",
                "9f59a476756e4fe8a9c816a9e58d80c7"));
    }

    private sealed class RecordingSecretClientFactory
        : IAzureKeyVaultSecretClientFactory
    {
        private readonly IAzureKeyVaultSecretClient _client;

        public RecordingSecretClientFactory(IAzureKeyVaultSecretClient client)
        {
            _client = client;
        }

        public Uri? VaultUri { get; private set; }

        public TokenCredential? Credential { get; private set; }

        public IAzureKeyVaultSecretClient Create(
            Uri vaultUri,
            TokenCredential credential)
        {
            VaultUri = vaultUri;
            Credential = credential;
            return _client;
        }
    }

    private sealed class StubSecretClient : IAzureKeyVaultSecretClient
    {
        private readonly AzureKeyVaultResolvedSecret? _resolvedSecret;
        private readonly Exception? _failure;
        private readonly List<SecretVersionRequest> _requests = [];

        public StubSecretClient(AzureKeyVaultResolvedSecret? resolvedSecret)
        {
            _resolvedSecret = resolvedSecret;
        }

        public StubSecretClient(Exception failure)
        {
            _failure = failure;
        }

        public IReadOnlyList<SecretVersionRequest> Requests => _requests;

        public bool LatestVersionWasRequested { get; private set; }

        public Task<AzureKeyVaultResolvedSecret?> GetSecretVersionAsync(
            string name,
            string version,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LatestVersionWasRequested = string.IsNullOrWhiteSpace(version);
            _requests.Add(new SecretVersionRequest(name, version));

            if (_failure is not null)
            {
                return Task.FromException<AzureKeyVaultResolvedSecret?>(
                    _failure);
            }

            return Task.FromResult(_resolvedSecret);
        }

        public Task<bool> DisableSecretVersionAsync(
            string name,
            string version,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }

    private sealed record SecretVersionRequest(
        string Name,
        string Version);

    private sealed class StubTokenCredential : TokenCredential
    {
        public override AccessToken GetToken(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public override ValueTask<AccessToken> GetTokenAsync(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
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
        private readonly List<VendorApiConfigurationSnapshot> _snapshots = [];

        public IReadOnlyList<VendorApiConfigurationSnapshot> Snapshots =>
            _snapshots;

        public Task ActivateAsync(
            VendorApiConfigurationSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _snapshots.Add(snapshot);
            return Task.CompletedTask;
        }
    }
}
