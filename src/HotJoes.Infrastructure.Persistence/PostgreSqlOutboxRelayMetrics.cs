using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.Persistence;

public sealed class PostgreSqlOutboxRelayMetrics : IDisposable
{
    private const string MeterName =
        "HotJoes.Infrastructure.Persistence";

    private readonly VendorRegistrationDbContext _context;
    private readonly Meter _meter = new(MeterName);

    private long _eligibleCount;
    private long _oldestEligibleAgeSeconds;
    private long _retryPendingCount;
    private long _stalledCount;

    public PostgreSqlOutboxRelayMetrics(
        VendorRegistrationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _meter.CreateObservableGauge(
            "hotjoes.vendor.outbox.eligible",
            () => Interlocked.Read(ref _eligibleCount),
            unit: "{work}",
            description: "Unpublished outbox work eligible for relay claim.");
        _meter.CreateObservableGauge(
            "hotjoes.vendor.outbox.oldest_eligible_age_seconds",
            () => Interlocked.Read(ref _oldestEligibleAgeSeconds),
            unit: "s",
            description: "Age in seconds of the oldest eligible outbox work.");
        _meter.CreateObservableGauge(
            "hotjoes.vendor.outbox.retry_pending",
            () => Interlocked.Read(ref _retryPendingCount),
            unit: "{work}",
            description: "Unpublished non-stalled outbox work with prior attempts.");
        _meter.CreateObservableGauge(
            "hotjoes.vendor.outbox.stalled",
            () => Interlocked.Read(ref _stalledCount),
            unit: "{work}",
            description: "Unpublished outbox work awaiting administrative requeue.");
    }

    public async Task RefreshAsync(
        DateTimeOffset observedAtUtc,
        CancellationToken cancellationToken = default)
    {
        DateTimeOffset observationTime = observedAtUtc.ToUniversalTime();

        IQueryable<DateTimeOffset> eligibleRegistrationTimes =
            from outbox in _context
                .Set<VendorRegistrationOutboxRecord>()
                .AsNoTracking()
            join vendor in _context
                .Set<VendorRegistrationRecord>()
                .AsNoTracking()
                on outbox.VendorId equals vendor.VendorId
            where outbox.PublishedAtUtc == null &&
                !outbox.IsStalled &&
                (outbox.NextAttemptAtUtc == null ||
                    outbox.NextAttemptAtUtc <= observationTime) &&
                (outbox.ClaimExpiresAtUtc == null ||
                    outbox.ClaimExpiresAtUtc <= observationTime)
            select vendor.RegisteredAtUtc;

        long eligibleCount = await eligibleRegistrationTimes.LongCountAsync(
            cancellationToken);
        DateTimeOffset? oldestEligibleAtUtc =
            await eligibleRegistrationTimes
                .Select(value => (DateTimeOffset?)value)
                .MinAsync(cancellationToken);

        long retryPendingCount = await _context
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .LongCountAsync(
                outbox => outbox.PublishedAtUtc == null &&
                    !outbox.IsStalled &&
                    outbox.AttemptCount > 0,
                cancellationToken);

        long stalledCount = await _context
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .LongCountAsync(
                outbox => outbox.PublishedAtUtc == null &&
                    outbox.IsStalled,
                cancellationToken);

        long oldestEligibleAgeSeconds = oldestEligibleAtUtc is null
            ? 0
            : checked((long)Math.Floor(Math.Max(
                0,
                (observationTime - oldestEligibleAtUtc.Value)
                    .TotalSeconds)));

        Interlocked.Exchange(ref _eligibleCount, eligibleCount);
        Interlocked.Exchange(
            ref _oldestEligibleAgeSeconds,
            oldestEligibleAgeSeconds);
        Interlocked.Exchange(ref _retryPendingCount, retryPendingCount);
        Interlocked.Exchange(ref _stalledCount, stalledCount);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}
