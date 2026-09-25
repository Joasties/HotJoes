namespace HotJoes.Application.Compliance;

public sealed class StubComplianceApplication
    : IComplianceDeterminationService
{
    private const string ActiveRuleSetVersion = "epic-1-v1";
    private const string SupportedAuthority =
        "Royal Borough of Greenwich";
    private static readonly TimeOnly LateNightStart = new(23, 0);
    private static readonly TimeOnly LateNightEnd = new(5, 0);

    public ComplianceDeterminationResult Determine(
        ComplianceDeterminationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!HasSupportedCoverage(request))
        {
            return ComplianceDeterminationResult.Unsupported();
        }

        bool streetTradingLicence =
            request.TradingLocation == TradingLocation.Stall;
        bool lateNightRefreshmentLicence =
            request.ServiceIncludesHotFood
            && request.WeeklyOpeningHours.Days.Any(OverlapsLateNightWindow);

        return ComplianceDeterminationResult.Succeeded(
            new ComplianceDetermination(
                ActiveRuleSetVersion,
                [
                    new ComplianceDeterminationItem(
                        RequiredLicenceType.FoodBusinessRegistration,
                        true),
                    new ComplianceDeterminationItem(
                        RequiredLicenceType.StreetTradingLicence,
                        streetTradingLicence),
                    new ComplianceDeterminationItem(
                        RequiredLicenceType.LateNightRefreshmentLicence,
                        lateNightRefreshmentLicence),
                    new ComplianceDeterminationItem(
                        RequiredLicenceType.PremisesLicence,
                        request.AlcoholService),
                    new ComplianceDeterminationItem(
                        RequiredLicenceType.PersonalLicenceHolder,
                        request.AlcoholService)
                ]));
    }

    private static bool HasSupportedCoverage(
        ComplianceDeterminationRequest request) =>
        string.Equals(
            request.FoodRegistrationAuthority,
            SupportedAuthority,
            StringComparison.Ordinal)
        && (request.PrimaryTradingAuthority is null
            || string.Equals(
                request.PrimaryTradingAuthority,
                SupportedAuthority,
                StringComparison.Ordinal));

    private static bool OverlapsLateNightWindow(DailyOpeningHours day)
    {
        if (day.IsClosed)
        {
            return false;
        }

        if (day.IsOpenAllDay)
        {
            return true;
        }

        if (day.StartTime is not TimeOnly start
            || day.EndTime is not TimeOnly end)
        {
            return false;
        }

        if (start > end)
        {
            return true;
        }

        return start < LateNightEnd || end > LateNightStart;
    }
}
