namespace HotJoes.Application.Vendor;

public abstract class RegisterVendorCommandValidationResult
{
    private RegisterVendorCommandValidationResult()
    {
    }

    public static RegisterVendorCommandValidationResult Accepted(
        RegisterVendorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return new Success(command);
    }

    public static RegisterVendorCommandValidationResult Invalid(
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

        return new Failure(copiedErrors);
    }

    public sealed class Success : RegisterVendorCommandValidationResult
    {
        internal Success(RegisterVendorCommand command)
        {
            Command = command;
        }

        public RegisterVendorCommand Command { get; }
    }

    public sealed class Failure : RegisterVendorCommandValidationResult
    {
        internal Failure(IEnumerable<RegistrationValidationError> errors)
        {
            Errors = Array.AsReadOnly(errors.ToArray());
        }

        public IReadOnlyList<RegistrationValidationError> Errors { get; }
    }
}
