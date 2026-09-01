using Azure.Core;
using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class AzureKeyVaultSecretVersionRetirerTests
{
    [Fact]
    public async Task AI_SEC_002_RetireCurrentVersion_DisablesExactOldVersion()
    {
        var client = new StubSecretClient(result: true);
        var credential = new StubTokenCredential();
        var factory = new RecordingClientFactory(client);
        var retirer = new AzureKeyVaultSecretVersionRetirer(
            credential,
            factory);
        SecretRotationRequest request = Request();

        bool result = await retirer.RetireCurrentVersionAsync(request);

        Assert.True(result);
        Assert.Equal(request.VaultUri, factory.VaultUri);
        Assert.Same(credential, factory.Credential);
        Assert.Equal(
            new[]
            {
                new VersionRetirementCall(
                    "vendor-api-persistence",
                    "current-version")
            },
            client.Calls);
    }

    [Fact]
    public async Task AI_SEC_002_KeyVaultRefusesRetirement_ReturnsFalse()
    {
        var retirer = new AzureKeyVaultSecretVersionRetirer(
            new StubTokenCredential(),
            new RecordingClientFactory(
                new StubSecretClient(result: false)));

        bool result = await retirer.RetireCurrentVersionAsync(Request());

        Assert.False(result);
    }

    [Fact]
    public async Task AI_SEC_002_KeyVaultProviderFails_ReturnsFalseWithoutDetail()
    {
        string providerDetail = "synthetic-" + "redaction-canary";
        var retirer = new AzureKeyVaultSecretVersionRetirer(
            new StubTokenCredential(),
            new RecordingClientFactory(
                new StubSecretClient(
                    new InvalidOperationException(providerDetail))));

        bool result = await retirer.RetireCurrentVersionAsync(Request());

        Assert.False(result);
    }

    [Fact]
    public async Task AI_SEC_002_CallerCancellation_Propagates()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var client = new StubSecretClient(result: true);
        var retirer = new AzureKeyVaultSecretVersionRetirer(
            new StubTokenCredential(),
            new RecordingClientFactory(client));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => retirer.RetireCurrentVersionAsync(
                Request(),
                cancellation.Token));

        Assert.Empty(client.Calls);
    }

    private static SecretRotationRequest Request()
    {
        return new SecretRotationRequest(
            new Uri("https://hotjoes-production.vault.azure.net"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                "vendor-api-persistence",
                "current-version"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                "vendor-api-persistence",
                "replacement-version"),
            ["vendor-api-a", "vendor-api-b"],
            SecretRotationStrategy.HealthGatedRollingReplacement);
    }

    private sealed class RecordingClientFactory
        : IAzureKeyVaultSecretClientFactory
    {
        private readonly IAzureKeyVaultSecretClient _client;

        public RecordingClientFactory(IAzureKeyVaultSecretClient client)
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
        private readonly bool _result;
        private readonly Exception? _failure;
        private readonly List<VersionRetirementCall> _calls = [];

        public StubSecretClient(bool result)
        {
            _result = result;
        }

        public StubSecretClient(Exception failure)
        {
            _failure = failure;
        }

        public IReadOnlyList<VersionRetirementCall> Calls => _calls;

        public Task<AzureKeyVaultResolvedSecret?> GetSecretVersionAsync(
            string name,
            string version,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<bool> DisableSecretVersionAsync(
            string name,
            string version,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _calls.Add(new VersionRetirementCall(name, version));

            if (_failure is not null)
            {
                return Task.FromException<bool>(_failure);
            }

            return Task.FromResult(_result);
        }
    }

    private sealed record VersionRetirementCall(
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
}
