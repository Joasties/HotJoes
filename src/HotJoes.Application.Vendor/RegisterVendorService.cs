namespace HotJoes.Application.Vendor;

public sealed class RegisterVendorService
{
    private readonly AddressResolutionInvoker _addressResolutionInvoker;
    private readonly IRegistrationOutcomeDeterminer _registrationOutcomeDeterminer;
    private readonly INewVendorRegistrationProcessor _newVendorRegistrationProcessor;

    public RegisterVendorService(
        AddressResolutionInvoker addressResolutionInvoker,
        IRegistrationOutcomeDeterminer registrationOutcomeDeterminer,
        INewVendorRegistrationProcessor newVendorRegistrationProcessor)
    {
        ArgumentNullException.ThrowIfNull(addressResolutionInvoker);
        ArgumentNullException.ThrowIfNull(registrationOutcomeDeterminer);
        ArgumentNullException.ThrowIfNull(newVendorRegistrationProcessor);

        _addressResolutionInvoker = addressResolutionInvoker;
        _registrationOutcomeDeterminer = registrationOutcomeDeterminer;
        _newVendorRegistrationProcessor = newVendorRegistrationProcessor;
    }

    public async Task<RegisterVendorResult> RegisterAsync(
        RegisterVendorCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        AddressResolutionResult addressResult = _addressResolutionInvoker.Resolve(
            command.AddressResolutionReference,
            command.TradingLocation);

        return addressResult switch
        {
            AddressResolutionResult.Success success =>
                await RegisterResolvedAsync(
                    command,
                    success.Values,
                    cancellationToken),
            AddressResolutionResult.InvalidReference =>
                RegisterVendorResult.ReferenceIsInvalid(),
            AddressResolutionResult.InvalidAddressResult =>
                RegisterVendorResult.AddressResultIsInvalid(),
            AddressResolutionResult.AddressServiceTemporarilyUnavailable =>
                RegisterVendorResult.AddressServiceIsTemporarilyUnavailable(),
            _ => throw new InvalidOperationException(
                "The Address Resolution result is not supported.")
        };
    }

    private async Task<RegisterVendorResult> RegisterResolvedAsync(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        CancellationToken cancellationToken)
    {
        VendorRegistrationIdentity identity = VendorRegistrationIdentity.Create(
            command,
            addressValues);
        RegistrationSemanticFingerprint fingerprint =
            RegistrationSemanticFingerprint.Create(command, addressValues);

        RegistrationOutcomeDetermination determination =
            await _registrationOutcomeDeterminer.DetermineAsync(
                identity,
                fingerprint,
                cancellationToken);

        return determination switch
        {
            RegistrationOutcomeDetermination.EquivalentReplay replay =>
                replay.OriginalResult,
            RegistrationOutcomeDetermination.Conflict =>
                RegisterVendorResult.IdempotencyConflictDetected(),
            RegistrationOutcomeDetermination.FirstProcessing =>
                await ProcessFirstRegistrationAsync(
                    command,
                    addressValues,
                    identity,
                    fingerprint,
                    cancellationToken),
            _ => throw new InvalidOperationException(
                "The registration determination is not supported.")
        };
    }

    private async Task<RegisterVendorResult> ProcessFirstRegistrationAsync(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        VendorRegistrationIdentity identity,
        RegistrationSemanticFingerprint fingerprint,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _newVendorRegistrationProcessor.ProcessAsync(
                command,
                addressValues,
                identity,
                fingerprint,
                cancellationToken);
        }
        catch (ConcurrentVendorRegistrationException)
        {
            RegistrationOutcomeDetermination reconciliation =
                await _registrationOutcomeDeterminer.DetermineAsync(
                    identity,
                    fingerprint,
                    cancellationToken);

            return reconciliation switch
            {
                RegistrationOutcomeDetermination.EquivalentReplay replay =>
                    replay.OriginalResult,
                RegistrationOutcomeDetermination.Conflict =>
                    RegisterVendorResult.IdempotencyConflictDetected(),
                RegistrationOutcomeDetermination.FirstProcessing =>
                    RegisterVendorResult.PersistenceOrAtomicRecordingFailed(),
                _ => throw new InvalidOperationException(
                    "The registration reconciliation result is not supported.")
            };
        }
    }
}
