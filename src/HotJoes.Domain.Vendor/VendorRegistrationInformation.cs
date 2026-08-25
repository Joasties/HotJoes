namespace HotJoes.Domain.Vendor;

public sealed record VendorRegistrationInformation
{
    public VendorRegistrationInformation(
        LegalOperatorType legalOperatorType,
        VendorName legalOperatorName,
        VendorName tradingName,
        CompanyRegistrationNumber? companyRegistrationNumber,
        PrimaryContact primaryContact,
        CanonicalAddressId canonicalAddressId,
        BusinessAddressSnapshot businessAddressSnapshot,
        FoodRegistrationAuthority foodRegistrationAuthority,
        PrimaryTradingAuthority? primaryTradingAuthority,
        TradingCharacteristics tradingCharacteristics)
    {
        var companyRegistrationNumberRequired = legalOperatorType is
            LegalOperatorType.LimitedCompany or
            LegalOperatorType.LimitedLiabilityPartnership or
            LegalOperatorType.CharitableIncorporatedOrganisation;

        if (companyRegistrationNumberRequired != (companyRegistrationNumber is not null))
        {
            throw new ArgumentException(
                "Company Registration Number presence conflicts with Legal Operator Type.",
                nameof(companyRegistrationNumber));
        }

        var primaryTradingAuthorityRequired =
            tradingCharacteristics.TradingLocation is TradingLocation.Stall;

        if (primaryTradingAuthorityRequired != (primaryTradingAuthority is not null))
        {
            throw new ArgumentException(
                "Primary Trading Authority presence conflicts with Trading Location.",
                nameof(primaryTradingAuthority));
        }

        LegalOperatorType = legalOperatorType;
        LegalOperatorName = legalOperatorName;
        TradingName = tradingName;
        CompanyRegistrationNumber = companyRegistrationNumber;
        PrimaryContact = primaryContact;
        CanonicalAddressId = canonicalAddressId;
        BusinessAddressSnapshot = businessAddressSnapshot;
        FoodRegistrationAuthority = foodRegistrationAuthority;
        PrimaryTradingAuthority = primaryTradingAuthority;
        TradingCharacteristics = tradingCharacteristics;
    }

    public LegalOperatorType LegalOperatorType { get; }
    public VendorName LegalOperatorName { get; }
    public VendorName TradingName { get; }
    public CompanyRegistrationNumber? CompanyRegistrationNumber { get; }
    public PrimaryContact PrimaryContact { get; }
    public CanonicalAddressId CanonicalAddressId { get; }
    public BusinessAddressSnapshot BusinessAddressSnapshot { get; }
    public FoodRegistrationAuthority FoodRegistrationAuthority { get; }
    public PrimaryTradingAuthority? PrimaryTradingAuthority { get; }
    public TradingCharacteristics TradingCharacteristics { get; }
}
