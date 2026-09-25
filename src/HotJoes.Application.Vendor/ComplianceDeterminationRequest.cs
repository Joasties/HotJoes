using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class ComplianceDeterminationRequest
{
    public ComplianceDeterminationRequest(
        LegalOperatorType legalOperatorType,
        TradingLocation tradingLocation,
        RegisterVendorWeeklyOpeningHours weeklyOpeningHours,
        bool serviceIncludesHotFood,
        bool alcoholService,
        BusinessAddressSnapshot businessAddress,
        FoodRegistrationAuthority foodRegistrationAuthority,
        PrimaryTradingAuthority? primaryTradingAuthority)
    {
        ArgumentNullException.ThrowIfNull(weeklyOpeningHours);
        ArgumentNullException.ThrowIfNull(businessAddress);
        ArgumentNullException.ThrowIfNull(foodRegistrationAuthority);

        LegalOperatorType = legalOperatorType;
        TradingLocation = tradingLocation;
        WeeklyOpeningHours = weeklyOpeningHours;
        ServiceIncludesHotFood = serviceIncludesHotFood;
        AlcoholService = alcoholService;
        BusinessAddress = businessAddress;
        FoodRegistrationAuthority = foodRegistrationAuthority;
        PrimaryTradingAuthority = primaryTradingAuthority;
    }

    public LegalOperatorType LegalOperatorType { get; }
    public TradingLocation TradingLocation { get; }
    public RegisterVendorWeeklyOpeningHours WeeklyOpeningHours { get; }
    public bool ServiceIncludesHotFood { get; }
    public bool AlcoholService { get; }
    public BusinessAddressSnapshot BusinessAddress { get; }
    public FoodRegistrationAuthority FoodRegistrationAuthority { get; }
    public PrimaryTradingAuthority? PrimaryTradingAuthority { get; }
}
