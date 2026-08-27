using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorServiceCanonicalContactTests
{
    [Fact]
    public async Task RegisterAsync_WhenContactValuesRequireCanonicalisation_PassesOnlyCanonicalValuesDownstream()
    {
        RegisterVendorCommand rawCommand = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        var determiner = new FirstProcessingDeterminer();
        var processor = new RecordingNewVendorRegistrationProcessor();
        var service = new RegisterVendorService(
            new RegisterVendorCommandValidator(),
            new AddressResolutionInvoker(
                new SuccessfulAddressResolver(addressValues)),
            determiner,
            processor);

        await service.RegisterAsync(rawCommand);

        Assert.NotNull(processor.Command);
        Assert.NotSame(rawCommand, processor.Command);
        Assert.Equal(
            "Jordan.Smith@example.test",
            processor.Command.ContactEmail);
        Assert.Equal("+442071234567", processor.Command.ContactTelephone);
        Assert.Equal(
            VendorRegistrationIdentity.Create(processor.Command, addressValues),
            determiner.Identity);
        Assert.Equal(
            RegistrationSemanticFingerprint.Create(
                processor.Command,
                addressValues),
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
            contactEmail: "  Jordan.Smith@Example.TEST  ",
            contactTelephone: "(020) 7123-4567",
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
        private readonly AddressAuthoritativeValues _values;

        public SuccessfulAddressResolver(AddressAuthoritativeValues values)
        {
            _values = values;
        }

        public AddressResolutionResult Resolve(
            string addressResolutionReference,
            TradingLocation tradingLocation)
        {
            return AddressResolutionResult.Succeeded(_values);
        }
    }

    private sealed class FirstProcessingDeterminer
        : IRegistrationOutcomeDeterminer
    {
        public VendorRegistrationIdentity? Identity { get; private set; }

        public RegistrationSemanticFingerprint? Fingerprint { get; private set; }

        public Task<RegistrationOutcomeDetermination> DetermineAsync(
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            Identity = identity;
            Fingerprint = fingerprint;
            return Task.FromResult(
                RegistrationOutcomeDetermination.FirstProcessingRequired());
        }
    }

    private sealed class RecordingNewVendorRegistrationProcessor
        : INewVendorRegistrationProcessor
    {
        public RegisterVendorCommand? Command { get; private set; }

        public Task<RegisterVendorResult> ProcessAsync(
            RegisterVendorCommand command,
            AddressAuthoritativeValues addressValues,
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            Command = command;
            return Task.FromResult(
                RegisterVendorResult.PersistenceOrAtomicRecordingFailed());
        }
    }
}
