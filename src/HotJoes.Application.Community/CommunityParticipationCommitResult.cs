namespace HotJoes.Application.Community;

public abstract class CommunityParticipationCommitResult
{
    private CommunityParticipationCommitResult()
    {
    }

    public static CommunityParticipationCommitResult Recorded(
        CommunityParticipation participation)
    {
        ArgumentNullException.ThrowIfNull(participation);
        return new FirstRecorded(participation);
    }

    public static CommunityParticipationCommitResult AlreadyRecorded(
        CommunityParticipation participation)
    {
        ArgumentNullException.ThrowIfNull(participation);
        return new EquivalentReplay(participation);
    }

    public static CommunityParticipationCommitResult Conflict() =>
        new PreferenceConflict();

    public static CommunityParticipationCommitResult
        TemporarilyUnavailable() => new PersistenceUnavailable();

    public sealed class FirstRecorded : CommunityParticipationCommitResult
    {
        internal FirstRecorded(CommunityParticipation participation)
        {
            Participation = participation;
        }

        public CommunityParticipation Participation { get; }
    }

    public sealed class EquivalentReplay
        : CommunityParticipationCommitResult
    {
        internal EquivalentReplay(CommunityParticipation participation)
        {
            Participation = participation;
        }

        public CommunityParticipation Participation { get; }
    }

    public sealed class PreferenceConflict
        : CommunityParticipationCommitResult
    {
        internal PreferenceConflict()
        {
        }
    }

    public sealed class PersistenceUnavailable
        : CommunityParticipationCommitResult
    {
        internal PersistenceUnavailable()
        {
        }
    }
}
