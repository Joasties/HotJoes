namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed class DeterministicCommunityParticipationRecordedStub
    : ICommunityParticipationRecordedProcessor
{
    public Task ProcessAsync(CommunityParticipationRecordedMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
