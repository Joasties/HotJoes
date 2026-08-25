using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class NewVendorRegistrationProcessorConcurrencyTests
{
    [Fact]
    public async Task ProcessAsync_ConcurrentIdentityRace_PreservesRaceSignal()
    {
        NewVendorRegistrationProcessor processor = CreateProcessor();
        RegisterVendorCommand command = CreateCommand();
        AddressAuthoritativeValues addressValues = CreateAddressValues();

        await Assert.ThrowsAsync<ConcurrentVendorRegistrationException>(
            () => processor.ProcessAsync(
                command,
                addressValues,
                VendorRegistrationIdentity.Create(command, addressValues),
                RegistrationSemanticFingerprint.Create(command, addressValues),
                CancellationToken.None));
    }

    private static NewVendorRegistrationProcessor CreateProcessor()
    {
        return new NewVendorRegistrationProcessor(
            new VendorRegisteredIntegrationEventMapper(),
            new ConcurrentRaceCommitter(),
            new FixedIdentifierGenerator(),
            new FixedTimeProvider());
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
            "address-reference-concurrent-reconciliation",
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
                "canonical-address-concurrent-reconciliation"),
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

    private sealed class ConcurrentRaceCommitter
        : INewVendorRegistrationCommitter
    {
        public Task CommitAsync(
            NewVendorRegistrationCommit commit,
            CancellationToken cancellationToken)
        {
            return Task.FromException(
                new ConcurrentVendorRegistrationException());
        }
    }

    private sealed class FixedIdentifierGenerator
        : IRegistrationIdentifierGenerator
    {
        public VendorId CreateVendorId()
        {
            return new VendorId(
                Guid.Parse("db0216d6-f3c9-4abe-9ee8-0e210253162e"));
        }

        public Guid CreateEventId()
        {
            return Guid.Parse("f7303520-f342-4da8-93d5-323703e078bc");
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
                19,
                0,
                0,
                TimeSpan.Zero);
        }
    }
}
