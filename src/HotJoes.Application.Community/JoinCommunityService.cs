namespace HotJoes.Application.Community;

public sealed class JoinCommunityService : IJoinCommunityService
{
    private readonly JoinCommunityRequestValidator _validator;
    private readonly IVendorRegistrationVerificationPort _verification;
    private readonly ICommunityParticipationCommitter _committer;

    public JoinCommunityService(
        JoinCommunityRequestValidator validator,
        IVendorRegistrationVerificationPort verification,
        ICommunityParticipationCommitter committer)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(verification);
        ArgumentNullException.ThrowIfNull(committer);

        _validator = validator;
        _verification = verification;
        _committer = committer;
    }

    public async Task<JoinCommunityResult> JoinAsync(
        JoinCommunityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        JoinCommunityRequestValidationResult validation =
            _validator.Validate(request);

        if (validation is
            JoinCommunityRequestValidationResult.Invalid invalid)
        {
            return JoinCommunityResult.ValidationFailed(invalid.Errors);
        }

        var accepted = (JoinCommunityRequestValidationResult.Accepted)
            validation;
        return await JoinValidatedAsync(
            accepted.Request,
            cancellationToken);
    }

    private async Task<JoinCommunityResult> JoinValidatedAsync(
        JoinCommunityRequest request,
        CancellationToken cancellationToken)
    {
        VendorRegistrationVerificationResult verificationResult =
            await _verification.VerifyAsync(
                request.VendorId,
                cancellationToken);

        return verificationResult switch
        {
            VendorRegistrationVerificationResult.SuccessfullyRegistered =>
                await CommitAsync(request, cancellationToken),
            VendorRegistrationVerificationResult.VendorNotFound =>
                JoinCommunityResult.VendorWasNotFound(),
            VendorRegistrationVerificationResult.VerificationUnavailable =>
                JoinCommunityResult.VerificationIsUnavailable(),
            _ => throw new InvalidOperationException(
                "The Vendor registration verification result is not supported.")
        };
    }

    private async Task<JoinCommunityResult> CommitAsync(
        JoinCommunityRequest request,
        CancellationToken cancellationToken)
    {
        CommunityParticipationCommitResult commitResult =
            await _committer.CommitAsync(request, cancellationToken);

        return commitResult switch
        {
            CommunityParticipationCommitResult.FirstRecorded recorded =>
                JoinCommunityResult.Recorded(recorded.Participation),
            CommunityParticipationCommitResult.EquivalentReplay replay =>
                JoinCommunityResult.AlreadyRecorded(replay.Participation),
            CommunityParticipationCommitResult.PreferenceConflict =>
                JoinCommunityResult.Conflict(),
            CommunityParticipationCommitResult.PersistenceUnavailable =>
                JoinCommunityResult.PersistenceIsUnavailable(),
            _ => throw new InvalidOperationException(
                "The Community Participation commit result is not supported.")
        };
    }
}
