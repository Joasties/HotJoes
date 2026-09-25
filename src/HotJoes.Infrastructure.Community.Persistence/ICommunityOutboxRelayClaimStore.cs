namespace HotJoes.Infrastructure.Community.Persistence;

public interface ICommunityOutboxRelayClaimStore
    : ICommunityOutboxRelayStore
{
    Task<IReadOnlyList<CommunityOutboxRelayClaim>> ClaimEligibleAsync(
        Guid workerId,
        DateTimeOffset claimedAtUtc,
        TimeSpan leaseDuration,
        int batchSize,
        CancellationToken cancellationToken = default);

    Task<bool> RequeueStalledAsync(
        Guid eventId,
        DateTimeOffset requeuedAtUtc,
        CancellationToken cancellationToken = default);
}
