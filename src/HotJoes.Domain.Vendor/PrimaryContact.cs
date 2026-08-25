namespace HotJoes.Domain.Vendor;

public sealed record PrimaryContact
{
    public PrimaryContact(
        string contactName,
        EmailAddress emailAddress,
        TelephoneNumber telephoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contactName);

        if (contactName.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(contactName));
        }

        ContactName = contactName;
        EmailAddress = emailAddress;
        TelephoneNumber = telephoneNumber;
    }

    public string ContactName { get; }
    public EmailAddress EmailAddress { get; }
    public TelephoneNumber TelephoneNumber { get; }
}
