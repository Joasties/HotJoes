using Azure;
using Azure.Core;
using Azure.Data.AppConfiguration;
using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiAzureAppConfigurationReplicaTopologyTests
{
    private static readonly Uri PreferredEndpoint =
        new("https://preferred.azconfig.io");
    private static readonly Uri FallbackEndpoint =
        new("https://fallback.azconfig.io");

    [Fact]
    public async Task Create_ApprovedDefinitions_PreservesOrderCredentialAndSnapshot()
    {
        var credential = new StubTokenCredential();
        var preferredClient = new StubConfigurationClient(
            CompleteSettings("production"));
        var fallbackClient = new StubConfigurationClient(
            CompleteSettings("production"));
        var factory = new RecordingClientFactory(
            new Dictionary<Uri, ConfigurationClient>
            {
                [PreferredEndpoint] = preferredClient,
                [FallbackEndpoint] = fallbackClient
            });

        var topology = CreateTopology(
            credential,
            factory,
            new VendorApiConfigurationSnapshotBinder());

        Assert.Equal(
            new[] { "preferred", "cross-region" },
            topology.Replicas.Select(replica => replica.Name));
        Assert.Equal(
            new[] { PreferredEndpoint, FallbackEndpoint },
            factory.Calls.Select(call => call.Endpoint));
        Assert.All(
            factory.Calls,
            call => Assert.Same(credential, call.Credential));

        foreach (IConfigurationSnapshotReplica<
            VendorApiConfigurationSnapshot> replica in topology.Replicas)
        {
            VendorApiConfigurationSnapshot snapshot =
                await replica.LoadAsync();

            Assert.Equal("production", snapshot.EnvironmentName);
        }

        Assert.Equal(
            "vendor-api-production-42",
            preferredClient.RequestedSnapshotName);
        Assert.Equal(
            "vendor-api-production-42",
            fallbackClient.RequestedSnapshotName);
    }

    [Fact]
    public void Create_FewerThanTwoDefinitions_RejectsTopology()
    {
        Assert.Throws<ArgumentException>(() => CreateTopology(
            Array.Empty<AzureAppConfigurationReplicaDefinition>()));
        Assert.Throws<ArgumentException>(() => CreateTopology(
            new[]
            {
                Definition("preferred", PreferredEndpoint)
            }));
    }

    [Fact]
    public void Create_NullDefinitionOrBlankName_RejectsTopology()
    {
        Assert.Throws<ArgumentException>(() => CreateTopology(
            new AzureAppConfigurationReplicaDefinition[]
            {
                null!,
                Definition("cross-region", FallbackEndpoint)
            }));
        Assert.Throws<ArgumentException>(() => CreateTopology(
            new[]
            {
                Definition("   ", PreferredEndpoint),
                Definition("cross-region", FallbackEndpoint)
            }));
    }

    [Theory]
    [InlineData("/relative/app-configuration")]
    [InlineData("http://insecure.azconfig.io")]
    [InlineData("https:hostless")]
    public void Create_InvalidAzureEndpoint_RejectsTopology(
        string endpointValue)
    {
        var invalidEndpoint = new Uri(
            endpointValue,
            UriKind.RelativeOrAbsolute);

        Assert.Throws<ArgumentException>(() => CreateTopology(
            new[]
            {
                Definition("preferred", invalidEndpoint),
                Definition("cross-region", FallbackEndpoint)
            }));
    }

    [Fact]
    public void Create_DuplicateNamesOrEndpoints_RejectsTopology()
    {
        Assert.Throws<ArgumentException>(() => CreateTopology(
            new[]
            {
                Definition("preferred", PreferredEndpoint),
                Definition("PREFERRED", FallbackEndpoint)
            }));
        Assert.Throws<ArgumentException>(() => CreateTopology(
            new[]
            {
                Definition("preferred", PreferredEndpoint),
                Definition(
                    "cross-region",
                    new Uri("HTTPS://PREFERRED.AZCONFIG.IO"))
            }));
    }

    [Fact]
    public void Create_InvalidSharedInputs_RejectsTopology()
    {
        AzureAppConfigurationReplicaDefinition[] definitions = Definitions();
        var credential = new StubTokenCredential();
        var binder = new VendorApiConfigurationSnapshotBinder();
        var factory = new RecordingClientFactory();

        Assert.Throws<ArgumentException>(() =>
            new VendorApiAzureAppConfigurationReplicaTopology(
                definitions,
                "   ",
                credential,
                binder,
                factory));
        Assert.Throws<ArgumentNullException>(() =>
            new VendorApiAzureAppConfigurationReplicaTopology(
                definitions,
                "vendor-api-production-42",
                null!,
                binder,
                factory));
        Assert.Throws<ArgumentNullException>(() =>
            new VendorApiAzureAppConfigurationReplicaTopology(
                definitions,
                "vendor-api-production-42",
                credential,
                null!,
                factory));
        Assert.Throws<ArgumentNullException>(() =>
            new VendorApiAzureAppConfigurationReplicaTopology(
                definitions,
                "vendor-api-production-42",
                credential,
                binder,
                null!));
    }

    [Fact]
    public void Create_ClientFactoryFails_DoesNotReturnPartialTopology()
    {
        var expected = new InvalidOperationException(
            "Cross-region client construction failed.");
        var factory = new RecordingClientFactory(
            failureEndpoint: FallbackEndpoint,
            failure: expected);

        InvalidOperationException actual =
            Assert.Throws<InvalidOperationException>(() => CreateTopology(
                new StubTokenCredential(),
                factory,
                new VendorApiConfigurationSnapshotBinder()));

        Assert.Same(expected, actual);
        Assert.Equal(2, factory.Calls.Count);
    }

    [Fact]
    public async Task Bootstrap_PreferredUnavailable_UsesCrossRegionReplica()
    {
        var preferredFailure = new RequestFailedException(
            503,
            "Preferred replica unavailable.");
        var factory = new RecordingClientFactory(
            new Dictionary<Uri, ConfigurationClient>
            {
                [PreferredEndpoint] =
                    new StubConfigurationClient(preferredFailure),
                [FallbackEndpoint] = new StubConfigurationClient(
                    CompleteSettings("production"))
            });
        var topology = CreateTopology(
            new StubTokenCredential(),
            factory,
            new VendorApiConfigurationSnapshotBinder());
        var bootstrapper = new ConfigurationSnapshotBootstrapper<
            VendorApiConfigurationSnapshot>(
                topology.Replicas,
                new VendorApiConfigurationSnapshotValidator("production"),
                new SuccessfulSecretResolver(),
                new RecordingActivator());

        ConfigurationBootstrapResult<VendorApiConfigurationSnapshot> result =
            await bootstrapper.BootstrapAsync();

        Assert.True(result.IsReady);
        Assert.Equal("cross-region", result.AuthoritativeReplicaName);
        Assert.Equal("production", result.Snapshot!.EnvironmentName);
    }

    [Fact]
    public void PublicSurface_ExposesNoConnectionStringOrStoredSecret()
    {
        Type[] publicTypes =
        {
            typeof(AzureAppConfigurationReplicaDefinition),
            typeof(IAzureAppConfigurationClientFactory),
            typeof(AzureAppConfigurationClientFactory),
            typeof(VendorApiAzureAppConfigurationReplicaTopology)
        };

        Assert.DoesNotContain(
            publicTypes.SelectMany(type => type.GetProperties()),
            property =>
                property.Name.Contains(
                    "ConnectionString",
                    StringComparison.OrdinalIgnoreCase) ||
                property.Name.Contains(
                    "Secret",
                    StringComparison.OrdinalIgnoreCase));
    }

    private static VendorApiAzureAppConfigurationReplicaTopology
        CreateTopology(
            IEnumerable<AzureAppConfigurationReplicaDefinition> definitions)
    {
        return CreateTopology(
            definitions,
            new StubTokenCredential(),
            new RecordingClientFactory(),
            new VendorApiConfigurationSnapshotBinder());
    }

    private static VendorApiAzureAppConfigurationReplicaTopology
        CreateTopology(
            TokenCredential credential,
            IAzureAppConfigurationClientFactory factory,
            IConfigurationSnapshotBinder<VendorApiConfigurationSnapshot>
                binder)
    {
        return CreateTopology(
            Definitions(),
            credential,
            factory,
            binder);
    }

    private static VendorApiAzureAppConfigurationReplicaTopology
        CreateTopology(
            IEnumerable<AzureAppConfigurationReplicaDefinition> definitions,
            TokenCredential credential,
            IAzureAppConfigurationClientFactory factory,
            IConfigurationSnapshotBinder<VendorApiConfigurationSnapshot>
                binder)
    {
        return new VendorApiAzureAppConfigurationReplicaTopology(
            definitions,
            "vendor-api-production-42",
            credential,
            binder,
            factory);
    }

    private static AzureAppConfigurationReplicaDefinition[] Definitions()
    {
        return
        [
            Definition("preferred", PreferredEndpoint),
            Definition("cross-region", FallbackEndpoint)
        ];
    }

    private static AzureAppConfigurationReplicaDefinition Definition(
        string name,
        Uri endpoint)
    {
        return new AzureAppConfigurationReplicaDefinition(name, endpoint);
    }

    private static ConfigurationSetting[] CompleteSettings(
        string environmentName)
    {
        return
        [
            new ConfigurationSetting(
                "VendorApi:EnvironmentName",
                environmentName),
            new ConfigurationSetting(
                "VendorApi:AddressServiceBaseUri",
                "https://address.internal.example"),
            new ConfigurationSetting(
                "VendorApi:KeyVaultUri",
                "https://hotjoes-production.vault.azure.net"),
            new ConfigurationSetting(
                "VendorApi:PersistenceConnectionSecretName",
                "vendor-api-persistence"),
            new ConfigurationSetting(
                "VendorApi:PersistenceConnectionSecretVersion",
                "9f59a476756e4fe8a9c816a9e58d80c7")
        ];
    }

    private sealed class StubTokenCredential : TokenCredential
    {
        private static readonly AccessToken Token = new(
            "test-token",
            DateTimeOffset.UtcNow.AddHours(1));

        public override AccessToken GetToken(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Token;
        }

        public override ValueTask<AccessToken> GetTokenAsync(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(Token);
        }
    }

    private sealed class RecordingClientFactory
        : IAzureAppConfigurationClientFactory
    {
        private readonly IReadOnlyDictionary<Uri, ConfigurationClient>
            _clients;
        private readonly Uri? _failureEndpoint;
        private readonly Exception? _failure;
        private readonly List<ClientFactoryCall> _calls = [];

        public RecordingClientFactory(
            IReadOnlyDictionary<Uri, ConfigurationClient>? clients = null,
            Uri? failureEndpoint = null,
            Exception? failure = null)
        {
            _clients = clients ?? new Dictionary<Uri, ConfigurationClient>();
            _failureEndpoint = failureEndpoint;
            _failure = failure;
        }

        public IReadOnlyList<ClientFactoryCall> Calls => _calls;

        public ConfigurationClient Create(
            Uri endpoint,
            TokenCredential credential)
        {
            _calls.Add(new ClientFactoryCall(endpoint, credential));

            if (_failureEndpoint is not null &&
                _failureEndpoint.Equals(endpoint))
            {
                throw _failure!;
            }

            return _clients.TryGetValue(
                endpoint,
                out ConfigurationClient? client)
                ? client
                : new StubConfigurationClient(
                    CompleteSettings("production"));
        }
    }

    private sealed record ClientFactoryCall(
        Uri Endpoint,
        TokenCredential Credential);

    private sealed class StubConfigurationClient : ConfigurationClient
    {
        private readonly IReadOnlyList<ConfigurationSetting>? _settings;
        private readonly Exception? _failure;

        public StubConfigurationClient(
            IReadOnlyList<ConfigurationSetting> settings)
        {
            _settings = settings;
        }

        public StubConfigurationClient(Exception failure)
        {
            _failure = failure;
        }

        public string? RequestedSnapshotName { get; private set; }

        public override AsyncPageable<ConfigurationSetting>
            GetConfigurationSettingsForSnapshotAsync(
                string snapshotName,
                CancellationToken cancellationToken = default)
        {
            RequestedSnapshotName = snapshotName;
            cancellationToken.ThrowIfCancellationRequested();

            if (_failure is not null)
            {
                throw _failure;
            }

            return AsyncPageable<ConfigurationSetting>.FromPages(
                new[]
                {
                    Page<ConfigurationSetting>.FromValues(
                        _settings!,
                        continuationToken: null,
                        response: null!)
                });
        }
    }

    private sealed class SuccessfulSecretResolver
        : IRequiredSecretResolver<VendorApiConfigurationSnapshot>
    {
        public Task<bool> ResolveRequiredSecretsAsync(
            VendorApiConfigurationSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(true);
        }
    }

    private sealed class RecordingActivator
        : IConfigurationSnapshotActivator<VendorApiConfigurationSnapshot>
    {
        public Task ActivateAsync(
            VendorApiConfigurationSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }
    }
}
