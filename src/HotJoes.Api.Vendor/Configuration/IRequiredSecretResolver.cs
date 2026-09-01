namespace HotJoes.Api.Vendor.Configuration;

public interface IRequiredSecretResolver<TOptions>
    where TOptions : class
{
    Task<bool> ResolveRequiredSecretsAsync(
        TOptions snapshot,
        CancellationToken cancellationToken = default);
}
