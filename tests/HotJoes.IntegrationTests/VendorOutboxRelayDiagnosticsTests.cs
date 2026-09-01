using System.Diagnostics.Metrics;
using System.Text;
using HotJoes.Infrastructure.Persistence;
using HotJoes.Infrastructure.VendorRelay;
using Microsoft.Extensions.Logging;

namespace HotJoes.IntegrationTests;

[Collection(RelayObservabilityTestCollection.Name)]
public sealed class VendorOutboxRelayDiagnosticsTests
{
    private const string MeterName =
        "HotJoes.Infrastructure.VendorRelay";
    private const string PublicationInstrument =
        "hotjoes.vendor.relay.publications";
    private const string SensitiveMarker =
        "alex@example.test 2 High Street secret-connection-string";

    private static readonly Guid EventId = Guid.Parse(
        "683cf437-eb14-43d8-bb3a-75603496c92c");

    [Fact]
    public async Task ProcessAsync_ConfirmedPublication_EmitsSafeStructuredLogAndMetric()
    {
        var logger = new RecordingLogger<VendorOutboxRelayProcessor>();
        using var metrics = new RecordingMeterListener(
            MeterName,
            PublicationInstrument);
        var processor = new VendorOutboxRelayProcessor(
            new RecordingRelayStore(),
            new ConfirmingPublisher(),
            logger);
        var claim = new OutboxRelayClaim(
            EventId,
            1,
            Encoding.UTF8.GetBytes(SensitiveMarker),
            traceParent: null,
            traceState: null,
            attemptCount: 2);

        OutboxRelayProcessingOutcome outcome = await processor.ProcessAsync(
            claim,
            Guid.NewGuid(),
            new DateTimeOffset(2026, 8, 29, 17, 0, 0, TimeSpan.Zero),
            new OutboxRelayRetryPolicy(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(4),
                automaticAttemptLimit: 3));

        Assert.Equal(OutboxRelayProcessingOutcome.Published, outcome);
        LogEntry entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        AssertStructuredRelayProperties(entry, "published", attempt: 3);
        AssertSafe(entry);

        MetricMeasurement measurement = Assert.Single(metrics.Measurements);
        Assert.Equal(1, measurement.Value);
        Assert.Equal("published", measurement.Tags["outcome"]);
    }

    [Fact]
    public async Task ProcessAsync_PublicationFailure_EmitsSanitizedRetryDiagnostics()
    {
        var logger = new RecordingLogger<VendorOutboxRelayProcessor>();
        using var metrics = new RecordingMeterListener(
            MeterName,
            PublicationInstrument);
        var processor = new VendorOutboxRelayProcessor(
            new RecordingRelayStore(),
            new FailingPublisher(),
            logger);
        var claim = new OutboxRelayClaim(
            EventId,
            1,
            Encoding.UTF8.GetBytes(SensitiveMarker),
            traceParent: null,
            traceState: null,
            attemptCount: 0);

        OutboxRelayProcessingOutcome outcome = await processor.ProcessAsync(
            claim,
            Guid.NewGuid(),
            new DateTimeOffset(2026, 8, 29, 17, 0, 0, TimeSpan.Zero),
            new OutboxRelayRetryPolicy(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(4),
                automaticAttemptLimit: 3));

        Assert.Equal(OutboxRelayProcessingOutcome.RetryScheduled, outcome);
        LogEntry entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        AssertStructuredRelayProperties(
            entry,
            "retryScheduled",
            attempt: 1);
        Assert.Null(entry.Exception);
        AssertSafe(entry);

        MetricMeasurement measurement = Assert.Single(metrics.Measurements);
        Assert.Equal(1, measurement.Value);
        Assert.Equal("retryScheduled", measurement.Tags["outcome"]);
    }

    private static void AssertStructuredRelayProperties(
        LogEntry entry,
        string expectedOutcome,
        int attempt)
    {
        Assert.Equal(EventId, entry.Properties["EventId"]);
        Assert.Equal("VendorRegistered", entry.Properties["EventType"]);
        Assert.Equal(1, entry.Properties["EventVersion"]);
        Assert.Equal(attempt, entry.Properties["OutboxAttempt"]);
        Assert.Equal(expectedOutcome, entry.Properties["RelayOutcome"]);
    }

    private static void AssertSafe(LogEntry entry)
    {
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

    private sealed class ConfirmingPublisher : IOutboxEventPublisher
    {
        public Task<OutboxPublicationConfirmation> PublishAsync(
            OutboxPublication publication,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                OutboxPublicationConfirmation.Confirmed);
        }
    }

    private sealed class FailingPublisher : IOutboxEventPublisher
    {
        public Task<OutboxPublicationConfirmation> PublishAsync(
            OutboxPublication publication,
            CancellationToken cancellationToken = default)
        {
            throw new OutboxPublicationException(SensitiveMarker);
        }
    }

    private sealed class RecordingRelayStore : IOutboxRelayStore
    {
        public Task MarkPublishedAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset publishedAtUtc,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RecordFailureAsync(
            Guid eventId,
            Guid workerId,
            DateTimeOffset failedAtUtc,
            OutboxRelayFailureCategory failureCategory,
            OutboxRelayRetryPolicy retryPolicy,
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
            var properties = state is
                IEnumerable<KeyValuePair<string, object?>> values
                    ? values
                        .Where(value => value.Key != "{OriginalFormat}")
                        .ToDictionary(value => value.Key, value => value.Value)
                    : [];
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

        public RecordingMeterListener(
            string meterName,
            string instrumentName)
        {
            _listener.InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == meterName &&
                    instrument.Name == instrumentName)
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            };
            _listener.SetMeasurementEventCallback<long>(
                (instrument, measurement, tags, state) =>
                {
                    Measurements.Add(new MetricMeasurement(
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
        long Value,
        IReadOnlyDictionary<string, object?> Tags);
}
