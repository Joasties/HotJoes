namespace HotJoes.Application.Vendor;

public sealed record RegistrationValidationError
{
    public RegistrationValidationError(
        string field,
        RegistrationValidationErrorCode code,
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        if (!Enum.IsDefined(code))
        {
            throw new ArgumentOutOfRangeException(nameof(code));
        }

        Field = field;
        Code = code;
        Message = message;
    }

    public string Field { get; }

    public RegistrationValidationErrorCode Code { get; }

    public string Message { get; }
}
