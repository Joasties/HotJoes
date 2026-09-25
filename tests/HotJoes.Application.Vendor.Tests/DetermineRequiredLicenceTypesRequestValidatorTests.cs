using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class DetermineRequiredLicenceTypesRequestValidatorTests
{
    [Fact]
    public void VR_DETERMINATION_002_MultipleIndependentErrors_AreReturnedTogether()
    {
        var request = new DetermineRequiredLicenceTypesRequest(
            (LegalOperatorType)int.MaxValue,
            (TradingLocation)int.MaxValue,
            new RegisterVendorWeeklyOpeningHours(
            [
                new RegisterVendorDailyOpeningHours(
                    TradingDay.Monday,
                    IsClosed: true,
                    IsOpenAllDay: true,
                    StartTime: new TimeOnly(9, 0),
                    EndTime: null)
            ]),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "");
        var validator = new DetermineRequiredLicenceTypesRequestValidator();

        DetermineRequiredLicenceTypesRequestValidationResult result =
            validator.Validate(request);

        var invalid = Assert.IsType<
            DetermineRequiredLicenceTypesRequestValidationResult.Invalid>(
                result);
        Assert.Contains(invalid.Errors, error =>
            error.Field == "legalOperatorType");
        Assert.Contains(invalid.Errors, error =>
            error.Field == "tradingLocation");
        Assert.Contains(invalid.Errors, error =>
            error.Field == "weeklyOpeningHours.days");
        Assert.Contains(invalid.Errors, error =>
            error.Field == "addressResolutionReference");
        Assert.Equal(4, invalid.Errors.Count);
    }

    [Fact]
    public void VR_DETERMINATION_002_ValidRequest_IsAcceptedWithoutChangingValues()
    {
        DetermineRequiredLicenceTypesRequest request = CreateValidRequest();
        var validator = new DetermineRequiredLicenceTypesRequestValidator();

        DetermineRequiredLicenceTypesRequestValidationResult result =
            validator.Validate(request);

        var accepted = Assert.IsType<
            DetermineRequiredLicenceTypesRequestValidationResult.Accepted>(
                result);
        Assert.Same(request, accepted.Request);
    }

    private static DetermineRequiredLicenceTypesRequest CreateValidRequest() =>
        new(
            LegalOperatorType.LimitedCompany,
            TradingLocation.Stall,
            RegisterVendorWeeklyOpeningHours.EveryDay(
                new TimeOnly(9, 0),
                new TimeOnly(17, 0)),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "address-resolution-reference-001");
}
