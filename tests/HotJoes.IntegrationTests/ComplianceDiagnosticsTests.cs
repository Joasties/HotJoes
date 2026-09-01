using System.Diagnostics.Metrics;
using System.Text;
using HotJoes.Infrastructure.ComplianceConsumer;
using Microsoft.Extensions.Logging;

namespace HotJoes.IntegrationTests;

[Collection(ComplianceObservabilityTestCollection.Name)]
public sealed class ComplianceDiagnosticsTests
{
    private const string MeterName =
        "HotJoes.Infrastructure.ComplianceConsumer";
    private const string ConsumerOutcomeInstrument =
        "hotjoes.compliance.consumer.outcomes";
    private const string DuplicateReceiptInstrument =
        "hotjoes.compliance.duplicate_receipts";
    private const string RecoveryRouteInstrument =
        "hotjoes.compliance.recovery.routes";
    private const string SensitiveMarker =
        "2 High Street secret-connection-string";

    private static readonly Guid EventId = Guid.Parse(
        "81d2a757-fefd-42c9-bd82-e3ebc3a09146");

    private static readonly DateTimeOffset ReceivedAtUtc = new(
        2026,
        8,
        29,
        19,
        0,
        0,
        TimeSpan.Zero);

    private static readonly byte[] SerializedEvent = Encoding.UTF8.GetBytes(
        $$"""
        {
          "eventId": "{{EventId:D}}",
          "eventType": "VendorRegistered",
          "eventVersion": 1,
          "occurredAt": "2026-08-29T18:00:00.0000000Z",
          "payload": {
            "vendorId": "4e512746-8714-4e31-8b29-e8a262dd54b2",
            "registeredAt": "2026-08-29T18:00:00.0000000Z",
            "vendorState": "pendingActivation",
            "tradingPreference": "online",
            "legalOperatorType": "soleTrader",
            "tradingCharacteristics": {
              "tradingLocation": "restaurant",
              "openingHours": {
                "startTime": "09:00:00",
                "endTime": "17:00:00"
              },
              "serviceIncludesHotFood": true,
              "alcoholService": false
            },
            "businessAddress": {
              "canonicalAddressId": "address-123",
              "recipientOrOrganisationName": null,
              "addressLine1": "{{SensitiveMarker}}",
              "addressLine2": null,
              "addressLine3": null,
              "postTown": "LEATHERHEAD",
              "postcode": "KT22 7QS",
              "county": null
            },
            "foodRegistrationAuthority": "Mole Valley District Council",
            "primaryTradingAuthority": null
          }
        }
        """);

    [Fact]
    public async Task ProcessAsync_NewAndDuplicateReceipts_EmitSafeStructuredDiagnostics()
    {
        var logger = new RecordingLogger<ComplianceDeliveryProcessor>();
        using var metrics = new RecordingMeterListener(MeterName);

        await ProcessAsync(
            ComplianceReceiptOutcome.Recorded,
            logger);
        await ProcessAsync(
            ComplianceReceiptOutcome.EquivalentDuplicate,
            logger);

        Assert.Collection(
            logger.Entries,
            entry => AssertReceiptEntry(
                entry,
                LogLevel.Information,
                "newReceipt"),
            entry => AssertReceiptEntry(
                entry,
                LogLevel.Information,
                "equivalentDuplicate"));

        AssertMetric(metrics, ConsumerOutcomeInstrument, "newReceipt");
        AssertMetric(
            metrics,
            ConsumerOutcomeInstrument,
            "equivalentDuplicate");
        MetricMeasurement duplicate = Assert.Single(
            metrics.Measurements,
            item => item.InstrumentName == DuplicateReceiptInstrument);
        Assert.Equal(1, duplicate.Value);
        Assert.Empty(duplicate.Tags);
    }

    [Fact]
    public async Task ProcessAsync_ConflictingReceipt_EmitsSafeWarningAndOutcomeMetric()
    {
        var logger = new RecordingLogger<ComplianceDeliveryProcessor>();
        using var metrics = new RecordingMeterListener(MeterName);

        ComplianceDeliveryOutcome outcome = await ProcessAsync(
            ComplianceReceiptOutcome.ConflictingBytes,
            logger);

        Assert.Equal(ComplianceDeliveryOutcome.ConflictingBytes, outcome);
        AssertReceiptEntry(
            Assert.Single(logger.Entries),
            LogLevel.Warning,
            "conflictingBytes");
        AssertMetric(
            metrics,
            ConsumerOutcomeInstrument,
            "conflictingBytes");
        Assert.DoesNotContain(
            metrics.Measurements,
            item => item.InstrumentName == DuplicateReceiptInstrument);
    }

    [Fact]
    public async Task RecoverAsync_RetryAndDeadLetter_EmitSafeAttemptAndRouteDiagnostics()
    {
        var logger =
            new RecordingLogger<ComplianceDeliveryRecoveryHandler>();
        using var metrics = new RecordingMeterListener(MeterName);
        var handler = new ComplianceDeliveryRecoveryHandler(
            new ComplianceConsumerRetryPolicy(
                maximumAutomaticAttempts: 3,
                retryDelay: TimeSpan.FromSeconds(1)),
            new ComplianceRecoveryDispatcher(
                new ConfirmingRecoveryPublisher()),
            logger);

        ComplianceRecoveryRoute retry = await handler.RecoverAsync(
            EventId,
            eventVersion: 1,
            SerializedEvent,
            currentAutomaticAttempt: 1,
            failureCategory: "receiptUnavailable",
            retryable: true,
            new RecordingAcknowledgement());
        ComplianceRecoveryRoute deadLetter = await handler.RecoverAsync(
            EventId,
            eventVersion: 1,
            SerializedEvent,
            currentAutomaticAttempt: 3,
            failureCategory: "receiptUnavailable",
            retryable: true,
            new RecordingAcknowledgement());

        Assert.Equal(ComplianceRecoveryRoute.Retry, retry);
        Assert.Equal(ComplianceRecoveryRoute.DeadLetter, deadLetter);
        Assert.Collection(
            logger.Entries,
            entry => AssertRecoveryEntry(
                entry,
                "retry",
                automaticAttempt: 2),
            entry => AssertRecoveryEntry(
                entry,
                "deadLetter",
                automaticAttempt: 3));
        AssertMetric(metrics, RecoveryRouteInstrument, "retry");
        AssertMetric(metrics, RecoveryRouteInstrument, "deadLetter");
    }

    private static async Task<ComplianceDeliveryOutcome> ProcessAsync(
        ComplianceReceiptOutcome receiptOutcome,
        RecordingLogger<ComplianceDeliveryProcessor> logger)
    {
        var processor = new ComplianceDeliveryProcessor(
            new StubReceiptStore(receiptOutcome),
            logger);

        return await processor.ProcessAsync(
            SerializedEvent,
            ReceivedAtUtc,
            new RecordingAcknowledgement());
    }

    private static void AssertReceiptEntry(
        LogEntry entry,
        LogLevel expectedLevel,
        string expectedOutcome)
    {
        Assert.Equal(expectedLevel, entry.Level);
        Assert.Equal(EventId, entry.Properties["EventId"]);
        Assert.Equal("VendorRegistered", entry.Properties["EventType"]);
        Assert.Equal(1, entry.Properties["EventVersion"]);
        Assert.Equal(
            expectedOutcome,
            entry.Properties["ConsumerReceiptOutcome"]);
        AssertSafe(entry);
    }

    private static void AssertRecoveryEntry(
        LogEntry entry,
        string expectedOutcome,
        int automaticAttempt)
    {
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal(EventId, entry.Properties["EventId"]);
        Assert.Equal("VendorRegistered", entry.Properties["EventType"]);
        Assert.Equal(1, entry.Properties["EventVersion"]);
        Assert.Equal(
            automaticAttempt,
            entry.Properties["ConsumerAttempt"]);
        Assert.Equal(
            expectedOutcome,
            entry.Properties["ConsumerRecoveryOutcome"]);
        Assert.Equal(
            "receiptUnavailable",
            entry.Properties["FailureCategory"]);
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
        Assert.All(
            entry.Properties.Values,
            value => Assert.DoesNotContain(
                SensitiveMarker,
                value?.ToString() ?? string.Empty,
                StringComparison.Ordinal));
    }

    private sealed class StubReceiptStore : IComplianceReceiptStore
    {
        private readonly ComplianceReceiptOutcome _outcome;

        public StubReceiptStore(ComplianceReceiptOutcome outcome)
        {
            _outcome = outcome;
        }

        public Task<ComplianceReceiptOutcome> RecordAsync(
            ComplianceReceiptCandidate candidate,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_outcome);
        }
    }

    private sealed class ConfirmingRecoveryPublisher
        : IComplianceRecoveryPublisher
    {
        public Task PublishAsync(
            ComplianceRecoveryRoute route,
            ComplianceRecoveryPublication publication,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingAcknowledgement
        : IComplianceDeliveryAcknowledgement
    {
        public Task AcknowledgeAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
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
            EventId eventId,
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
