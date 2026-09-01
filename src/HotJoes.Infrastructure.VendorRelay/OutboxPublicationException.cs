namespace HotJoes.Infrastructure.VendorRelay;

public sealed class OutboxPublicationException : Exception
{
    public OutboxPublicationException(string message)
        : base(message)
    {
    }

    public OutboxPublicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
