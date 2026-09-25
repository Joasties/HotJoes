namespace HotJoes.Application.Community;

public interface ICommunityParticipationCommitter
{
    ValueTask<CommunityParticipationCommitResult> CommitAsync(
        JoinCommunityRequest request,
        CancellationToken cancellationToken = default);
}
