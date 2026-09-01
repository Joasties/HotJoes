namespace HotJoes.Api.Vendor.Configuration;

public sealed class SecretRotationCoordinator
{
    private readonly ISecretRotationCandidateValidator _candidateValidator;
    private readonly ISecretRotationOverlapCoordinator _overlapCoordinator;
    private readonly ISecretRotationReferencePublisher _referencePublisher;
    private readonly ISecretRotationConsumerCutover _consumerCutover;
    private readonly ISecretRotationHealthGate _healthGate;
    private readonly ISecretRotationCredentialRevoker _credentialRevoker;
    private readonly ISecretRotationVersionRetirer _versionRetirer;

    public SecretRotationCoordinator(
        ISecretRotationCandidateValidator candidateValidator,
        ISecretRotationOverlapCoordinator overlapCoordinator,
        ISecretRotationReferencePublisher referencePublisher,
        ISecretRotationConsumerCutover consumerCutover,
        ISecretRotationHealthGate healthGate,
        ISecretRotationCredentialRevoker credentialRevoker,
        ISecretRotationVersionRetirer versionRetirer)
    {
        ArgumentNullException.ThrowIfNull(candidateValidator);
        ArgumentNullException.ThrowIfNull(overlapCoordinator);
        ArgumentNullException.ThrowIfNull(referencePublisher);
        ArgumentNullException.ThrowIfNull(consumerCutover);
        ArgumentNullException.ThrowIfNull(healthGate);
        ArgumentNullException.ThrowIfNull(credentialRevoker);
        ArgumentNullException.ThrowIfNull(versionRetirer);

        _candidateValidator = candidateValidator;
        _overlapCoordinator = overlapCoordinator;
        _referencePublisher = referencePublisher;
        _consumerCutover = consumerCutover;
        _healthGate = healthGate;
        _credentialRevoker = credentialRevoker;
        _versionRetirer = versionRetirer;
    }

    public async Task<SecretRotationResult> RotateAsync(
        SecretRotationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsValid(request))
        {
            return Result(SecretRotationResultKind.InvalidRequest);
        }

        bool publicationAttempted = false;
        SecretRotationPhase phase =
            SecretRotationPhase.ReplacementValidation;

        try
        {
            if (!await _consumerCutover.IsStrategyVerifiedAsync(
                    request,
                    cancellationToken))
            {
                return Result(
                    SecretRotationResultKind.CutoverStrategyNotVerified);
            }

            if (!await _candidateValidator.ValidateReplacementAsync(
                    request,
                    cancellationToken))
            {
                return Result(
                    SecretRotationResultKind.ReplacementValidationFailed);
            }

            phase = SecretRotationPhase.OverlapEstablishment;

            if (!await _overlapCoordinator.EstablishOverlapAsync(
                    request,
                    cancellationToken))
            {
                return Result(SecretRotationResultKind.OverlapUnavailable);
            }

            phase = SecretRotationPhase.ReferencePublication;
            publicationAttempted = true;

            if (!await _referencePublisher.PublishReplacementAsync(
                    request,
                    cancellationToken))
            {
                await RestoreCurrentSafelyAsync(request);
                return Result(
                    SecretRotationResultKind.ReferencePublicationFailed);
            }

            phase = SecretRotationPhase.ConsumerCutover;

            if (!await _consumerCutover.CutOverAsync(
                    request,
                    cancellationToken))
            {
                await RestoreCurrentSafelyAsync(request);
                return Result(
                    SecretRotationResultKind.ConsumerCutoverFailed);
            }

            phase = SecretRotationPhase.HealthAndDrainConfirmation;

            if (!await _healthGate.ConfirmHealthyDrainedAndDurableAsync(
                    request,
                    cancellationToken))
            {
                await RestoreCurrentSafelyAsync(request);
                return Result(
                    SecretRotationResultKind.HealthAndDrainFailed);
            }
        }
        catch (OperationCanceledException)
        {
            if (publicationAttempted)
            {
                await RestoreCurrentSafelyAsync(request);
            }

            throw;
        }
        catch (Exception)
        {
            if (publicationAttempted)
            {
                await RestoreCurrentSafelyAsync(request);
            }

            return Result(FailureKind(phase));
        }

        try
        {
            if (!await _credentialRevoker.RevokeCurrentCredentialAsync(
                    request,
                    cancellationToken))
            {
                return Result(
                    SecretRotationResultKind
                        .ProtectedResourceRevocationFailed);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result(
                SecretRotationResultKind.ProtectedResourceRevocationFailed);
        }

        try
        {
            if (!await _versionRetirer.RetireCurrentVersionAsync(
                    request,
                    cancellationToken))
            {
                return Result(
                    SecretRotationResultKind
                        .KeyVaultVersionRetirementFailed);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result(
                SecretRotationResultKind.KeyVaultVersionRetirementFailed);
        }

        return Result(SecretRotationResultKind.Completed);
    }

    private async Task RestoreCurrentSafelyAsync(
        SecretRotationRequest request)
    {
        try
        {
            await _referencePublisher.RestoreCurrentAsync(
                request,
                CancellationToken.None);
        }
        catch (Exception)
        {
            // The closed rotation result remains value-free. Operational
            // adapters own alerting for a failed restoration attempt.
        }
    }

    private static bool IsValid(SecretRotationRequest? request)
    {
        return request is not null &&
            IsAbsoluteHttpsUri(request.VaultUri) &&
            IsValidReference(request.Current) &&
            IsValidReference(request.Replacement) &&
            string.Equals(
                request.Current.Purpose,
                request.Replacement.Purpose,
                StringComparison.Ordinal) &&
            string.Equals(
                request.Current.Name,
                request.Replacement.Name,
                StringComparison.Ordinal) &&
            !string.Equals(
                request.Current.Version,
                request.Replacement.Version,
                StringComparison.Ordinal) &&
            request.ConsumerNames is not null &&
            request.ConsumerNames.Count > 0 &&
            request.ConsumerNames.All(name =>
                !string.IsNullOrWhiteSpace(name)) &&
            request.ConsumerNames.Distinct(
                StringComparer.OrdinalIgnoreCase).Count() ==
                request.ConsumerNames.Count &&
            Enum.IsDefined(request.Strategy);
    }

    private static bool IsValidReference(
        RequiredSecretReference? reference)
    {
        return reference is not null &&
            !string.IsNullOrWhiteSpace(reference.Purpose) &&
            !string.IsNullOrWhiteSpace(reference.Name) &&
            !string.IsNullOrWhiteSpace(reference.Version);
    }

    private static bool IsAbsoluteHttpsUri(Uri? uri)
    {
        return uri is not null &&
            uri.IsAbsoluteUri &&
            string.Equals(
                uri.Scheme,
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(uri.Host);
    }

    private static SecretRotationResultKind FailureKind(
        SecretRotationPhase phase)
    {
        return phase switch
        {
            SecretRotationPhase.ReplacementValidation =>
                SecretRotationResultKind.ReplacementValidationFailed,
            SecretRotationPhase.OverlapEstablishment =>
                SecretRotationResultKind.OverlapUnavailable,
            SecretRotationPhase.ReferencePublication =>
                SecretRotationResultKind.ReferencePublicationFailed,
            SecretRotationPhase.ConsumerCutover =>
                SecretRotationResultKind.ConsumerCutoverFailed,
            SecretRotationPhase.HealthAndDrainConfirmation =>
                SecretRotationResultKind.HealthAndDrainFailed,
            _ => throw new ArgumentOutOfRangeException(nameof(phase))
        };
    }

    private static SecretRotationResult Result(
        SecretRotationResultKind kind)
    {
        return new SecretRotationResult(kind);
    }
}
