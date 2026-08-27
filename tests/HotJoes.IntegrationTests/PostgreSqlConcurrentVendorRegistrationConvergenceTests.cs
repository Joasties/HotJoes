using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlConcurrentVendorRegistrationConvergenceTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlConcurrentVendorRegistrationConvergenceTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RegisterAsync_EquivalentConcurrentRequests_ConvergeOnOneCommittedOutcome()
    {
        string suffix = Guid.NewGuid().ToString("N");
        RegisterVendorCommand firstCommand = CreateCommand(
            suffix,
            "Equivalent concurrent registration.");
        RegisterVendorCommand secondCommand = CreateCommand(
            suffix,
            "Equivalent concurrent registration.");
        AddressAuthoritativeValues addressValues = CreateAddressValues(suffix);
        var release = new CommitBoundaryRelease(participantCount: 2);

        await EnsureSchemaExistsAsync();
        await using RegistrationHarness first = CreateHarness(
            addressValues,
            release,
            Guid.Parse("207b6ed4-780a-45f3-9b58-02a752389fad"),
            Guid.Parse("7c525444-5f3f-445c-af94-5d30044d86ba"));
        await using RegistrationHarness second = CreateHarness(
            addressValues,
            release,
            Guid.Parse("ad346e0d-abf5-40cc-aa16-c4dba976e498"),
            Guid.Parse("aa427607-bfdc-47dd-ae28-74f825372f50"));

        Task<RegisterVendorResult> firstTask =
            first.Service.RegisterAsync(firstCommand);
        Task<RegisterVendorResult> secondTask =
            second.Service.RegisterAsync(secondCommand);

        RegisterVendorResult[] results = await Task.WhenAll(
            firstTask,
            secondTask);

        RegisterVendorResult.Success[] successes = results
            .Select(Assert.IsType<RegisterVendorResult.Success>)
            .ToArray();
        Assert.Equal(successes[0].VendorId, successes[1].VendorId);
        Assert.All(
            successes,
            success => Assert.Equal(
                VendorState.PendingActivation,
                success.VendorState));

        await AssertSingleDurableRegistrationAsync(
            firstCommand,
            addressValues,
            successes[0].VendorId.Value);
    }

    [Fact]
    public async Task RegisterAsync_ConflictingConcurrentRequests_ConvergeOnSuccessAndConflict()
    {
        string suffix = Guid.NewGuid().ToString("N");
        RegisterVendorCommand firstCommand = CreateCommand(
            suffix,
            "First concurrent registration.");
        RegisterVendorCommand secondCommand = CreateCommand(
            suffix,
            "Materially different concurrent registration.");
        AddressAuthoritativeValues addressValues = CreateAddressValues(suffix);
        var release = new CommitBoundaryRelease(participantCount: 2);

        await EnsureSchemaExistsAsync();
        await using RegistrationHarness first = CreateHarness(
            addressValues,
            release,
            Guid.Parse("46fe7f6f-d11a-4f14-b73d-f8a4f71e526b"),
            Guid.Parse("bebca4aa-70a1-45aa-a4bb-6950558106aa"));
        await using RegistrationHarness second = CreateHarness(
            addressValues,
            release,
            Guid.Parse("f382751e-fae8-4fb1-b71c-f07228310666"),
            Guid.Parse("0d8de6ee-927d-443a-959e-e25d85c0d289"));

        Task<RegisterVendorResult> firstTask =
            first.Service.RegisterAsync(firstCommand);
        Task<RegisterVendorResult> secondTask =
            second.Service.RegisterAsync(secondCommand);

        RegisterVendorResult[] results = await Task.WhenAll(
            firstTask,
            secondTask);

        RegisterVendorResult.Success success = Assert.Single(
            results.OfType<RegisterVendorResult.Success>());
        Assert.Single(results.OfType<RegisterVendorResult.IdempotencyConflict>());

        await AssertSingleDurableRegistrationAsync(
            firstCommand,
            addressValues,
            success.VendorId.Value);
    }

    private RegistrationHarness CreateHarness(
        AddressAuthoritativeValues addressValues,
        CommitBoundaryRelease release,
        Guid vendorId,
        Guid eventId)
    {
        VendorRegistrationDbContext context = CreateContext();
        var realCommitter = new PostgreSqlNewVendorRegistrationCommitter(
            context,
            new VendorRegisteredIntegrationEventSerializer());
        var coordinatedCommitter = new CoordinatedCommitter(
            realCommitter,
            release);
        var processor = new NewVendorRegistrationProcessor(
            new VendorRegisteredIntegrationEventMapper(),
            coordinatedCommitter,
            new FixedIdentifierGenerator(vendorId, eventId),
            TimeProvider.System);
        var service = new RegisterVendorService(
            new AcceptingRegisterVendorCommandValidator(),
            new AddressResolutionInvoker(
                new SuccessfulAddressResolver(addressValues)),
            new PostgreSqlRegistrationOutcomeDeterminer(context),
            processor);

        return new RegistrationHarness(context, service);
    }

    private async Task AssertSingleDurableRegistrationAsync(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        Guid successfulVendorId)
    {
        VendorRegistrationIdentity identity = VendorRegistrationIdentity.Create(
            command,
            addressValues);
        await using VendorRegistrationDbContext context = CreateContext();

        VendorRegistrationRecord[] vendors = await context
            .Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .Where(record =>
                record.NormalizedTradingName ==
                    identity.NormalizedTradingName.ToLowerInvariant()
                && record.NormalizedLegalOperatorName ==
                    identity.NormalizedLegalOperatorName.ToLowerInvariant()
                && record.CanonicalAddressId ==
                    identity.CanonicalAddressId.Value)
            .ToArrayAsync();

        VendorRegistrationRecord vendor = Assert.Single(vendors);
        Assert.Equal(successfulVendorId, vendor.VendorId);
        Assert.Equal(
            1,
            await context.Set<VendorRegistrationOutcomeRecord>()
                .AsNoTracking()
                .CountAsync(record => record.VendorId == vendor.VendorId));
        Assert.Equal(
            1,
            await context.Set<VendorRegistrationOutboxRecord>()
                .AsNoTracking()
                .CountAsync(record => record.VendorId == vendor.VendorId));
    }

    private async Task EnsureSchemaExistsAsync()
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static RegisterVendorCommand CreateCommand(
        string suffix,
        string businessDescription)
    {
        return new RegisterVendorCommand(
            $"Concurrent Convergence Kitchen {suffix}",
            $"Concurrent Convergence Operator {suffix}",
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
            $"address-reference-concurrent-convergence-{suffix}",
            website: null,
            businessDescription,
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues CreateAddressValues(string suffix)
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId(
                $"canonical-address-concurrent-convergence-{suffix}"),
            new BusinessAddressSnapshot(
                "20 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null);
    }

    private sealed class RegistrationHarness : IAsyncDisposable
    {
        private readonly VendorRegistrationDbContext _context;

        public RegistrationHarness(
            VendorRegistrationDbContext context,
            RegisterVendorService service)
        {
            _context = context;
            Service = service;
        }

        public RegisterVendorService Service { get; }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }
    }

    private sealed class CoordinatedCommitter : INewVendorRegistrationCommitter
    {
        private readonly INewVendorRegistrationCommitter _inner;
        private readonly CommitBoundaryRelease _release;

        public CoordinatedCommitter(
            INewVendorRegistrationCommitter inner,
            CommitBoundaryRelease release)
        {
            _inner = inner;
            _release = release;
        }

        public async Task CommitAsync(
            NewVendorRegistrationCommit commit,
            CancellationToken cancellationToken)
        {
            await _release.ArriveAndWaitAsync(cancellationToken);
            await _inner.CommitAsync(commit, cancellationToken);
        }
    }

    private sealed class CommitBoundaryRelease
    {
        private readonly int _participantCount;
        private readonly TaskCompletionSource _release =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _arrivals;

        public CommitBoundaryRelease(int participantCount)
        {
            _participantCount = participantCount;
        }

        public async Task ArriveAndWaitAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _arrivals) == _participantCount)
            {
                _release.TrySetResult();
            }

            await _release.Task.WaitAsync(
                TimeSpan.FromSeconds(10),
                cancellationToken);
        }
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

    private sealed class AcceptingRegisterVendorCommandValidator
        : IRegisterVendorCommandValidator
    {
        public RegisterVendorCommandValidationResult Validate(
            RegisterVendorCommand command)
        {
            return RegisterVendorCommandValidationResult.Accepted(command);
        }
    }

    private sealed class FixedIdentifierGenerator : IRegistrationIdentifierGenerator
    {
        private readonly VendorId _vendorId;
        private readonly Guid _eventId;

        public FixedIdentifierGenerator(Guid vendorId, Guid eventId)
        {
            _vendorId = new VendorId(vendorId);
            _eventId = eventId;
        }

        public VendorId CreateVendorId()
        {
            return _vendorId;
        }

        public Guid CreateEventId()
        {
            return _eventId;
        }
    }
}
