using HotJoes.Application.Community;

namespace HotJoes.Api.Vendor;

public sealed class CommunityApiErrorMapper
{
    public VendorApiErrorMapping Map(JoinCommunityResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result switch
        {
            JoinCommunityResult.RequestValidationFailure failure =>
                ValidationFailure(failure),
            JoinCommunityResult.VendorNotFound => Create(
                StatusCodes.Status404NotFound,
                "vendorNotFound",
                "The registered Vendor was not found."),
            JoinCommunityResult.CommunityParticipationConflict => Create(
                StatusCodes.Status409Conflict,
                "communityParticipationConflict",
                "Community Participation already exists with a different Contact Preference."),
            JoinCommunityResult.VendorVerificationUnavailable => Create(
                StatusCodes.Status503ServiceUnavailable,
                "vendorVerificationUnavailable",
                "Vendor verification is temporarily unavailable. The request may be retried."),
            JoinCommunityResult.CommunityPersistenceUnavailable => Create(
                StatusCodes.Status503ServiceUnavailable,
                "communityPersistenceUnavailable",
                "Community Participation could not be recorded. The request may be retried."),
            JoinCommunityResult.CommunityParticipationRecorded =>
                throw SuccessfulResult(result),
            JoinCommunityResult.CommunityParticipationAlreadyRecorded =>
                throw SuccessfulResult(result),
            _ => throw new ArgumentOutOfRangeException(
                nameof(result),
                result.GetType().FullName,
                "The Join Community result is not an approved controlled failure.")
        };
    }

    private static VendorApiErrorMapping ValidationFailure(
        JoinCommunityResult.RequestValidationFailure failure)
    {
        VendorApiValidationErrorResponse[] errors = failure.Errors
            .Select(error => new VendorApiValidationErrorResponse(
                MapField(error.Field),
                error.Code,
                error.Message))
            .ToArray();

        return new VendorApiErrorMapping(
            StatusCodes.Status400BadRequest,
            new VendorApiErrorResponse(
                "communityParticipationValidationFailed",
                "Community Participation could not be recorded because supplied information is invalid.",
                Array.AsReadOnly(errors)));
    }

    private static string MapField(string field) => field switch
    {
        nameof(HotJoes.Application.Community.JoinCommunityRequest.VendorId) =>
            "vendorId",
        nameof(HotJoes.Application.Community.JoinCommunityRequest.ContactPreference) =>
            "contactPreference",
        _ when field.Length > 0 && char.IsLower(field[0]) => field,
        _ => throw new ArgumentOutOfRangeException(nameof(field), field,
            "The Community validation field has no approved API JSON path.")
    };

    private static VendorApiErrorMapping Create(
        int statusCode,
        string code,
        string message) =>
        new(
            statusCode,
            new VendorApiErrorResponse(code, message, ValidationErrors: null));

    private static ArgumentException SuccessfulResult(
        JoinCommunityResult result) =>
        new(
            "Successful Join Community results are not error mappings.",
            nameof(result));
}
