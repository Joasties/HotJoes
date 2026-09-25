using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class DetermineRequiredLicenceTypesRequest
{
    public DetermineRequiredLicenceTypesRequest(
        LegalOperatorType legalOperatorType,
        TradingLocation tradingLocation,
        RegisterVendorWeeklyOpeningHours weeklyOpeningHours,
        bool serviceIncludesHotFood,
        bool alcoholService,
        string addressResolutionReference)
    {
        ArgumentNullException.ThrowIfNull(weeklyOpeningHours);
        ArgumentNullException.ThrowIfNull(addressResolutionReference);

        LegalOperatorType = legalOperatorType;
        TradingLocation = tradingLocation;
        WeeklyOpeningHours = weeklyOpeningHours;
        ServiceIncludesHotFood = serviceIncludesHotFood;
        AlcoholService = alcoholService;
        AddressResolutionReference = addressResolutionReference;
    }

    public LegalOperatorType LegalOperatorType { get; }
    public TradingLocation TradingLocation { get; }
    public RegisterVendorWeeklyOpeningHours WeeklyOpeningHours { get; }
    public bool ServiceIncludesHotFood { get; }
    public bool AlcoholService { get; }
    public string AddressResolutionReference { get; }
}
