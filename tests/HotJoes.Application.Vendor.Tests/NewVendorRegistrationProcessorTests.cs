using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class NewVendorRegistrationProcessorTests
{
    [Fact]
    public async Task ProcessAsync_FirstProcessing_CreatesAndStagesOneCoherentRegistration()
    {
        var vendorId = new VendorId(
            new Guid("f10734bd-81e4-4b07-bbdd-520e29124dd3"));
        var eventId = new Guid("3ac04798-ef01-4bca-b070-dde8dac9502d");
        var registeredAt = new DateTimeOffset(
            2026,
            8,
            23,
            10,
            15,
            30,
            TimeSpan.Zero);
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();
        VendorRegistrationIdentity identity = VendorRegistrationIdentity.Create(
            command,
            addressValues);
        RegistrationSemanticFingerprint fingerprint =
            RegistrationSemanticFingerprint.Create(command, addressValues);
        var committer = new RecordingNewVendorRegistrationCommitter();
        var processor = new NewVendorRegistrationProcessor(
            new VendorRegisteredIntegrationEventMapper(),
            committer,
            new FixedRegistrationIdentifierGenerator(vendorId, eventId),
            new FixedTimeProvider(registeredAt));
        using var cancellationSource = new CancellationTokenSource();

        RegisterVendorResult result = await processor.ProcessAsync(
            command,
            addressValues,
            identity,
            fingerprint,
            cancellationSource.Token);

        RegisterVendorResult.Success success =
            Assert.IsType<RegisterVendorResult.Success>(result);
        Assert.Equal(vendorId, success.VendorId);

        NewVendorRegistrationCommit commit = Assert.Single(committer.Commits);
        Assert.Equal(cancellationSource.Token, committer.CancellationToken);
        Assert.Same(identity, commit.Identity);
        Assert.Same(fingerprint, commit.Fingerprint);

        HotJoes.Domain.Vendor.Vendor vendor = commit.Vendor;
        Assert.Equal(vendorId, vendor.Id);
        Assert.Equal(registeredAt, vendor.RegisteredAt);
        Assert.Equal(VendorState.PendingActivation, vendor.State);
        Assert.Equal("Hot Joes Greenwich", vendor.RegisteredInformation.TradingName.Value);
        Assert.Equal("Hot Joes Limited", vendor.RegisteredInformation.LegalOperatorName.Value);
        Assert.Equal(addressValues.CanonicalAddressId, vendor.RegisteredInformation.CanonicalAddressId);
        Assert.Equal(
            addressValues.BusinessAddressSnapshot,
            vendor.RegisteredInformation.BusinessAddressSnapshot);
        Assert.Equal(
            addressValues.FoodRegistrationAuthority,
            vendor.RegisteredInformation.FoodRegistrationAuthority);
        Assert.Equal(
            addressValues.PrimaryTradingAuthority,
            vendor.RegisteredInformation.PrimaryTradingAuthority);

        RegisterVendorResult.Success originalResult =
            Assert.IsType<RegisterVendorResult.Success>(commit.OriginalResult);
        Assert.Equal(vendorId, originalResult.VendorId);

        Assert.Equal(eventId, commit.IntegrationEvent.EventId);
        Assert.Equal(registeredAt, commit.IntegrationEvent.OccurredAt);
        Assert.Equal(vendorId.Value, commit.IntegrationEvent.Payload.VendorId);
        Assert.Equal(registeredAt, commit.IntegrationEvent.Payload.RegisteredAt);
    }

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            "Hot Joes Greenwich",
            "Hot Joes Limited",
            LegalOperatorType.LimitedCompany,
            "12345678",
            TradingLocation.Stall,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Joseph Bloggs",
            "joe@hotjoes.example",
            "020 7946 0123",
            "address-reference-001",
            "https://hotjoes.example",
            "Hot food from our Greenwich trading location.",
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
                "Greenwich Market",
                "Unit 4",
                "GREENWICH",
                "SE10 8AA",
                "Greater London",
                "Hot Joes Limited"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
    }

    private sealed class RecordingNewVendorRegistrationCommitter
        : INewVendorRegistrationCommitter
    {
        public List<NewVendorRegistrationCommit> Commits { get; } = [];

        public CancellationToken CancellationToken { get; private set; }

        public Task CommitAsync(
            NewVendorRegistrationCommit commit,
            CancellationToken cancellationToken)
        {
            Commits.Add(commit);
            CancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedRegistrationIdentifierGenerator
        : IRegistrationIdentifierGenerator
    {
        private readonly VendorId _vendorId;
        private readonly Guid _eventId;

        public FixedRegistrationIdentifierGenerator(VendorId vendorId, Guid eventId)
        {
            _vendorId = vendorId;
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

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }
    }
}
