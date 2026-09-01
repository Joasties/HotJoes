namespace HotJoes.Api.Vendor.Configuration;

public sealed class ConfigurationSnapshotRefreshCoordinator<TOptions>
    where TOptions : class
{
    private readonly IConfigurationSnapshotValidator<TOptions> _validator;
    private readonly IConfigurationSnapshotChangeClassifier<TOptions>
        _changeClassifier;
    private readonly IConfigurationSnapshotActivator<TOptions> _activator;
    private TOptions _currentSnapshot;

    public ConfigurationSnapshotRefreshCoordinator(
        TOptions currentSnapshot,
        IConfigurationSnapshotValidator<TOptions> validator,
        IConfigurationSnapshotChangeClassifier<TOptions> changeClassifier,
        IConfigurationSnapshotActivator<TOptions> activator)
    {
        ArgumentNullException.ThrowIfNull(currentSnapshot);
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(changeClassifier);
        ArgumentNullException.ThrowIfNull(activator);

        _currentSnapshot = currentSnapshot;
        _validator = validator;
        _changeClassifier = changeClassifier;
        _activator = activator;
    }

    public TOptions CurrentSnapshot => Volatile.Read(ref _currentSnapshot);

    public async Task<bool> RefreshAsync(
        IConfigurationSnapshotReplica<TOptions> replica,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replica);
        cancellationToken.ThrowIfCancellationRequested();

        TOptions candidateSnapshot;

        try
        {
            candidateSnapshot = await replica.LoadAsync(cancellationToken);
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

        if (!_validator.IsValid(candidateSnapshot))
        {
            return false;
        }

        TOptions currentSnapshot = CurrentSnapshot;
        ConfigurationChangeClassification classification =
            _changeClassifier.Classify(
                currentSnapshot,
                candidateSnapshot);

        if (classification != ConfigurationChangeClassification.ReloadSafe)
        {
            return false;
        }

        await _activator.ActivateAsync(
            candidateSnapshot,
            cancellationToken);
        Volatile.Write(ref _currentSnapshot, candidateSnapshot);

        return true;
    }
}
