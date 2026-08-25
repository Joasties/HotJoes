namespace HotJoes.Domain.Vendor;

public sealed record CanonicalAddressId
{
    public CanonicalAddressId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }
}
