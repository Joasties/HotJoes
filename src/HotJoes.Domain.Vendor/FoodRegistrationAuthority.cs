namespace HotJoes.Domain.Vendor;

public sealed record FoodRegistrationAuthority
{
    public FoodRegistrationAuthority(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }
}
