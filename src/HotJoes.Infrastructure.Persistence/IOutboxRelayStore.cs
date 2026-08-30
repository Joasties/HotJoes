namespace HotJoes.Infrastructure.Persistence;

public interface IOutboxRelayStore
{
    Task MarkPublishedAsync(
        Guid eventId,
        Guid workerId,
        DateTimeOffset publishedAtUtc,
        CancellationToken cancellationToken = default);

    Task RecordFailureAsync(
        Guid eventId,
        Guid workerId,
        DateTimeOffset failedAtUtc,
        OutboxRelayFailureCategory failureCategory,
        OutboxRelayRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default);
}
