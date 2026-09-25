namespace HotJoes.Application.Vendor;

public abstract class DetermineRequiredLicenceTypesRequestValidationResult
{
    private DetermineRequiredLicenceTypesRequestValidationResult()
    {
    }

    public static DetermineRequiredLicenceTypesRequestValidationResult Accept(
        DetermineRequiredLicenceTypesRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new Accepted(request);
    }

    public static DetermineRequiredLicenceTypesRequestValidationResult Reject(
        IEnumerable<RegistrationValidationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        RegistrationValidationError[] copiedErrors = errors.ToArray();

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

    public sealed class Accepted
        : DetermineRequiredLicenceTypesRequestValidationResult
    {
        internal Accepted(DetermineRequiredLicenceTypesRequest request)
        {
            Request = request;
        }

        public DetermineRequiredLicenceTypesRequest Request { get; }
    }

    public sealed class Invalid
        : DetermineRequiredLicenceTypesRequestValidationResult
    {
        internal Invalid(IEnumerable<RegistrationValidationError> errors)
        {
            Errors = Array.AsReadOnly(errors.ToArray());
        }

        public IReadOnlyList<RegistrationValidationError> Errors { get; }
    }
}
