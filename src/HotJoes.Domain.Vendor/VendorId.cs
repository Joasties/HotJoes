namespace HotJoes.Domain.Vendor;

public readonly record struct VendorId
{
    public VendorId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("VendorId cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }
}
