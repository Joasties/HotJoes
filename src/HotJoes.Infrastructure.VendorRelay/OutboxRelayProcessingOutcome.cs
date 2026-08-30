namespace HotJoes.Infrastructure.VendorRelay;

public enum OutboxRelayProcessingOutcome
{
    Published = 1,
    RetryScheduled = 2
}
