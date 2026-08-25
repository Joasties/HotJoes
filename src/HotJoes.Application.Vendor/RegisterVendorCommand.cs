using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RegisterVendorCommand
{
    public RegisterVendorCommand(
        string tradingName,
        string legalOperatorName,
        LegalOperatorType legalOperatorType,
        string? companyRegistrationNumber,
        TradingLocation tradingLocation,
        TimeOnly openingHoursStartTime,
        TimeOnly openingHoursEndTime,
        bool serviceIncludesHotFood,
        bool alcoholService,
        string contactName,
        string contactEmail,
        string contactTelephone,
        string addressResolutionReference,
        string? website,
        string? businessDescription,
        bool authorisedToRegisterBusiness,
        bool informationAccurate,
        bool acceptHotJoesPlatformTerms)
    {
        TradingName = tradingName;
        LegalOperatorName = legalOperatorName;
        LegalOperatorType = legalOperatorType;
        CompanyRegistrationNumber = companyRegistrationNumber;
        TradingLocation = tradingLocation;
        OpeningHoursStartTime = openingHoursStartTime;
        OpeningHoursEndTime = openingHoursEndTime;
        ServiceIncludesHotFood = serviceIncludesHotFood;
        AlcoholService = alcoholService;
        ContactName = contactName;
        ContactEmail = contactEmail;
        ContactTelephone = contactTelephone;
        AddressResolutionReference = addressResolutionReference;
        Website = website;
        BusinessDescription = businessDescription;
        AuthorisedToRegisterBusiness = authorisedToRegisterBusiness;
        InformationAccurate = informationAccurate;
        AcceptHotJoesPlatformTerms = acceptHotJoesPlatformTerms;
    }

    public string TradingName { get; }

    public string LegalOperatorName { get; }

    public LegalOperatorType LegalOperatorType { get; }

    public string? CompanyRegistrationNumber { get; }

    public TradingLocation TradingLocation { get; }

    public TimeOnly OpeningHoursStartTime { get; }

    public TimeOnly OpeningHoursEndTime { get; }

    public bool ServiceIncludesHotFood { get; }

    public bool AlcoholService { get; }

    public string ContactName { get; }

    public string ContactEmail { get; }

    public string ContactTelephone { get; }

    public string AddressResolutionReference { get; }

    public string? Website { get; }

    public string? BusinessDescription { get; }

    public bool AuthorisedToRegisterBusiness { get; }

    public bool InformationAccurate { get; }

    public bool AcceptHotJoesPlatformTerms { get; }
}
