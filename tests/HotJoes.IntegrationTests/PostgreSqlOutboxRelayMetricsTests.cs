using System.Diagnostics.Metrics;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlOutboxRelayMetricsTests
{
    private const string MeterName =
        "HotJoes.Infrastructure.Persistence";
    private const string EligibleCountInstrument =
        "hotjoes.vendor.outbox.eligible";
    private const string OldestEligibleAgeInstrument =
        "hotjoes.vendor.outbox.oldest_eligible_age_seconds";
    private const string RetryPendingCountInstrument =
        "hotjoes.vendor.outbox.retry_pending";
    private const string StalledCountInstrument =
        "hotjoes.vendor.outbox.stalled";

    private static readonly DateTimeOffset ObservedAtUtc = new(
        2026,
        8,
        29,
        18,
        0,
        0,
        TimeSpan.Zero);

    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlOutboxRelayMetricsTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RefreshAsync_ReportsEligibleAgeRetryAndStalledWork()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            CreateOptions();
        await ResetAndSeedAsync(options);

        await using var context = new VendorRegistrationDbContext(options);
        using var metrics = new PostgreSqlOutboxRelayMetrics(context);
        using var listener = new RecordingMeterListener(MeterName);

        await metrics.RefreshAsync(ObservedAtUtc);
        listener.RecordObservableInstruments();

        AssertMeasurement(listener, EligibleCountInstrument, 2);
        AssertMeasurement(listener, OldestEligibleAgeInstrument, 300);
        AssertMeasurement(listener, RetryPendingCountInstrument, 3);
        AssertMeasurement(listener, StalledCountInstrument, 1);
    }

    [Fact]
    public async Task RefreshAsync_NoEligibleWork_ReportsZeroAgeAndCounts()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            CreateOptions();
        await ResetSchemaAsync(options);

        await using var context = new VendorRegistrationDbContext(options);
        using var metrics = new PostgreSqlOutboxRelayMetrics(context);
        using var listener = new RecordingMeterListener(MeterName);

        await metrics.RefreshAsync(ObservedAtUtc);
        listener.RecordObservableInstruments();

        AssertMeasurement(listener, EligibleCountInstrument, 0);
        AssertMeasurement(listener, OldestEligibleAgeInstrument, 0);
        AssertMeasurement(listener, RetryPendingCountInstrument, 0);
        AssertMeasurement(listener, StalledCountInstrument, 0);
    }

    private DbContextOptions<VendorRegistrationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<VendorRegistrationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;
    }

    private static async Task ResetAndSeedAsync(
        DbContextOptions<VendorRegistrationDbContext> options)
    {
        await ResetSchemaAsync(options);

        await using var context = new VendorRegistrationDbContext(options);

        AddWork(
            context,
            index: 1,
            registeredAtUtc: ObservedAtUtc.AddMinutes(-5));
        AddWork(
            context,
            index: 2,
            registeredAtUtc: ObservedAtUtc.AddMinutes(-4),
            attemptCount: 1,
            nextAttemptAtUtc: ObservedAtUtc.AddSeconds(-1));
        AddWork(
            context,
            index: 3,
            registeredAtUtc: ObservedAtUtc.AddMinutes(-3),
            attemptCount: 1,
            nextAttemptAtUtc: ObservedAtUtc.AddMinutes(1));
        AddWork(
            context,
            index: 4,
            registeredAtUtc: ObservedAtUtc.AddMinutes(-2),
            attemptCount: 1,
            claimedBy: Guid.Parse(
                "40000000-0000-0000-0000-000000000004"),
            claimExpiresAtUtc: ObservedAtUtc.AddMinutes(1));
        AddWork(
            context,
            index: 5,
            registeredAtUtc: ObservedAtUtc.AddMinutes(-10),
            attemptCount: 3,
            isStalled: true);
        AddWork(
            context,
            index: 6,
            registeredAtUtc: ObservedAtUtc.AddMinutes(-20),
            publishedAtUtc: ObservedAtUtc.AddMinutes(-19));

        await context.SaveChangesAsync();
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

    private static void AddWork(
        VendorRegistrationDbContext context,
        int index,
        DateTimeOffset registeredAtUtc,
        int attemptCount = 0,
        DateTimeOffset? nextAttemptAtUtc = null,
        Guid? claimedBy = null,
        DateTimeOffset? claimExpiresAtUtc = null,
        bool isStalled = false,
        DateTimeOffset? publishedAtUtc = null)
    {
        Guid vendorId = Guid.Parse(
            $"00000000-0000-0000-0000-{index + 700:D12}");
        Guid eventId = Guid.Parse(
            $"10000000-0000-0000-0000-{index + 700:D12}");

        context.Set<VendorRegistrationRecord>().Add(
            VendorRegistrationRecordMapper.ToRecord(
                CreateVendor(vendorId, index, registeredAtUtc)));
        context.Set<VendorRegistrationOutboxRecord>().Add(
            new VendorRegistrationOutboxRecord
            {
                EventId = eventId,
                VendorId = vendorId,
                EventVersion = 1,
                SerializedEvent = [9, 8, 7, checked((byte)index)],
                AttemptCount = attemptCount,
                NextAttemptAtUtc = nextAttemptAtUtc,
                ClaimedBy = claimedBy,
                ClaimExpiresAtUtc = claimExpiresAtUtc,
                LastAttemptAtUtc = attemptCount == 0
                    ? null
                    : registeredAtUtc.AddMinutes(1),
                LastFailureCategory = attemptCount == 0
                    ? null
                    : OutboxRelayFailureCategory.PublicationFailed,
                IsStalled = isStalled,
                PublishedAtUtc = publishedAtUtc
            });
    }

    private static VendorAggregate CreateVendor(
        Guid vendorId,
        int index,
        DateTimeOffset registeredAtUtc)
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName($"Metrics Operator {index}"),
            new VendorName($"Metrics Vendor {index}"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                $"Metrics Contact {index}",
                new EmailAddress($"metrics{index}@example.test"),
                new TelephoneNumber("+442079460123")),
            new CanonicalAddressId($"metrics-address-{index}"),
            new BusinessAddressSnapshot(
                $"{index} Metrics Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(
                    new TimeOnly(9, 0),
                    new TimeOnly(17, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(vendorId),
            information,
            website: null,
            businessDescription: null,
            registeredAtUtc);
    }

    private static void AssertMeasurement(
        RecordingMeterListener listener,
        string instrumentName,
        long expectedValue)
    {
        MetricMeasurement measurement = Assert.Single(
            listener.Measurements,
            item => item.InstrumentName == instrumentName);

        Assert.Equal(expectedValue, measurement.Value);
        Assert.Empty(measurement.Tags);
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

        public void RecordObservableInstruments()
        {
            _listener.RecordObservableInstruments();
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }

    private sealed record MetricMeasurement(
        string InstrumentName,
        long Value,
        IReadOnlyDictionary<string, object?> Tags);
}
