namespace HotJoes.Application.Vendor;

public abstract record AddressResolutionResult
{
    private AddressResolutionResult()
    {
    }

    public static AddressResolutionResult Succeeded(AddressAuthoritativeValues values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return new Success(values);
    }

    public static AddressResolutionResult ReferenceIsInvalid()
    {
        return new InvalidReference();
    }

    public static AddressResolutionResult InvalidAddress()
    {
        return new InvalidAddressResult();
    }

    public static AddressResolutionResult TemporarilyUnavailable()
    {
        return new AddressServiceTemporarilyUnavailable();
    }

    public sealed record Success : AddressResolutionResult
    {
        internal Success(AddressAuthoritativeValues values)
        {
            Values = values;
        }

        public AddressAuthoritativeValues Values { get; }
    }

    public sealed record InvalidReference : AddressResolutionResult
    {
        internal InvalidReference()
        {
        }
    }

    public sealed record InvalidAddressResult : AddressResolutionResult
    {
        internal InvalidAddressResult()
        {
        }
    }

    public sealed record AddressServiceTemporarilyUnavailable : AddressResolutionResult
    {
        internal AddressServiceTemporarilyUnavailable()
        {
        }
    }
}
