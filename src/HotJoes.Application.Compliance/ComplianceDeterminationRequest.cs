namespace HotJoes.Application.Compliance;

public sealed class ComplianceDeterminationRequest
{
    public ComplianceDeterminationRequest(
        LegalOperatorType legalOperatorType,
        TradingLocation tradingLocation,
        WeeklyOpeningHours weeklyOpeningHours,
        bool serviceIncludesHotFood,
        bool alcoholService,
        BusinessAddress businessAddress,
        string foodRegistrationAuthority,
        string? primaryTradingAuthority)
    {
        ArgumentNullException.ThrowIfNull(weeklyOpeningHours);
        ArgumentNullException.ThrowIfNull(businessAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(foodRegistrationAuthority);

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
    public WeeklyOpeningHours WeeklyOpeningHours { get; }
    public bool ServiceIncludesHotFood { get; }
    public bool AlcoholService { get; }
    public BusinessAddress BusinessAddress { get; }
    public string FoodRegistrationAuthority { get; }
    public string? PrimaryTradingAuthority { get; }
}
