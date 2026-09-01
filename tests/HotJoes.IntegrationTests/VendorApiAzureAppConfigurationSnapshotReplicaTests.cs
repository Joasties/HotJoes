using Azure;
using Azure.Data.AppConfiguration;
using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class VendorApiAzureAppConfigurationSnapshotReplicaTests
{
    [Fact]
    public async Task LoadAsync_NamedSnapshotAvailable_BindsCompleteSnapshotOnce()
    {
        var settings = new[]
        {
            new ConfigurationSetting(
                "VendorApi:EnvironmentName",
                "production"),
            new ConfigurationSetting(
                "VendorApi:AddressServiceBaseUri",
                "https://address.internal.example")
        };
        var client = new StubConfigurationClient(settings);
        var binder = new RecordingBinder();
        var replica = new AzureAppConfigurationSnapshotReplica<
            VendorApiAzureOptions>(
                "uk-south",
                "vendor-api-production-42",
                client,
                binder);

        VendorApiAzureOptions result = await replica.LoadAsync();

        Assert.Equal("uk-south", replica.Name);
        Assert.Equal("vendor-api-production-42", client.RequestedSnapshotName);
        Assert.Equal(1, client.LoadCount);
        Assert.Equal(1, binder.BindCount);
        Assert.Equal("production", result.EnvironmentName);
        Assert.Equal(
            new Uri("https://address.internal.example"),
            result.AddressServiceBaseUri);
        Assert.Equal(
            new Dictionary<string, string?>
            {
                ["VendorApi:EnvironmentName"] = "production",
                ["VendorApi:AddressServiceBaseUri"] =
                    "https://address.internal.example"
            },
            binder.LastSettings);
    }

    [Fact]
    public async Task LoadAsync_AzureRetrievalFails_PropagatesFailureForCoordinator()
    {
        var expected = new RequestFailedException(
            503,
            "App Configuration unavailable.");
        var replica = new AzureAppConfigurationSnapshotReplica<
            VendorApiAzureOptions>(
                "uk-west",
                "vendor-api-production-42",
                new StubConfigurationClient(expected),
                new RecordingBinder());

        RequestFailedException actual =
            await Assert.ThrowsAsync<RequestFailedException>(
                () => replica.LoadAsync());

        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task LoadAsync_Cancelled_PropagatesCancellationToAzureClient()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var client = new StubConfigurationClient(
            Array.Empty<ConfigurationSetting>());
        var replica = new AzureAppConfigurationSnapshotReplica<
            VendorApiAzureOptions>(
                "uk-south",
                "vendor-api-production-42",
                client,
                new RecordingBinder());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => replica.LoadAsync(cancellation.Token));

        Assert.Equal(cancellation.Token, client.ObservedCancellationToken);
    }

    [Fact]
    public async Task LoadAsync_BindingFails_PropagatesFailureWithoutPartialOptions()
    {
        var expected = new InvalidOperationException(
            "The authoritative snapshot is incomplete.");
        var binder = new RecordingBinder(expected);
        var replica = new AzureAppConfigurationSnapshotReplica<
            VendorApiAzureOptions>(
                "uk-south",
                "vendor-api-production-42",
                new StubConfigurationClient(
                    new[]
                    {
                        new ConfigurationSetting(
                            "VendorApi:EnvironmentName",
                            "production")
                    }),
                binder);

        InvalidOperationException actual =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => replica.LoadAsync());

        Assert.Same(expected, actual);
        Assert.Equal(1, binder.BindCount);
    }

    private sealed record VendorApiAzureOptions(
        string EnvironmentName,
        Uri AddressServiceBaseUri);

    private sealed class RecordingBinder
        : IConfigurationSnapshotBinder<VendorApiAzureOptions>
    {
        private readonly Exception? _failure;

        public RecordingBinder(Exception? failure = null)
        {
            _failure = failure;
        }

        public int BindCount { get; private set; }

        public IReadOnlyDictionary<string, string?>? LastSettings
        {
            get;
            private set;
        }

        public VendorApiAzureOptions Bind(
            IReadOnlyDictionary<string, string?> settings)
        {
            BindCount++;
            LastSettings = settings;

            if (_failure is not null)
            {
                throw _failure;
            }

            return new VendorApiAzureOptions(
                settings["VendorApi:EnvironmentName"]!,
                new Uri(settings["VendorApi:AddressServiceBaseUri"]!));
        }
    }

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

        public CancellationToken ObservedCancellationToken
        {
            get;
            private set;
        }

        public int LoadCount { get; private set; }

        public override AsyncPageable<ConfigurationSetting>
            GetConfigurationSettingsForSnapshotAsync(
                string snapshotName,
                CancellationToken cancellationToken = default)
        {
            RequestedSnapshotName = snapshotName;
            ObservedCancellationToken = cancellationToken;
            LoadCount++;
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
}
