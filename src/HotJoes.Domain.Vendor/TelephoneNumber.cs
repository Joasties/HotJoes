namespace HotJoes.Domain.Vendor;

public sealed record TelephoneNumber
{
    public TelephoneNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }
}
