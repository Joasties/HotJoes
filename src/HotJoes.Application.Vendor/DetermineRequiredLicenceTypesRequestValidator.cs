using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class DetermineRequiredLicenceTypesRequestValidator
{
    public DetermineRequiredLicenceTypesRequestValidationResult Validate(
        DetermineRequiredLicenceTypesRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var errors = new List<RegistrationValidationError>();

        ValidateEnum(
            request.LegalOperatorType,
            "legalOperatorType",
            errors);
        ValidateEnum(
            request.TradingLocation,
            "tradingLocation",
            errors);
        ValidateWeeklyOpeningHours(request.WeeklyOpeningHours, errors);

        if (string.IsNullOrWhiteSpace(request.AddressResolutionReference))
        {
            errors.Add(new RegistrationValidationError(
                "addressResolutionReference",
                RegistrationValidationErrorCode.Required,
                "Address Resolution Reference is required."));
        }

        return errors.Count == 0
            ? DetermineRequiredLicenceTypesRequestValidationResult.Accept(request)
            : DetermineRequiredLicenceTypesRequestValidationResult.Reject(errors);
    }

    private static void ValidateEnum<TEnum>(
        TEnum value,
        string field,
        ICollection<RegistrationValidationError> errors)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            errors.Add(new RegistrationValidationError(
                field,
                RegistrationValidationErrorCode.InvalidValue,
                $"{field} must be a supported value."));
        }
    }

    private static void ValidateWeeklyOpeningHours(
        RegisterVendorWeeklyOpeningHours weeklyOpeningHours,
        ICollection<RegistrationValidationError> errors)
    {
        IReadOnlyList<RegisterVendorDailyOpeningHours> days =
            weeklyOpeningHours.Days;

        if (days.Count != 7 ||
            days.Any(day => !Enum.IsDefined(day.Day)) ||
            days.Select(day => day.Day).Distinct().Count() != 7 ||
            Enum.GetValues<TradingDay>().Any(expected =>
                days.All(day => day.Day != expected)) ||
            days.Any(day => !IsValid(day)))
        {
            errors.Add(new RegistrationValidationError(
                "weeklyOpeningHours.days",
                RegistrationValidationErrorCode.InvalidValue,
                "Weekly Opening Hours must contain exactly one valid entry for every day Monday through Sunday."));
        }
    }

    private static bool IsValid(RegisterVendorDailyOpeningHours day) =>
        day switch
        {
            {
                IsClosed: true,
                IsOpenAllDay: false,
                StartTime: null,
                EndTime: null
            } => true,
            {
                IsClosed: false,
                IsOpenAllDay: true,
                StartTime: null,
                EndTime: null
            } => true,
            {
                IsClosed: false,
                IsOpenAllDay: false,
                StartTime: not null,
                EndTime: not null
            } value => value.StartTime != value.EndTime,
            _ => false
        };
}
