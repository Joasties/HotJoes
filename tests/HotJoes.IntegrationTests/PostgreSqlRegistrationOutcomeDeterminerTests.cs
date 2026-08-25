using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlRegistrationOutcomeDeterminerTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlRegistrationOutcomeDeterminerTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DetermineAsync_IdentityIsAbsent_ReturnsFirstProcessing()
    {
        RegistrationInputs inputs = CreateInputs("absent");
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var determiner = new PostgreSqlRegistrationOutcomeDeterminer(context);

        RegistrationOutcomeDetermination result = await determiner.DetermineAsync(
            inputs.Identity,
            inputs.Fingerprint,
            CancellationToken.None);

        Assert.IsType<RegistrationOutcomeDetermination.FirstProcessing>(result);
    }

    [Fact]
    public async Task DetermineAsync_IdentityAndFingerprintMatch_ReturnsOriginalResult()
    {
        RegistrationInputs inputs = CreateInputs("matching");
        Guid vendorId = Guid.Parse("ae4c80db-4e7e-47a8-93a8-f7d29c90d31f");
        await SeedRegistrationAsync(
            vendorId,
            inputs,
            fingerprintVersion: inputs.Fingerprint.Version,
            fingerprintBytes: Convert.FromHexString(
                inputs.Fingerprint.Sha256Digest),
            currentVendorState: "pendingActivation");
        await using VendorRegistrationDbContext context = CreateContext();
        var determiner = new PostgreSqlRegistrationOutcomeDeterminer(context);

        RegistrationOutcomeDetermination result = await determiner.DetermineAsync(
            inputs.Identity,
            inputs.Fingerprint,
            CancellationToken.None);

        var replay = Assert.IsType<
            RegistrationOutcomeDetermination.EquivalentReplay>(result);
        Assert.Equal(new VendorId(vendorId), replay.OriginalResult.VendorId);
        Assert.Equal(
            VendorState.PendingActivation,
            replay.OriginalResult.VendorState);
    }

    [Fact]
    public async Task DetermineAsync_SameIdentityWithDifferentDigest_ReturnsConflict()
    {
        RegistrationInputs persistedInputs = CreateInputs("digest-conflict");
        RegistrationInputs changedInputs = CreateInputs(
            "digest-conflict",
            businessDescription: "Materially different registration information.");
        await SeedRegistrationAsync(
            Guid.Parse("dbd5c6b5-dfa1-44c1-bfbd-d1c257274d68"),
            persistedInputs,
            fingerprintVersion: persistedInputs.Fingerprint.Version,
            fingerprintBytes: Convert.FromHexString(
                persistedInputs.Fingerprint.Sha256Digest),
            currentVendorState: "pendingActivation");
        await using VendorRegistrationDbContext context = CreateContext();
        var determiner = new PostgreSqlRegistrationOutcomeDeterminer(context);

        RegistrationOutcomeDetermination result = await determiner.DetermineAsync(
            changedInputs.Identity,
            changedInputs.Fingerprint,
            CancellationToken.None);

        Assert.IsType<RegistrationOutcomeDetermination.Conflict>(result);
    }

    [Fact]
    public async Task DetermineAsync_SameIdentityWithDifferentFingerprintVersion_ReturnsConflict()
    {
        RegistrationInputs inputs = CreateInputs("version-conflict");
        await SeedRegistrationAsync(
            Guid.Parse("77800733-76c3-4b5c-a506-7c378b2c9990"),
            inputs,
            fingerprintVersion: 2,
            fingerprintBytes: Convert.FromHexString(
                inputs.Fingerprint.Sha256Digest),
            currentVendorState: "pendingActivation");
        await using VendorRegistrationDbContext context = CreateContext();
        var determiner = new PostgreSqlRegistrationOutcomeDeterminer(context);

        RegistrationOutcomeDetermination result = await determiner.DetermineAsync(
            inputs.Identity,
            inputs.Fingerprint,
            CancellationToken.None);

        Assert.IsType<RegistrationOutcomeDetermination.Conflict>(result);
    }

    [Fact]
    public async Task DetermineAsync_CurrentVendorStateChanged_ReplaysOriginalResultState()
    {
        RegistrationInputs inputs = CreateInputs("later-state");
        Guid vendorId = Guid.Parse("3be06009-7068-40d3-a85a-c39a11d4fb31");
        await SeedRegistrationAsync(
            vendorId,
            inputs,
            fingerprintVersion: inputs.Fingerprint.Version,
            fingerprintBytes: Convert.FromHexString(
                inputs.Fingerprint.Sha256Digest),
            currentVendorState: "activated");
        await using VendorRegistrationDbContext context = CreateContext();
        var determiner = new PostgreSqlRegistrationOutcomeDeterminer(context);

        RegistrationOutcomeDetermination result = await determiner.DetermineAsync(
            inputs.Identity,
            inputs.Fingerprint,
            CancellationToken.None);

        var replay = Assert.IsType<
            RegistrationOutcomeDetermination.EquivalentReplay>(result);
        Assert.Equal(new VendorId(vendorId), replay.OriginalResult.VendorId);
        Assert.Equal(
            VendorState.PendingActivation,
            replay.OriginalResult.VendorState);
    }

    private async Task SeedRegistrationAsync(
        Guid vendorId,
        RegistrationInputs inputs,
        short fingerprintVersion,
        byte[] fingerprintBytes,
        string currentVendorState)
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        context.Set<VendorRegistrationRecord>().Add(
            CreateVendorRecord(vendorId, inputs, currentVendorState));
        context.Set<VendorRegistrationOutcomeRecord>().Add(
            new VendorRegistrationOutcomeRecord
            {
                VendorId = vendorId,
                FingerprintVersion = fingerprintVersion,
                SemanticFingerprintSha256 = fingerprintBytes,
                ResultVendorState = "pendingActivation"
            });
        await context.SaveChangesAsync();
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static RegistrationInputs CreateInputs(
        string suffix,
        string businessDescription = "Original registration information.")
    {
        var command = new RegisterVendorCommand(
            $"Replay Kitchen {suffix}",
            $"Replay Operator {suffix}",
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
            $"address-reference-{suffix}",
            website: null,
            businessDescription,
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
        var addressValues = new AddressAuthoritativeValues(
            new CanonicalAddressId($"canonical-address-{suffix}"),
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

        return new RegistrationInputs(
            command,
            addressValues,
            VendorRegistrationIdentity.Create(command, addressValues),
            RegistrationSemanticFingerprint.Create(command, addressValues));
    }

    private static VendorRegistrationRecord CreateVendorRecord(
        Guid vendorId,
        RegistrationInputs inputs,
        string vendorState)
    {
        return new VendorRegistrationRecord
        {
            VendorId = vendorId,
            VendorState = vendorState,
            TradingPreference = "offline",
            RegisteredAtUtc = new DateTimeOffset(
                2026,
                8,
                25,
                17,
                0,
                0,
                TimeSpan.Zero),
            LegalOperatorType = "soleTrader",
            LegalOperatorName = inputs.Command.LegalOperatorName,
            NormalizedLegalOperatorName =
                inputs.Command.LegalOperatorName.Trim().ToLowerInvariant(),
            TradingName = inputs.Command.TradingName,
            NormalizedTradingName =
                inputs.Command.TradingName.Trim().ToLowerInvariant(),
            CompanyRegistrationNumber = null,
            ContactName = inputs.Command.ContactName,
            ContactEmail = inputs.Command.ContactEmail,
            ContactTelephone = inputs.Command.ContactTelephone,
            CanonicalAddressId = inputs.AddressValues.CanonicalAddressId.Value,
            RecipientOrOrganisationName = null,
            AddressLine1 = "14 Example Street",
            AddressLine2 = null,
            AddressLine3 = null,
            PostTown = "LONDON",
            Postcode = "AB1 2CD",
            County = null,
            FoodRegistrationAuthority = "Greenwich Borough Council",
            PrimaryTradingAuthority = null,
            TradingLocation = "kitchen",
            OpeningHoursStart = inputs.Command.OpeningHoursStartTime,
            OpeningHoursEnd = inputs.Command.OpeningHoursEndTime,
            ServiceIncludesHotFood = inputs.Command.ServiceIncludesHotFood,
            AlcoholService = inputs.Command.AlcoholService,
            Website = null,
            BusinessDescription = inputs.Command.BusinessDescription
        };
    }

    private sealed record RegistrationInputs(
        RegisterVendorCommand Command,
        AddressAuthoritativeValues AddressValues,
        VendorRegistrationIdentity Identity,
        RegistrationSemanticFingerprint Fingerprint);
}
