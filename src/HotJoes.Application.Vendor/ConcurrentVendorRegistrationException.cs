namespace HotJoes.Application.Vendor;

public sealed class ConcurrentVendorRegistrationException : Exception
{
    public ConcurrentVendorRegistrationException()
        : base("Another registration committed the same Vendor identity.")
    {
    }
}
