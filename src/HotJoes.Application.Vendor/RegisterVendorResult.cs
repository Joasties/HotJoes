using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public abstract class RegisterVendorResult
{
    private RegisterVendorResult()
    {
    }

    public static RegisterVendorResult Succeeded(VendorId vendorId)
    {
        return new Success(vendorId);
    }

    public static RegisterVendorResult RequestValidationFailed(
        IEnumerable<RegistrationValidationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new RequestValidationFailure(errors);
    }

    public static RegisterVendorResult ReferenceIsInvalid()
    {
        return new InvalidReference();
    }

    public static RegisterVendorResult AddressResultIsInvalid()
    {
        return new InvalidAddressResult();
    }

    public static RegisterVendorResult AddressServiceIsTemporarilyUnavailable()
    {
        return new AddressServiceTemporarilyUnavailable();
    }

    public static RegisterVendorResult AggregateInvariantFailed()
    {
        return new AggregateInvariantFailure();
    }

    public static RegisterVendorResult PersistenceOrAtomicRecordingFailed()
    {
        return new PersistenceOrAtomicRecordingFailure();
    }

    public static RegisterVendorResult IdempotencyConflictDetected()
    {
        return new IdempotencyConflict();
    }

    public sealed class Success : RegisterVendorResult
    {
        internal Success(VendorId vendorId)
        {
            VendorId = vendorId;
        }

        public VendorId VendorId { get; }

        public VendorState VendorState =>
            HotJoes.Domain.Vendor.VendorState.PendingActivation;
    }

    public sealed class RequestValidationFailure : RegisterVendorResult
    {
        internal RequestValidationFailure(
            IEnumerable<RegistrationValidationError> errors)
        {
            RegistrationValidationError[] copiedErrors = errors.ToArray();

            if (copiedErrors.Length == 0)
            {
                throw new ArgumentException(
                    "At least one validation error is required.",
                    nameof(errors));
            }

            if (copiedErrors.Any(error => error is null))
            {
                throw new ArgumentException(
                    "Validation errors cannot contain null.",
                    nameof(errors));
            }

            Errors = Array.AsReadOnly(copiedErrors);
        }

        public IReadOnlyList<RegistrationValidationError> Errors { get; }
    }

    public sealed class InvalidReference : RegisterVendorResult
    {
        internal InvalidReference()
        {
        }
    }

    public sealed class InvalidAddressResult : RegisterVendorResult
    {
        internal InvalidAddressResult()
        {
        }
    }

    public sealed class AddressServiceTemporarilyUnavailable : RegisterVendorResult
    {
        internal AddressServiceTemporarilyUnavailable()
        {
        }
    }

    public sealed class AggregateInvariantFailure : RegisterVendorResult
    {
        internal AggregateInvariantFailure()
        {
        }
    }

    public sealed class PersistenceOrAtomicRecordingFailure : RegisterVendorResult
    {
        internal PersistenceOrAtomicRecordingFailure()
        {
        }
    }

    public sealed class IdempotencyConflict : RegisterVendorResult
    {
        internal IdempotencyConflict()
        {
        }
    }
}
