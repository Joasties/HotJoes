namespace HotJoes.Application.Vendor;

public abstract class RetrieveRegisteredVendorResult
{
    private RetrieveRegisteredVendorResult()
    {
    }

    public static RetrieveRegisteredVendorResult VendorFound(
        RegisteredVendorDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);
        return new Found(details);
    }

    public static RetrieveRegisteredVendorResult VendorNotFound()
    {
        return new NotFound();
    }

    public sealed class Found : RetrieveRegisteredVendorResult
    {
        internal Found(RegisteredVendorDetails details)
        {
            Details = details;
        }

        public RegisteredVendorDetails Details { get; }
    }

    public sealed class NotFound : RetrieveRegisteredVendorResult
    {
        internal NotFound()
        {
        }
    }
}
