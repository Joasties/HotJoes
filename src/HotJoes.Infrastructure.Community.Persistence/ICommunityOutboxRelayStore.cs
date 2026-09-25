namespace HotJoes.Infrastructure.Community.Persistence;

public interface ICommunityOutboxRelayStore
{
    Task MarkPublishedAsync(Guid eventId, Guid workerId,
        DateTimeOffset publishedAtUtc, CancellationToken cancellationToken = default);
    Task RecordFailureAsync(Guid eventId, Guid workerId,
        DateTimeOffset failedAtUtc, CommunityOutboxRelayFailureCategory failureCategory,
        CommunityOutboxRelayRetryPolicy retryPolicy,
        CancellationToken cancellationToken = default);
}
