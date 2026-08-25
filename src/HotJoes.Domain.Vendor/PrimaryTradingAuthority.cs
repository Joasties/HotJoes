namespace HotJoes.Domain.Vendor;

public sealed record PrimaryTradingAuthority
{
    public PrimaryTradingAuthority(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }
}
