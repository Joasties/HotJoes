namespace HotJoes.Application.Vendor;

public abstract class DetermineRequiredLicenceTypesResult
{
    private DetermineRequiredLicenceTypesResult()
    {
    }

    public static DetermineRequiredLicenceTypesResult Succeeded(
        ComplianceDetermination determination)
    {
        ArgumentNullException.ThrowIfNull(determination);
        return new Success(determination);
    }

    public static DetermineRequiredLicenceTypesResult ValidationFailed(
        IEnumerable<RegistrationValidationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new DeterminationRequestValidationFailure(errors);
    }

    public static DetermineRequiredLicenceTypesResult ReferenceIsInvalid() =>
        new InvalidReference();

    public static DetermineRequiredLicenceTypesResult AddressResultIsInvalid() =>
        new InvalidAddressResult();

    public static DetermineRequiredLicenceTypesResult AddressIsTemporarilyUnavailable() =>
        new AddressServiceTemporarilyUnavailable();

    public static DetermineRequiredLicenceTypesResult Unsupported() =>
        new UnsupportedDetermination();

    public static DetermineRequiredLicenceTypesResult ComplianceIsTemporarilyUnavailable() =>
        new ComplianceDeterminationTemporarilyUnavailable();

    public sealed class Success : DetermineRequiredLicenceTypesResult
    {
        internal Success(ComplianceDetermination determination)
        {
            Determination = determination;
        }

        public ComplianceDetermination Determination { get; }
    }

    public sealed class DeterminationRequestValidationFailure
        : DetermineRequiredLicenceTypesResult
    {
        internal DeterminationRequestValidationFailure(
            IEnumerable<RegistrationValidationError> errors)
        {
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

            Errors = Array.AsReadOnly(copiedErrors);
        }

        public IReadOnlyList<RegistrationValidationError> Errors { get; }
    }

    public sealed class InvalidReference : DetermineRequiredLicenceTypesResult
    {
        internal InvalidReference()
        {
        }
    }

    public sealed class InvalidAddressResult
        : DetermineRequiredLicenceTypesResult
    {
        internal InvalidAddressResult()
        {
        }
    }

    public sealed class AddressServiceTemporarilyUnavailable
        : DetermineRequiredLicenceTypesResult
    {
        internal AddressServiceTemporarilyUnavailable()
        {
        }
    }

    public sealed class UnsupportedDetermination
        : DetermineRequiredLicenceTypesResult
    {
        internal UnsupportedDetermination()
        {
        }
    }

    public sealed class ComplianceDeterminationTemporarilyUnavailable
        : DetermineRequiredLicenceTypesResult
    {
        internal ComplianceDeterminationTemporarilyUnavailable()
        {
        }
    }
}
