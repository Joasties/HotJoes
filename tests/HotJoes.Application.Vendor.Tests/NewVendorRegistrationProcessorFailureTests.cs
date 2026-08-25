using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class NewVendorRegistrationProcessorFailureTests
{
    [Fact]
    public async Task ProcessAsync_CommitterFails_ReturnsControlledPersistenceFailureOnce()
    {
        var committer = new ThrowingCommitter(
            token => new InvalidOperationException(
                "Provider-specific database detail must not escape."));
        NewVendorRegistrationProcessor processor = CreateProcessor(committer);
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        using var cancellationSource = new CancellationTokenSource();

        RegisterVendorResult result = await processor.ProcessAsync(
            command,
            addressValues,
            VendorRegistrationIdentity.Create(command, addressValues),
            RegistrationSemanticFingerprint.Create(command, addressValues),
            cancellationSource.Token);

        Assert.IsType<
            RegisterVendorResult.PersistenceOrAtomicRecordingFailure>(result);
        Assert.Equal(1, committer.InvocationCount);
        Assert.Equal(cancellationSource.Token, committer.CancellationToken);
    }

    [Fact]
    public async Task ProcessAsync_CallerCancellationDuringCommit_PropagatesCancellation()
    {
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();
        var committer = new ThrowingCommitter(
            token => new OperationCanceledException(token));
        NewVendorRegistrationProcessor processor = CreateProcessor(committer);
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();

        OperationCanceledException exception =
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => processor.ProcessAsync(
                    command,
                    addressValues,
                    VendorRegistrationIdentity.Create(command, addressValues),
                    RegistrationSemanticFingerprint.Create(
                        command,
                        addressValues),
                    cancellationSource.Token));

        Assert.Equal(cancellationSource.Token, exception.CancellationToken);
        Assert.Equal(1, committer.InvocationCount);
    }

    private static NewVendorRegistrationProcessor CreateProcessor(
        INewVendorRegistrationCommitter committer)
    {
        return new NewVendorRegistrationProcessor(
            new VendorRegisteredIntegrationEventMapper(),
            committer,
            new FixedIdentifierGenerator(),
            new FixedTimeProvider());
    }

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            "Persistence Failure Kitchen",
            "Persistence Failure Operator",
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
            "address-reference-persistence-failure",
            website: null,
            businessDescription: null,
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues CreateAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId("canonical-address-persistence-failure"),
            new BusinessAddressSnapshot(
                "14 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null);
    }

    private sealed class ThrowingCommitter : INewVendorRegistrationCommitter
    {
        private readonly Func<CancellationToken, Exception> _createException;

        public ThrowingCommitter(
            Func<CancellationToken, Exception> createException)
        {
            _createException = createException;
        }

        public int InvocationCount { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public Task CommitAsync(
            NewVendorRegistrationCommit commit,
            CancellationToken cancellationToken)
        {
            InvocationCount++;
            CancellationToken = cancellationToken;
            return Task.FromException(_createException(cancellationToken));
        }
    }

    private sealed class FixedIdentifierGenerator
        : IRegistrationIdentifierGenerator
    {
        public VendorId CreateVendorId()
        {
            return new VendorId(
                Guid.Parse("5dbd136c-fec0-4f27-b650-5391cd23d3cd"));
        }

        public Guid CreateEventId()
        {
            return Guid.Parse("fd2740e8-ee62-4d32-aa86-445030e6ba72");
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return new DateTimeOffset(
                2026,
                8,
                25,
                16,
                0,
                0,
                TimeSpan.Zero);
        }
    }
}
