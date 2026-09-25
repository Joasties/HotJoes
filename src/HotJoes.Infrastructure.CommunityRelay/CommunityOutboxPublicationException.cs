namespace HotJoes.Infrastructure.CommunityRelay;

public sealed class CommunityOutboxPublicationException : Exception
{
    public CommunityOutboxPublicationException(string message) : base(message) { }
    public CommunityOutboxPublicationException(string message,
        Exception innerException) : base(message, innerException) { }
}
