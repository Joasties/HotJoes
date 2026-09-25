using ComplianceApplication = HotJoes.Application.Compliance;
using VendorApplication = HotJoes.Application.Vendor;
using VendorDomain = HotJoes.Domain.Vendor;

namespace HotJoes.Infrastructure.Vendor.Compliance;

public sealed class ComplianceDeterminationAdapter
    : VendorApplication.IComplianceDeterminationPort
{
    private readonly ComplianceApplication.IComplianceDeterminationService
        _complianceService;

    public ComplianceDeterminationAdapter(
        ComplianceApplication.IComplianceDeterminationService complianceService)
    {
        ArgumentNullException.ThrowIfNull(complianceService);
        _complianceService = complianceService;
    }

    public VendorApplication.ComplianceDeterminationPortResult Determine(
        VendorApplication.ComplianceDeterminationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ComplianceApplication.ComplianceDeterminationResult result =
            _complianceService.Determine(TranslateRequest(request));

        return result switch
        {
            ComplianceApplication.ComplianceDeterminationResult.Success success =>
                VendorApplication.ComplianceDeterminationPortResult.Succeeded(
                    TranslateDetermination(success.Determination)),
            ComplianceApplication.ComplianceDeterminationResult.UnsupportedCoverage =>
                VendorApplication.ComplianceDeterminationPortResult.Unsupported(),
            ComplianceApplication.ComplianceDeterminationResult.TemporaryFailure =>
                VendorApplication.ComplianceDeterminationPortResult
                    .TemporarilyUnavailable(),
            _ => throw new InvalidOperationException(
                "The Compliance Application result is unsupported.")
        };
    }

    private static ComplianceApplication.ComplianceDeterminationRequest
        TranslateRequest(
            VendorApplication.ComplianceDeterminationRequest request) =>
        new(
            MapLegalOperatorType(request.LegalOperatorType),
            MapTradingLocation(request.TradingLocation),
            new ComplianceApplication.WeeklyOpeningHours(
                request.WeeklyOpeningHours.Days.Select(day =>
                    new ComplianceApplication.DailyOpeningHours(
                        MapTradingDay(day.Day),
                        day.IsClosed,
                        day.IsOpenAllDay,
                        day.StartTime,
                        day.EndTime))),
            request.ServiceIncludesHotFood,
            request.AlcoholService,
            new ComplianceApplication.BusinessAddress(
                request.BusinessAddress.AddressLine1,
                request.BusinessAddress.AddressLine2,
                request.BusinessAddress.AddressLine3,
                request.BusinessAddress.PostTown,
                request.BusinessAddress.Postcode,
                request.BusinessAddress.County,
                request.BusinessAddress.RecipientOrOrganisationName),
            request.FoodRegistrationAuthority.Value,
            request.PrimaryTradingAuthority?.Value);

    private static VendorApplication.ComplianceDetermination
        TranslateDetermination(
            ComplianceApplication.ComplianceDetermination determination) =>
        new(
            determination.RuleSetVersion,
            determination.Items.Select(item =>
                new VendorApplication.ComplianceDeterminationItem(
                    MapRequiredLicenceType(item.RequiredLicenceType),
                    item.IsRequired)));

    private static ComplianceApplication.LegalOperatorType MapLegalOperatorType(
        VendorDomain.LegalOperatorType value) =>
        value switch
        {
            VendorDomain.LegalOperatorType.SoleTrader =>
                ComplianceApplication.LegalOperatorType.SoleTrader,
            VendorDomain.LegalOperatorType.GeneralPartnership =>
                ComplianceApplication.LegalOperatorType.GeneralPartnership,
            VendorDomain.LegalOperatorType.LimitedCompany =>
                ComplianceApplication.LegalOperatorType.LimitedCompany,
            VendorDomain.LegalOperatorType.LimitedLiabilityPartnership =>
                ComplianceApplication.LegalOperatorType
                    .LimitedLiabilityPartnership,
            VendorDomain.LegalOperatorType.CharitableCommunityGroup =>
                ComplianceApplication.LegalOperatorType
                    .CharitableCommunityGroup,
            VendorDomain.LegalOperatorType
                .CharitableIncorporatedOrganisation =>
                ComplianceApplication.LegalOperatorType
                    .CharitableIncorporatedOrganisation,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

    private static ComplianceApplication.TradingLocation MapTradingLocation(
        VendorDomain.TradingLocation value) =>
        value switch
        {
            VendorDomain.TradingLocation.Restaurant =>
                ComplianceApplication.TradingLocation.Restaurant,
            VendorDomain.TradingLocation.Stall =>
                ComplianceApplication.TradingLocation.Stall,
            VendorDomain.TradingLocation.Kitchen =>
                ComplianceApplication.TradingLocation.Kitchen,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

    private static ComplianceApplication.TradingDay MapTradingDay(
        VendorDomain.TradingDay value) =>
        value switch
        {
            VendorDomain.TradingDay.Monday =>
                ComplianceApplication.TradingDay.Monday,
            VendorDomain.TradingDay.Tuesday =>
                ComplianceApplication.TradingDay.Tuesday,
            VendorDomain.TradingDay.Wednesday =>
                ComplianceApplication.TradingDay.Wednesday,
            VendorDomain.TradingDay.Thursday =>
                ComplianceApplication.TradingDay.Thursday,
            VendorDomain.TradingDay.Friday =>
                ComplianceApplication.TradingDay.Friday,
            VendorDomain.TradingDay.Saturday =>
                ComplianceApplication.TradingDay.Saturday,
            VendorDomain.TradingDay.Sunday =>
                ComplianceApplication.TradingDay.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };

    private static VendorApplication.RequiredLicenceType MapRequiredLicenceType(
        ComplianceApplication.RequiredLicenceType value) =>
        value switch
        {
            ComplianceApplication.RequiredLicenceType
                .FoodBusinessRegistration =>
                VendorApplication.RequiredLicenceType
                    .FoodBusinessRegistration,
            ComplianceApplication.RequiredLicenceType
                .StreetTradingLicence =>
                VendorApplication.RequiredLicenceType.StreetTradingLicence,
            ComplianceApplication.RequiredLicenceType
                .LateNightRefreshmentLicence =>
                VendorApplication.RequiredLicenceType
                    .LateNightRefreshmentLicence,
            ComplianceApplication.RequiredLicenceType.PremisesLicence =>
                VendorApplication.RequiredLicenceType.PremisesLicence,
            ComplianceApplication.RequiredLicenceType.PersonalLicenceHolder =>
                VendorApplication.RequiredLicenceType.PersonalLicenceHolder,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
}
