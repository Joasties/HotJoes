using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorServiceAddressFailureTests
{
    [Theory]
    [MemberData(nameof(AddressFailures))]
    public async Task RegisterAsync_WhenAddressResolutionFails_ReturnsFailureWithoutDeterminationOrFirstProcessing(
        AddressResolutionResult addressResult,
        Type expectedResultType)
    {
        var resolver = new RecordingAddressResolver(addressResult);
        var determiner = new ProhibitedDeterminer();
        var newVendorProcessor = new ProhibitedNewVendorRegistrationProcessor();
        var service = new RegisterVendorService(
            new AddressResolutionInvoker(resolver),
            determiner,
            newVendorProcessor);
        RegisterVendorCommand command = CreateCommand();

        RegisterVendorResult result = await service.RegisterAsync(command);

        Assert.IsType(expectedResultType, result);
        Assert.Equal(1, resolver.CallCount);
        Assert.Equal(
            command.AddressResolutionReference,
            resolver.AddressResolutionReference);
        Assert.Equal(command.TradingLocation, resolver.TradingLocation);
        Assert.Equal(0, determiner.CallCount);
        Assert.Equal(0, newVendorProcessor.CallCount);
    }

    public static TheoryData<AddressResolutionResult, Type> AddressFailures =>
        new()
        {
            {
                AddressResolutionResult.ReferenceIsInvalid(),
                typeof(RegisterVendorResult.InvalidReference)
            },
            {
                AddressResolutionResult.InvalidAddress(),
                typeof(RegisterVendorResult.InvalidAddressResult)
            },
            {
                AddressResolutionResult.TemporarilyUnavailable(),
                typeof(RegisterVendorResult.AddressServiceTemporarilyUnavailable)
            }
        };

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            tradingName: "Hot Joes Greenwich",
            legalOperatorName: "Hot Joes Limited",
            legalOperatorType: LegalOperatorType.LimitedCompany,
            companyRegistrationNumber: "12345678",
            tradingLocation: TradingLocation.Stall,
            openingHoursStartTime: new TimeOnly(9, 0),
            openingHoursEndTime: new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            contactName: "Joseph Bloggs",
            contactEmail: "joe@hotjoes.example",
            contactTelephone: "020 7946 0123",
            addressResolutionReference: "address-resolution-reference-001",
            website: "https://hotjoes.example",
            businessDescription: "Hot food from our Greenwich market stall.",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private sealed class RecordingAddressResolver : IAddressResolver
    {
        private readonly AddressResolutionResult _result;

        public RecordingAddressResolver(AddressResolutionResult result)
        {
            _result = result;
        }

        public int CallCount { get; private set; }
        public string? AddressResolutionReference { get; private set; }
        public TradingLocation? TradingLocation { get; private set; }

        public AddressResolutionResult Resolve(
            string addressResolutionReference,
            TradingLocation tradingLocation)
        {
            CallCount++;
            AddressResolutionReference = addressResolutionReference;
            TradingLocation = tradingLocation;

            return _result;
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
                "Registration determination must not occur after Address failure.");
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
                "First processing must not occur after Address failure.");
        }
    }
}
