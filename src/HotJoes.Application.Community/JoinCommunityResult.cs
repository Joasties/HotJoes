namespace HotJoes.Application.Community;

public abstract class JoinCommunityResult
{
    private JoinCommunityResult()
    {
    }

    public static JoinCommunityResult Recorded(
        CommunityParticipation participation)
    {
        ArgumentNullException.ThrowIfNull(participation);
        return new CommunityParticipationRecorded(participation);
    }

    public static JoinCommunityResult AlreadyRecorded(
        CommunityParticipation participation)
    {
        ArgumentNullException.ThrowIfNull(participation);
        return new CommunityParticipationAlreadyRecorded(participation);
    }

    public static JoinCommunityResult ValidationFailed(
        IEnumerable<CommunityValidationError> errors) =>
        new RequestValidationFailure(errors);

    public static JoinCommunityResult VendorWasNotFound() =>
        new VendorNotFound();

    public static JoinCommunityResult Conflict() =>
        new CommunityParticipationConflict();

    public static JoinCommunityResult VerificationIsUnavailable() =>
        new VendorVerificationUnavailable();

    public static JoinCommunityResult PersistenceIsUnavailable() =>
        new CommunityPersistenceUnavailable();

    public sealed class CommunityParticipationRecorded
        : JoinCommunityResult
    {
        internal CommunityParticipationRecorded(
            CommunityParticipation participation)
        {
            Participation = participation;
        }

        public CommunityParticipation Participation { get; }
    }

    public sealed class CommunityParticipationAlreadyRecorded
        : JoinCommunityResult
    {
        internal CommunityParticipationAlreadyRecorded(
            CommunityParticipation participation)
        {
            Participation = participation;
        }

        public CommunityParticipation Participation { get; }
    }

    public sealed class RequestValidationFailure : JoinCommunityResult
    {
        internal RequestValidationFailure(
            IEnumerable<CommunityValidationError> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);
            CommunityValidationError[] copiedErrors = errors.ToArray();

            if (copiedErrors.Length == 0)
            {
                throw new ArgumentException(
                    "At least one validation error is required.",
                    nameof(errors));
            }

            if (copiedErrors.Any(error => error is null))
            {
                throw new ArgumentException(
                    "Validation errors cannot contain null.",
                    nameof(errors));
            }

            Errors = Array.AsReadOnly(copiedErrors);
        }

        public IReadOnlyList<CommunityValidationError> Errors { get; }
    }

    public sealed class VendorNotFound : JoinCommunityResult
    {
        internal VendorNotFound()
        {
        }
    }

    public sealed class CommunityParticipationConflict : JoinCommunityResult
    {
        internal CommunityParticipationConflict()
        {
        }
    }

    public sealed class VendorVerificationUnavailable : JoinCommunityResult
    {
        internal VendorVerificationUnavailable()
        {
        }
    }

    public sealed class CommunityPersistenceUnavailable : JoinCommunityResult
    {
        internal CommunityPersistenceUnavailable()
        {
        }
    }
}
