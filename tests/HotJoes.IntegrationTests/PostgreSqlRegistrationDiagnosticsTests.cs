using System.Diagnostics.Metrics;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(RegistrationObservabilityTestCollection.Name)]
public sealed class PostgreSqlRegistrationDiagnosticsTests
{
    private const string MeterName =
        "HotJoes.Infrastructure.Persistence";
    private const string IdempotencyInstrument =
        "hotjoes.vendor.registration.idempotency";
    private const string PersistenceInstrument =
        "hotjoes.vendor.registration.persistence";
    private const string SensitiveMarker =
        "alex@example.test 2 High Street secret-connection-string";

    private static readonly Guid VendorId = Guid.Parse(
        "897726cd-64f4-4460-a5e7-6118a1136137");
    private static readonly Guid EventId = Guid.Parse(
        "275217d5-1202-46a4-b7f3-9f743cfab51f");

    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlRegistrationDiagnosticsTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DetermineAsync_FirstReplayAndConflict_EmitSafeIdempotencyDiagnostics()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            CreateOptions();
        await ResetSchemaAsync(options);
        RegistrationInputs original = CreateInputs(
            businessDescription: "Original registration.");
        RegistrationInputs changed = CreateInputs(
            businessDescription: "Materially changed registration.");
        var logger =
            new RecordingLogger<PostgreSqlRegistrationOutcomeDeterminer>();
        using var metrics = new RecordingMeterListener(MeterName);

        await using (var context = new VendorRegistrationDbContext(options))
        {
            var determiner = new PostgreSqlRegistrationOutcomeDeterminer(
                context,
                logger);

            RegistrationOutcomeDetermination first =
                await determiner.DetermineAsync(
                    original.Identity,
                    original.Fingerprint,
                    CancellationToken.None);

            Assert.IsType<
                RegistrationOutcomeDetermination.FirstProcessing>(first);
        }

        await CommitAsync(options, original);

        await using (var context = new VendorRegistrationDbContext(options))
        {
            var determiner = new PostgreSqlRegistrationOutcomeDeterminer(
                context,
                logger);

            RegistrationOutcomeDetermination replay =
                await determiner.DetermineAsync(
                    original.Identity,
                    original.Fingerprint,
                    CancellationToken.None);
            RegistrationOutcomeDetermination conflict =
                await determiner.DetermineAsync(
                    changed.Identity,
                    changed.Fingerprint,
                    CancellationToken.None);

            Assert.IsType<
                RegistrationOutcomeDetermination.EquivalentReplay>(replay);
            Assert.IsType<RegistrationOutcomeDetermination.Conflict>(
                conflict);
        }

        Assert.Collection(
            logger.Entries,
            entry => AssertIdempotencyEntry(
                entry,
                "firstProcessing",
                expectedVendorId: null),
            entry => AssertIdempotencyEntry(
                entry,
                "equivalentReplay",
                VendorId),
            entry => AssertIdempotencyEntry(
                entry,
                "conflict",
                VendorId));
        AssertMetric(metrics, IdempotencyInstrument, "firstProcessing");
        AssertMetric(metrics, IdempotencyInstrument, "equivalentReplay");
        AssertMetric(metrics, IdempotencyInstrument, "conflict");
    }

    [Fact]
    public async Task CommitAsync_SuccessAndFailure_EmitSafePersistenceDiagnostics()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            CreateOptions();
        await ResetSchemaAsync(options);
        RegistrationInputs original = CreateInputs(
            businessDescription: "Original registration.");
        var logger =
            new RecordingLogger<PostgreSqlNewVendorRegistrationCommitter>();
        using var metrics = new RecordingMeterListener(MeterName);

        await CommitAsync(options, original, logger);

        RegistrationInputs second = CreateInputs(
            suffix: "second",
            businessDescription: "Second registration.");
        NewVendorRegistrationCommit duplicateEventCommit = CreateCommit(
            second,
            Guid.Parse("51280c34-e01f-42b5-a0e6-9056e7211221"),
            EventId);

        await using (var context = new VendorRegistrationDbContext(options))
        {
            var committer = new PostgreSqlNewVendorRegistrationCommitter(
                context,
                new VendorRegisteredIntegrationEventSerializer(),
                logger);

            await Assert.ThrowsAsync<DbUpdateException>(() =>
                committer.CommitAsync(
                    duplicateEventCommit,
                    CancellationToken.None));
        }

        Assert.Collection(
            logger.Entries,
            entry => AssertPersistenceEntry(
                entry,
                "committed",
                VendorId,
                EventId,
                LogLevel.Information),
            entry => AssertPersistenceEntry(
                entry,
                "failed",
                duplicateEventCommit.Vendor.Id.Value,
                EventId,
                LogLevel.Warning));
        AssertMetric(metrics, PersistenceInstrument, "committed");
        AssertMetric(metrics, PersistenceInstrument, "failed");
    }

    private DbContextOptions<VendorRegistrationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<VendorRegistrationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;
    }

    private static async Task ResetSchemaAsync(
        DbContextOptions<VendorRegistrationDbContext> options)
    {
        await using var context = new VendorRegistrationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                vendor_registration_outbox,
                vendor_registration_outcomes,
                vendor_registrations
            RESTART IDENTITY CASCADE;
            """);
    }

    private static Task CommitAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        RegistrationInputs inputs)
    {
        return CommitAsync(
            options,
            inputs,
            logger: null);
    }

    private static async Task CommitAsync(
        DbContextOptions<VendorRegistrationDbContext> options,
        RegistrationInputs inputs,
        ILogger<PostgreSqlNewVendorRegistrationCommitter>? logger)
    {
        NewVendorRegistrationCommit commit = CreateCommit(
            inputs,
            VendorId,
            EventId);
        await using var context = new VendorRegistrationDbContext(options);
        var committer = new PostgreSqlNewVendorRegistrationCommitter(
            context,
            new VendorRegisteredIntegrationEventSerializer(),
            logger);

        await committer.CommitAsync(commit, CancellationToken.None);
    }

    private static NewVendorRegistrationCommit CreateCommit(
        RegistrationInputs inputs,
        Guid vendorId,
        Guid eventId)
    {
        VendorAggregate vendor = CreateVendor(
            vendorId,
            inputs.Command,
            inputs.AddressValues);
        VendorRegistered completedFact = Assert.IsType<VendorRegistered>(
            Assert.Single(vendor.DomainEvents));
        VendorRegisteredIntegrationEvent integrationEvent =
            new VendorRegisteredIntegrationEventMapper().Map(
                completedFact,
                vendor,
                eventId,
                vendor.RegisteredAt);
        var originalResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(vendor.Id));

        return new NewVendorRegistrationCommit(
            vendor,
            inputs.Identity,
            inputs.Fingerprint,
            originalResult,
            integrationEvent);
    }

    private static RegistrationInputs CreateInputs(
        string suffix = "original",
        string businessDescription = "Original registration.")
    {
        var command = new RegisterVendorCommand(
            $"Diagnostics Kitchen {suffix}",
            $"Diagnostics Operator {suffix}",
            LegalOperatorType.SoleTrader,
            companyRegistrationNumber: null,
            TradingLocation.Kitchen,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Alex Morgan",
            "alex@example.test",
            "+442079460123",
            $"address-resolution-{suffix}",
            website: null,
            businessDescription,
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
        var addressValues = new AddressAuthoritativeValues(
            new CanonicalAddressId($"canonical-address-{suffix}"),
            new BusinessAddressSnapshot(
                SensitiveMarker,
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

    private static VendorAggregate CreateVendor(
        Guid vendorId,
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
    {
        var information = new VendorRegistrationInformation(
            command.LegalOperatorType,
            new VendorName(command.LegalOperatorName),
            new VendorName(command.TradingName),
            companyRegistrationNumber: null,
            new PrimaryContact(
                command.ContactName,
                new EmailAddress(command.ContactEmail),
                new TelephoneNumber(command.ContactTelephone)),
            addressValues.CanonicalAddressId,
            addressValues.BusinessAddressSnapshot,
            addressValues.FoodRegistrationAuthority,
            addressValues.PrimaryTradingAuthority,
            new TradingCharacteristics(
                command.TradingLocation,
                new OpeningHours(
                    command.OpeningHoursStartTime,
                    command.OpeningHoursEndTime),
                command.ServiceIncludesHotFood,
                command.AlcoholService));

        return VendorAggregate.Register(
            new VendorId(vendorId),
            information,
            website: null,
            command.BusinessDescription,
            new DateTimeOffset(2026, 8, 29, 20, 0, 0, TimeSpan.Zero));
    }

    private static void AssertIdempotencyEntry(
        LogEntry entry,
        string expectedOutcome,
        Guid? expectedVendorId)
    {
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Equal(
            expectedOutcome,
            entry.Properties["IdempotencyOutcome"]);

        if (expectedVendorId is null)
        {
            Assert.DoesNotContain("VendorId", entry.Properties.Keys);
        }
        else
        {
            Assert.Equal(expectedVendorId, entry.Properties["VendorId"]);
        }

        AssertSafe(entry);
    }

    private static void AssertPersistenceEntry(
        LogEntry entry,
        string expectedOutcome,
        Guid expectedVendorId,
        Guid expectedEventId,
        LogLevel expectedLevel)
    {
        Assert.Equal(expectedLevel, entry.Level);
        Assert.Equal(expectedVendorId, entry.Properties["VendorId"]);
        Assert.Equal(expectedEventId, entry.Properties["EventId"]);
        Assert.Equal("VendorRegistered", entry.Properties["EventType"]);
        Assert.Equal(1, entry.Properties["EventVersion"]);
        Assert.Equal(
            expectedOutcome,
            entry.Properties["PersistenceOutcome"]);
        AssertSafe(entry);
    }

    private static void AssertMetric(
        RecordingMeterListener metrics,
        string instrumentName,
        string expectedOutcome)
    {
        MetricMeasurement measurement = Assert.Single(
            metrics.Measurements,
            item => item.InstrumentName == instrumentName &&
                Equals(item.Tags["outcome"], expectedOutcome));

        Assert.Equal(1, measurement.Value);
        Assert.Single(measurement.Tags);
    }

    private static void AssertSafe(LogEntry entry)
    {
        Assert.Null(entry.Exception);
        Assert.DoesNotContain(
            SensitiveMarker,
            entry.Message,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "alex@example.test",
            entry.Message,
            StringComparison.Ordinal);
        Assert.All(
            entry.Properties.Values,
            value =>
            {
                string text = value?.ToString() ?? string.Empty;
                Assert.DoesNotContain(
                    SensitiveMarker,
                    text,
                    StringComparison.Ordinal);
                Assert.DoesNotContain(
                    "alex@example.test",
                    text,
                    StringComparison.Ordinal);
            });
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            IReadOnlyDictionary<string, object?> properties =
                state is IEnumerable<KeyValuePair<string, object?>> values
                    ? values
                        .Where(pair => pair.Key != "{OriginalFormat}")
                        .ToDictionary(
                            pair => pair.Key,
                            pair => pair.Value,
                            StringComparer.Ordinal)
                    : new Dictionary<string, object?>();

            Entries.Add(new LogEntry(
                logLevel,
                formatter(state, exception),
                exception,
                properties));
        }
    }

    private sealed class RecordingMeterListener : IDisposable
    {
        private readonly MeterListener _listener = new();

        public RecordingMeterListener(string meterName)
        {
            _listener.InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == meterName)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            };
            _listener.SetMeasurementEventCallback<long>(
                (instrument, measurement, tags, state) =>
                {
                    Measurements.Add(new MetricMeasurement(
                        instrument.Name,
                        measurement,
                        tags.ToArray().ToDictionary(
                            tag => tag.Key,
                            tag => tag.Value)));
                });
            _listener.Start();
        }

        public List<MetricMeasurement> Measurements { get; } = [];

        public void Dispose()
        {
            _listener.Dispose();
        }
    }

    private sealed record RegistrationInputs(
        RegisterVendorCommand Command,
        AddressAuthoritativeValues AddressValues,
        VendorRegistrationIdentity Identity,
        RegistrationSemanticFingerprint Fingerprint);

    private sealed record LogEntry(
        LogLevel Level,
        string Message,
        Exception? Exception,
        IReadOnlyDictionary<string, object?> Properties);

    private sealed record MetricMeasurement(
        string InstrumentName,
        long Value,
        IReadOnlyDictionary<string, object?> Tags);
}
