using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorServiceValidationTests
{
    [Fact]
    public async Task RegisterAsync_WhenRequestHasIndependentValidationErrors_ReturnsAllErrorsWithoutDownstreamWork()
    {
        RegisterVendorCommand command = CreateCommand();
        RegistrationValidationError[] errors =
        {
            new(
                field: nameof(RegisterVendorCommand.TradingName),
                code: RegistrationValidationErrorCode.LengthOutOfRange,
                message: "Trading Name must contain between 1 and 160 characters."),
            new(
                field: nameof(RegisterVendorCommand.ContactEmail),
                code: RegistrationValidationErrorCode.InvalidFormat,
                message: "Contact Email must use the supported email format.")
        };
        var validator = new RecordingInvalidValidator(errors);
        var addressResolver = new ProhibitedAddressResolver();
        var determiner = new ProhibitedDeterminer();
        var processor = new ProhibitedNewVendorRegistrationProcessor();
        var service = new RegisterVendorService(
            validator,
            new AddressResolutionInvoker(addressResolver),
            determiner,
            processor);

        RegisterVendorResult result = await service.RegisterAsync(command);

        var failure = Assert.IsType<RegisterVendorResult.RequestValidationFailure>(
            result);
        Assert.Equal(errors, failure.Errors);
        Assert.NotSame(errors, failure.Errors);
        Assert.Equal(1, validator.CallCount);
        Assert.Same(command, validator.Command);
        Assert.Equal(0, addressResolver.CallCount);
        Assert.Equal(0, determiner.CallCount);
        Assert.Equal(0, processor.CallCount);
    }

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            tradingName: string.Empty,
            legalOperatorName: "Hot Joes Limited",
            legalOperatorType: LegalOperatorType.LimitedCompany,
            companyRegistrationNumber: "12345678",
            tradingLocation: TradingLocation.Stall,
            openingHoursStartTime: new TimeOnly(9, 0),
            openingHoursEndTime: new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            contactName: "Joseph Bloggs",
            contactEmail: "not-an-email",
            contactTelephone: "020 7946 0123",
            addressResolutionReference: "address-resolution-reference-001",
            website: "https://hotjoes.example",
            businessDescription: "Hot food from our Greenwich market stall.",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private sealed class RecordingInvalidValidator : IRegisterVendorCommandValidator
    {
        private readonly RegistrationValidationError[] _errors;

        public RecordingInvalidValidator(
            IEnumerable<RegistrationValidationError> errors)
        {
            _errors = errors.ToArray();
        }

        public int CallCount { get; private set; }

        public RegisterVendorCommand? Command { get; private set; }

        public RegisterVendorCommandValidationResult Validate(
            RegisterVendorCommand command)
        {
            CallCount++;
            Command = command;

            return RegisterVendorCommandValidationResult.Invalid(_errors);
        }
    }

    private sealed class ProhibitedAddressResolver : IAddressResolver
    {
        public int CallCount { get; private set; }

        public AddressResolutionResult Resolve(
            string addressResolutionReference,
            TradingLocation tradingLocation)
        {
            CallCount++;
            throw new InvalidOperationException(
                "Address resolution must not occur after validation failure.");
        }
    }

    private sealed class ProhibitedDeterminer : IRegistrationOutcomeDeterminer
    {
        public int CallCount { get; private set; }

        public Task<RegistrationOutcomeDetermination> DetermineAsync(
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            CallCount++;
            throw new InvalidOperationException(
                "Registration determination must not occur after validation failure.");
        }
    }

    private sealed class ProhibitedNewVendorRegistrationProcessor
        : INewVendorRegistrationProcessor
    {
        public int CallCount { get; private set; }

        public Task<RegisterVendorResult> ProcessAsync(
            RegisterVendorCommand command,
            AddressAuthoritativeValues addressValues,
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            CallCount++;
            throw new InvalidOperationException(
                "First processing must not occur after validation failure.");
        }
    }
}
