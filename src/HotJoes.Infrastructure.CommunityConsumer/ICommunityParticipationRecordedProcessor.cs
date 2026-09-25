namespace HotJoes.Infrastructure.CommunityConsumer;
public interface ICommunityParticipationRecordedProcessor
{
    Task ProcessAsync(CommunityParticipationRecordedMessage message,
        CancellationToken cancellationToken = default);
}
