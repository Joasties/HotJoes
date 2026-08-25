namespace HotJoes.Domain.Vendor;

public sealed class Vendor
{
    private readonly List<object> _domainEvents = [];

    private Vendor(
        VendorId id,
        VendorRegistrationInformation registeredInformation,
        Uri? website,
        string? businessDescription,
        DateTimeOffset registeredAt,
        VendorState state,
        TradingPreference tradingPreference,
        bool recordRegistrationEvent)
    {
        Id = id;
        RegisteredInformation = registeredInformation;
        Website = website;
        BusinessDescription = businessDescription;
        RegisteredAt = registeredAt;
        State = state;
        TradingPreference = tradingPreference;

        if (recordRegistrationEvent)
        {
            _domainEvents.Add(new VendorRegistered());
        }
    }

    public VendorId Id { get; }
    public VendorRegistrationInformation RegisteredInformation { get; }
    public Uri? Website { get; }
    public string? BusinessDescription { get; }
    public VendorState State { get; private set; }
    public TradingPreference TradingPreference { get; private set; }
    public DateTimeOffset RegisteredAt { get; }
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    public static Vendor Register(
        VendorId vendorId,
        VendorRegistrationInformation registrationInformation,
        Uri? website,
        string? businessDescription,
        DateTimeOffset registeredAt)
    {
        ValidateRegistrationValues(
            registrationInformation,
            website,
            businessDescription);

        return new Vendor(
            vendorId,
            registrationInformation,
            website,
            businessDescription,
            registeredAt,
            VendorState.PendingActivation,
            TradingPreference.Offline,
            recordRegistrationEvent: true);
    }

    internal static Vendor Rehydrate(
        VendorId vendorId,
        VendorRegistrationInformation registrationInformation,
        Uri? website,
        string? businessDescription,
        DateTimeOffset registeredAt,
        VendorState state,
        TradingPreference tradingPreference)
    {
        ValidateRegistrationValues(
            registrationInformation,
            website,
            businessDescription);

        return new Vendor(
            vendorId,
            registrationInformation,
            website,
            businessDescription,
            registeredAt,
            state,
            tradingPreference,
            recordRegistrationEvent: false);
    }

    private static void ValidateRegistrationValues(
        VendorRegistrationInformation registrationInformation,
        Uri? website,
        string? businessDescription)
    {
        ArgumentNullException.ThrowIfNull(registrationInformation);

        if (website is not null && website.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException(
                "Vendor website must use HTTPS.",
                nameof(website));
        }

        if (businessDescription?.Length > 2_000)
        {
            throw new ArgumentOutOfRangeException(nameof(businessDescription));
        }
    }
}
