using HotJoes.Application.Community;
using ApplicationRequest = HotJoes.Application.Community.JoinCommunityRequest;

namespace HotJoes.Api.Vendor.Tests;

public sealed class StubJoinCommunityService : IJoinCommunityService
{
    public JoinCommunityResult NextResult { get; set; } =
        JoinCommunityResult.VendorWasNotFound();

    public int InvocationCount { get; private set; }

    public ApplicationRequest? LastRequest { get; private set; }

    public bool LastCancellationTokenCanBeCanceled { get; private set; }

    public Task<JoinCommunityResult> JoinAsync(
        ApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        InvocationCount++;
        LastRequest = request;
        LastCancellationTokenCanBeCanceled = cancellationToken.CanBeCanceled;
        return Task.FromResult(NextResult);
    }
}
