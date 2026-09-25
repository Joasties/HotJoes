namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class SystemCommunityPersistenceIdentityGenerator
    : ICommunityPersistenceIdentityGenerator
{
    public Guid NewCommunityParticipationId() => Guid.NewGuid();

    public Guid NewEventId() => Guid.NewGuid();
}
