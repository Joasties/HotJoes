namespace HotJoes.Application.Community;

public interface IJoinCommunityService
{
    Task<JoinCommunityResult> JoinAsync(
        JoinCommunityRequest request,
        CancellationToken cancellationToken = default);
}
