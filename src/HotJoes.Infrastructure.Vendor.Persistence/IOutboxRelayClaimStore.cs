namespace HotJoes.Infrastructure.Vendor.Persistence;

public interface IOutboxRelayClaimStore : IOutboxRelayStore
{
    Task<IReadOnlyList<OutboxRelayClaim>> ClaimEligibleAsync(
        Guid workerId,
        DateTimeOffset claimedAtUtc,
        TimeSpan leaseDuration,
        int batchSize,
        CancellationToken cancellationToken = default);
}
