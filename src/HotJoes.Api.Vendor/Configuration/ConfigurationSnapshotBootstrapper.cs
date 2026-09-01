namespace HotJoes.Api.Vendor.Configuration;

public sealed class ConfigurationSnapshotBootstrapper<TOptions>
    where TOptions : class
{
    private readonly IReadOnlyList<IConfigurationSnapshotReplica<TOptions>>
        _replicas;
    private readonly IConfigurationSnapshotValidator<TOptions> _validator;
    private readonly IRequiredSecretResolver<TOptions> _secretResolver;
    private readonly IConfigurationSnapshotActivator<TOptions> _activator;

    public ConfigurationSnapshotBootstrapper(
        IEnumerable<IConfigurationSnapshotReplica<TOptions>> replicas,
        IConfigurationSnapshotValidator<TOptions> validator,
        IRequiredSecretResolver<TOptions> secretResolver,
        IConfigurationSnapshotActivator<TOptions> activator)
    {
        ArgumentNullException.ThrowIfNull(replicas);
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(secretResolver);
        ArgumentNullException.ThrowIfNull(activator);

        IConfigurationSnapshotReplica<TOptions>[] replicaArray =
            replicas.ToArray();

        if (replicaArray.Length == 0)
        {
            throw new ArgumentException(
                "At least one authoritative configuration replica is required.",
                nameof(replicas));
        }

        if (replicaArray.Any(replica => replica is null))
        {
            throw new ArgumentException(
                "An authoritative configuration replica cannot be null.",
                nameof(replicas));
        }

        _replicas = replicaArray;
        _validator = validator;
        _secretResolver = secretResolver;
        _activator = activator;
    }

    public async Task<ConfigurationBootstrapResult<TOptions>> BootstrapAsync(
        CancellationToken cancellationToken = default)
    {
        foreach (IConfigurationSnapshotReplica<TOptions> replica in _replicas)
        {
            cancellationToken.ThrowIfCancellationRequested();

            TOptions snapshot;

            try
            {
                snapshot = await replica.LoadAsync(cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception)
            {
                continue;
            }

            if (!_validator.IsValid(snapshot))
            {
                continue;
            }

            bool requiredSecretsResolved =
                await _secretResolver.ResolveRequiredSecretsAsync(
                    snapshot,
                    cancellationToken);

            if (!requiredSecretsResolved)
            {
                continue;
            }

            await _activator.ActivateAsync(snapshot, cancellationToken);

            return new ConfigurationBootstrapResult<TOptions>(
                isReady: true,
                snapshot: snapshot,
                authoritativeReplicaName: replica.Name);
        }

        return new ConfigurationBootstrapResult<TOptions>(
            isReady: false,
            snapshot: null,
            authoritativeReplicaName: null);
    }
}
