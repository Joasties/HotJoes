using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class NewVendorRegistrationProcessorAggregateInvariantTests
{
    [Fact]
    public async Task ProcessAsync_DomainRejectsRegistration_ReturnsControlledFailureWithoutCommit()
    {
        var committer = new RecordingCommitter();
        var processor = new NewVendorRegistrationProcessor(
            new VendorRegisteredIntegrationEventMapper(),
            committer,
            new FixedIdentifierGenerator(),
            new FixedTimeProvider());
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues =
            CreateInvariantViolatingAddressValues();

        RegisterVendorResult result = await processor.ProcessAsync(
            command,
            addressValues,
            VendorRegistrationIdentity.Create(command, addressValues),
            RegistrationSemanticFingerprint.Create(command, addressValues),
            CancellationToken.None);

        Assert.IsType<RegisterVendorResult.AggregateInvariantFailure>(result);
        Assert.Equal(0, committer.InvocationCount);
    }

    private static RegisterVendorCommand CreateCommand()
    {
        return new RegisterVendorCommand(
            "Invariant Kitchen",
            "Invariant Kitchen Limited",
            LegalOperatorType.LimitedCompany,
            "12345678",
            TradingLocation.Stall,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Jamie Taylor",
            "jamie@example.test",
            "+442071234567",
            "address-reference-invariant-failure",
            website: null,
            businessDescription: null,
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues
        CreateInvariantViolatingAddressValues()
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId("canonical-address-invariant-failure"),
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

    private sealed class RecordingCommitter : INewVendorRegistrationCommitter
    {
        public int InvocationCount { get; private set; }

        public Task CommitAsync(
            NewVendorRegistrationCommit commit,
            CancellationToken cancellationToken)
        {
            InvocationCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedIdentifierGenerator
        : IRegistrationIdentifierGenerator
    {
        public VendorId CreateVendorId()
        {
            return new VendorId(
                Guid.Parse("52692f93-d54d-43ac-9eca-aa9181e0c58b"));
        }

        public Guid CreateEventId()
        {
            return Guid.Parse("9cb4d888-86df-40c7-a4dc-3ea41e494102");
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return new DateTimeOffset(
                2026,
                8,
                27,
                10,
                0,
                0,
                TimeSpan.Zero);
        }
    }
}
