using Azure.Core;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class AzureKeyVaultSecretVersionRetirer
    : ISecretRotationVersionRetirer
{
    private readonly TokenCredential _credential;
    private readonly IAzureKeyVaultSecretClientFactory _clientFactory;

    public AzureKeyVaultSecretVersionRetirer(
        TokenCredential credential,
        IAzureKeyVaultSecretClientFactory clientFactory)
    {
        ArgumentNullException.ThrowIfNull(credential);
        ArgumentNullException.ThrowIfNull(clientFactory);

        _credential = credential;
        _clientFactory = clientFactory;
    }

    public async Task<bool> RetireCurrentVersionAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            IAzureKeyVaultSecretClient client = _clientFactory.Create(
                request.VaultUri,
                _credential);

            return await client.DisableSecretVersionAsync(
                request.Current.Name,
                request.Current.Version,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
