namespace HotJoes.Infrastructure.CommunityConsumer;
public interface ICommunityReceiptStore
{
    Task<CommunityReceiptOutcome> ClassifyAsync(CommunityReceiptCandidate candidate,
        CancellationToken cancellationToken = default);
}
