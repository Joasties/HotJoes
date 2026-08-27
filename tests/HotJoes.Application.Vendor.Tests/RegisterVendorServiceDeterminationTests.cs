using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorServiceDeterminationTests
{
    [Fact]
    public async Task RegisterAsync_WhenEquivalentReplay_ReturnsOriginalResultWithoutFirstProcessing()
    {
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        var originalResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(
                new VendorId(new Guid("2fbefb02-5589-49fc-aa24-28b04789c7c9"))));
        var determiner = new RecordingDeterminer(
            RegistrationOutcomeDetermination.Replay(originalResult));
        var newVendorProcessor = new RecordingNewVendorRegistrationProcessor(
            RegisterVendorResult.PersistenceOrAtomicRecordingFailed());
        var service = CreateService(addressValues, determiner, newVendorProcessor);

        RegisterVendorResult result = await service.RegisterAsync(command);

        Assert.Same(originalResult, result);
        AssertDerivedDetermination(command, addressValues, determiner);
        Assert.Equal(0, newVendorProcessor.CallCount);
    }

    [Fact]
    public async Task RegisterAsync_WhenRegistrationConflicts_ReturnsConflictWithoutFirstProcessing()
    {
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        var determiner = new RecordingDeterminer(
            RegistrationOutcomeDetermination.ConflictDetected());
        var newVendorProcessor = new RecordingNewVendorRegistrationProcessor(
            RegisterVendorResult.PersistenceOrAtomicRecordingFailed());
        var service = CreateService(addressValues, determiner, newVendorProcessor);

        RegisterVendorResult result = await service.RegisterAsync(command);

        Assert.IsType<RegisterVendorResult.IdempotencyConflict>(result);
        AssertDerivedDetermination(command, addressValues, determiner);
        Assert.Equal(0, newVendorProcessor.CallCount);
    }

    [Fact]
    public async Task RegisterAsync_WhenFirstProcessingIsRequired_DelegatesResolvedRegistrationOnce()
    {
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        var determiner = new RecordingDeterminer(
            RegistrationOutcomeDetermination.FirstProcessingRequired());
        var expectedResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(
                new VendorId(new Guid("0d166d6e-f4fd-443e-b814-d953d575b375"))));
        var newVendorProcessor = new RecordingNewVendorRegistrationProcessor(expectedResult);
        var service = CreateService(addressValues, determiner, newVendorProcessor);

        RegisterVendorResult result = await service.RegisterAsync(command);

        Assert.Same(expectedResult, result);
        AssertDerivedDetermination(command, addressValues, determiner);
        Assert.Equal(1, newVendorProcessor.CallCount);
        Assert.Same(command, newVendorProcessor.Command);
        Assert.Same(addressValues, newVendorProcessor.AddressValues);
        Assert.Equal(determiner.Identity, newVendorProcessor.Identity);
        Assert.Equal(determiner.Fingerprint, newVendorProcessor.Fingerprint);
    }

    private static RegisterVendorService CreateService(
        AddressAuthoritativeValues addressValues,
        IRegistrationOutcomeDeterminer determiner,
        INewVendorRegistrationProcessor newVendorProcessor)
    {
        return new RegisterVendorService(
            new AcceptingRegisterVendorCommandValidator(),
            new AddressResolutionInvoker(new SuccessfulAddressResolver(addressValues)),
            determiner,
            newVendorProcessor);
    }

    private static void AssertDerivedDetermination(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        RecordingDeterminer determiner)
    {
        Assert.Equal(1, determiner.CallCount);
        Assert.Equal(
            VendorRegistrationIdentity.Create(command, addressValues),
            determiner.Identity);
        Assert.Equal(
            RegistrationSemanticFingerprint.Create(command, addressValues),
            determiner.Fingerprint);
    }

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

    private static AddressAuthoritativeValues CreateAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId("canonical-address-001"),
            new BusinessAddressSnapshot(
                "2 High Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null,
                "Hot Joes Limited"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
    }

    private sealed class SuccessfulAddressResolver : IAddressResolver
    {
        private readonly AddressAuthoritativeValues _addressValues;

        public SuccessfulAddressResolver(AddressAuthoritativeValues addressValues)
        {
            _addressValues = addressValues;
        }

        public AddressResolutionResult Resolve(
            string addressResolutionReference,
            TradingLocation tradingLocation)
        {
            return AddressResolutionResult.Succeeded(_addressValues);
        }
    }

    private sealed class RecordingDeterminer : IRegistrationOutcomeDeterminer
    {
        private readonly RegistrationOutcomeDetermination _determination;

        public RecordingDeterminer(RegistrationOutcomeDetermination determination)
        {
            _determination = determination;
        }

        public int CallCount { get; private set; }
        public VendorRegistrationIdentity? Identity { get; private set; }
        public RegistrationSemanticFingerprint? Fingerprint { get; private set; }

        public Task<RegistrationOutcomeDetermination> DetermineAsync(
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            CallCount++;
            Identity = identity;
            Fingerprint = fingerprint;

            return Task.FromResult(_determination);
        }
    }

    private sealed class RecordingNewVendorRegistrationProcessor
        : INewVendorRegistrationProcessor
    {
        private readonly RegisterVendorResult _result;

        public RecordingNewVendorRegistrationProcessor(RegisterVendorResult result)
        {
            _result = result;
        }

        public int CallCount { get; private set; }
        public RegisterVendorCommand? Command { get; private set; }
        public AddressAuthoritativeValues? AddressValues { get; private set; }
        public VendorRegistrationIdentity? Identity { get; private set; }
        public RegistrationSemanticFingerprint? Fingerprint { get; private set; }

        public Task<RegisterVendorResult> ProcessAsync(
            RegisterVendorCommand command,
            AddressAuthoritativeValues addressValues,
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            CallCount++;
            Command = command;
            AddressValues = addressValues;
            Identity = identity;
            Fingerprint = fingerprint;

            return Task.FromResult(_result);
        }
    }
}
