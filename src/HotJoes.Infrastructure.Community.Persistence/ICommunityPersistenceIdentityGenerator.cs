namespace HotJoes.Infrastructure.Community.Persistence;

public interface ICommunityPersistenceIdentityGenerator
{
    Guid NewCommunityParticipationId();

    Guid NewEventId();
}
