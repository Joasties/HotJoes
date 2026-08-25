using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RegisteredVendorDetails
{
    public RegisteredVendorDetails(
        VendorId vendorId,
        DateTimeOffset registeredAt,
        VendorState vendorState,
        TradingPreference tradingPreference,
        LegalOperatorType legalOperatorType,
        string legalOperatorName,
        string? companyRegistrationNumber,
        string tradingName,
        RegisteredVendorTradingCharacteristics tradingCharacteristics,
        string contactName,
        string contactEmail,
        string contactTelephone,
        string canonicalAddressId,
        RegisteredVendorBusinessAddress businessAddress,
        string foodRegistrationAuthority,
        string? primaryTradingAuthority,
        string? website,
        string? businessDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(legalOperatorName);
        ArgumentException.ThrowIfNullOrWhiteSpace(tradingName);
        ArgumentNullException.ThrowIfNull(tradingCharacteristics);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactTelephone);
        ArgumentException.ThrowIfNullOrWhiteSpace(canonicalAddressId);
        ArgumentNullException.ThrowIfNull(businessAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(foodRegistrationAuthority);

        VendorId = vendorId;
        RegisteredAt = registeredAt;
        VendorState = vendorState;
        TradingPreference = tradingPreference;
        LegalOperatorType = legalOperatorType;
        LegalOperatorName = legalOperatorName;
        CompanyRegistrationNumber = companyRegistrationNumber;
        TradingName = tradingName;
        TradingCharacteristics = tradingCharacteristics;
        ContactName = contactName;
        ContactEmail = contactEmail;
        ContactTelephone = contactTelephone;
        CanonicalAddressId = canonicalAddressId;
        BusinessAddress = businessAddress;
        FoodRegistrationAuthority = foodRegistrationAuthority;
        PrimaryTradingAuthority = primaryTradingAuthority;
        Website = website;
        BusinessDescription = businessDescription;
    }

    public VendorId VendorId { get; }

    public DateTimeOffset RegisteredAt { get; }

    public VendorState VendorState { get; }

    public TradingPreference TradingPreference { get; }

    public LegalOperatorType LegalOperatorType { get; }

    public string LegalOperatorName { get; }

    public string? CompanyRegistrationNumber { get; }

    public string TradingName { get; }

    public RegisteredVendorTradingCharacteristics TradingCharacteristics { get; }

    public string ContactName { get; }

    public string ContactEmail { get; }

    public string ContactTelephone { get; }

    public string CanonicalAddressId { get; }

    public RegisteredVendorBusinessAddress BusinessAddress { get; }

    public string FoodRegistrationAuthority { get; }

    public string? PrimaryTradingAuthority { get; }

    public string? Website { get; }

    public string? BusinessDescription { get; }
}
