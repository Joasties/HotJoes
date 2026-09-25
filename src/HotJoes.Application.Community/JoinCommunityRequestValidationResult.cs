namespace HotJoes.Application.Community;

public abstract class JoinCommunityRequestValidationResult
{
    private JoinCommunityRequestValidationResult()
    {
    }

    public static JoinCommunityRequestValidationResult Accept(
        JoinCommunityRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new Accepted(request);
    }

    public static JoinCommunityRequestValidationResult Reject(
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

        return new Invalid(copiedErrors);
    }

    public sealed class Accepted : JoinCommunityRequestValidationResult
    {
        internal Accepted(JoinCommunityRequest request)
        {
            Request = request;
        }

        public JoinCommunityRequest Request { get; }
    }

    public sealed class Invalid : JoinCommunityRequestValidationResult
    {
        internal Invalid(IEnumerable<CommunityValidationError> errors)
        {
            Errors = Array.AsReadOnly(errors.ToArray());
        }

        public IReadOnlyList<CommunityValidationError> Errors { get; }
    }
}
