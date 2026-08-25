namespace HotJoes.Application.Address;

public abstract record AddressResolutionResult
{
    private AddressResolutionResult()
    {
    }

    public static AddressResolutionResult Succeeded(CompleteAddressResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new Success(result);
    }

    public static AddressResolutionResult ReferenceIsInvalid()
    {
        return new InvalidReference();
    }

    public static AddressResolutionResult InvalidAddress()
    {
        return new InvalidAddressResult();
    }

    public static AddressResolutionResult FailedTechnically(
        AddressTechnicalFailure failure)
    {
        return new TechnicalFailure(failure);
    }

    public sealed record Success : AddressResolutionResult
    {
        internal Success(CompleteAddressResult result)
        {
            Result = result;
        }

        public CompleteAddressResult Result { get; }
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

    public sealed record TechnicalFailure : AddressResolutionResult
    {
        internal TechnicalFailure(AddressTechnicalFailure failure)
        {
            Failure = failure;
        }

        public AddressTechnicalFailure Failure { get; }
    }
}
