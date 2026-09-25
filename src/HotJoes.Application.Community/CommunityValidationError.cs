namespace HotJoes.Application.Community;

public sealed class CommunityValidationError
{
    public CommunityValidationError(
        string field,
        string code,
        string message)
    {
        Field = RequireValue(field, nameof(field));
        Code = RequireValue(code, nameof(code));
        Message = RequireValue(message, nameof(message));
    }

    public string Field { get; }

    public string Code { get; }

    public string Message { get; }

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Validation information must not be empty.",
                parameterName);
        }

        return value;
    }
}
