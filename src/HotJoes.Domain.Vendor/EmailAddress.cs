namespace HotJoes.Domain.Vendor;

public sealed record EmailAddress
{
    public EmailAddress(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!value.Contains('@') || value.StartsWith('@') || value.EndsWith('@'))
        {
            throw new ArgumentException("Invalid email address.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }
}
