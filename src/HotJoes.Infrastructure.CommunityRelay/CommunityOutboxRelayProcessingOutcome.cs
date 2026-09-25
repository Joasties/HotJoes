namespace HotJoes.Infrastructure.CommunityRelay;

public enum CommunityOutboxRelayProcessingOutcome
{
    Published = 1,
    RetryScheduled = 2
}
