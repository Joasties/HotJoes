using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorServiceConcurrentReconciliationTests
{
    [Fact]
    public async Task RegisterAsync_EquivalentRaceLoser_ReturnsWinnerOriginalResult()
    {
        var originalResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(
                new VendorId(
                    Guid.Parse("e9ac5ba1-ed27-479a-a995-e8e7927d9761"))));
        var determiner = new SequencedDeterminer(
            RegistrationOutcomeDetermination.FirstProcessingRequired(),
            RegistrationOutcomeDetermination.Replay(originalResult));
        var processor = new ConcurrentRaceProcessor();
        RegisterVendorService service = CreateService(determiner, processor);

        RegisterVendorResult result = await service.RegisterAsync(CreateCommand());

        Assert.Same(originalResult, result);
        Assert.Equal(2, determiner.InvocationCount);
        Assert.Equal(1, processor.InvocationCount);
        Assert.Equal(determiner.Identities[0], determiner.Identities[1]);
        Assert.Equal(determiner.Fingerprints[0], determiner.Fingerprints[1]);
    }

    [Fact]
    public async Task RegisterAsync_ConflictingRaceLoser_ReturnsIdempotencyConflict()
    {
        var determiner = new SequencedDeterminer(
            RegistrationOutcomeDetermination.FirstProcessingRequired(),
            RegistrationOutcomeDetermination.ConflictDetected());
        var processor = new ConcurrentRaceProcessor();
        RegisterVendorService service = CreateService(determiner, processor);

        RegisterVendorResult result = await service.RegisterAsync(CreateCommand());

        Assert.IsType<RegisterVendorResult.IdempotencyConflict>(result);
        Assert.Equal(2, determiner.InvocationCount);
        Assert.Equal(1, processor.InvocationCount);
    }

    [Fact]
    public async Task RegisterAsync_RaceWinnerCannotBeResolved_ReturnsPersistenceFailure()
    {
        var determiner = new SequencedDeterminer(
            RegistrationOutcomeDetermination.FirstProcessingRequired(),
            RegistrationOutcomeDetermination.FirstProcessingRequired());
        var processor = new ConcurrentRaceProcessor();
        RegisterVendorService service = CreateService(determiner, processor);

        RegisterVendorResult result = await service.RegisterAsync(CreateCommand());

        Assert.IsType<
            RegisterVendorResult.PersistenceOrAtomicRecordingFailure>(result);
        Assert.Equal(2, determiner.InvocationCount);
        Assert.Equal(1, processor.InvocationCount);
    }

    private static RegisterVendorService CreateService(
        IRegistrationOutcomeDeterminer determiner,
        INewVendorRegistrationProcessor processor)
    {
        return new RegisterVendorService(
            new AcceptingRegisterVendorCommandValidator(),
            new AddressResolutionInvoker(
                new SuccessfulAddressResolver(CreateAddressValues())),
            determiner,
            processor);
    }

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            "Concurrent Reconciliation Kitchen",
            "Concurrent Reconciliation Operator",
            LegalOperatorType.SoleTrader,
            companyRegistrationNumber: null,
            TradingLocation.Kitchen,
            new TimeOnly(17, 0),
            new TimeOnly(2, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Jamie Taylor",
            "jamie@example.test",
            "+44 20 7946 0123",
            "address-reference-concurrent-reconciliation-service",
            website: null,
            businessDescription: null,
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues CreateAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId(
                "canonical-address-concurrent-reconciliation-service"),
            new BusinessAddressSnapshot(
                "18 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null);
    }

    private sealed class SuccessfulAddressResolver : IAddressResolver
    {
        private readonly AddressAuthoritativeValues _addressValues;

        public SuccessfulAddressResolver(
            AddressAuthoritativeValues addressValues)
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

    private sealed class SequencedDeterminer : IRegistrationOutcomeDeterminer
    {
        private readonly Queue<RegistrationOutcomeDetermination> _results;

        public SequencedDeterminer(
            params RegistrationOutcomeDetermination[] results)
        {
            _results = new Queue<RegistrationOutcomeDetermination>(results);
        }

        public int InvocationCount { get; private set; }

        public List<VendorRegistrationIdentity> Identities { get; } = [];

        public List<RegistrationSemanticFingerprint> Fingerprints { get; } = [];

        public Task<RegistrationOutcomeDetermination> DetermineAsync(
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            InvocationCount++;
            Identities.Add(identity);
            Fingerprints.Add(fingerprint);
            return Task.FromResult(_results.Dequeue());
        }
    }

    private sealed class ConcurrentRaceProcessor
        : INewVendorRegistrationProcessor
    {
        public int InvocationCount { get; private set; }

        public Task<RegisterVendorResult> ProcessAsync(
            RegisterVendorCommand command,
            AddressAuthoritativeValues addressValues,
            VendorRegistrationIdentity identity,
            RegistrationSemanticFingerprint fingerprint,
            CancellationToken cancellationToken)
        {
            InvocationCount++;
            return Task.FromException<RegisterVendorResult>(
                new ConcurrentVendorRegistrationException());
        }
    }
}
