namespace HotJoes.Api.Vendor;

public sealed class DetermineRequiredLicenceTypesRequest
{
    public string? LegalOperatorType { get; init; }

    public string? TradingLocation { get; init; }

    public RegisterVendorWeeklyOpeningHoursRequest? WeeklyOpeningHours { get; init; }

    public bool? ServiceIncludesHotFood { get; init; }

    public bool? AlcoholService { get; init; }

    public string? AddressResolutionReference { get; init; }
}
