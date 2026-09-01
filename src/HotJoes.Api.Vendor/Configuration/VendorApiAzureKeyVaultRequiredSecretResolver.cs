using Azure.Core;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class VendorApiAzureKeyVaultRequiredSecretResolver
    : IRequiredSecretResolver<VendorApiConfigurationSnapshot>
{
    private readonly TokenCredential _credential;
    private readonly IAzureKeyVaultSecretClientFactory _clientFactory;

    public VendorApiAzureKeyVaultRequiredSecretResolver(
        TokenCredential credential,
        IAzureKeyVaultSecretClientFactory clientFactory)
    {
        ArgumentNullException.ThrowIfNull(credential);
        ArgumentNullException.ThrowIfNull(clientFactory);

        _credential = credential;
        _clientFactory = clientFactory;
    }

    public async Task<bool> ResolveRequiredSecretsAsync(
        VendorApiConfigurationSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        cancellationToken.ThrowIfCancellationRequested();

        RequiredSecretReference reference =
            snapshot.PersistenceConnectionSecretReference;

        if (!IsValidReference(reference))
        {
            return false;
        }

        try
        {
            IAzureKeyVaultSecretClient client = _clientFactory.Create(
                snapshot.KeyVaultUri,
                _credential);
            AzureKeyVaultResolvedSecret? resolved =
                await client.GetSecretVersionAsync(
                    reference.Name,
                    reference.Version,
                    cancellationToken);

            return resolved is not null &&
                resolved.IsEnabled &&
                !string.IsNullOrWhiteSpace(resolved.Value) &&
                string.Equals(
                    resolved.Name,
                    reference.Name,
                    StringComparison.Ordinal) &&
                string.Equals(
                    resolved.Version,
                    reference.Version,
                    StringComparison.Ordinal);
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

    private static bool IsValidReference(
        RequiredSecretReference? reference)
    {
        return reference is not null &&
            !string.IsNullOrWhiteSpace(reference.Purpose) &&
            !string.IsNullOrWhiteSpace(reference.Name) &&
            !string.IsNullOrWhiteSpace(reference.Version);
    }
}
