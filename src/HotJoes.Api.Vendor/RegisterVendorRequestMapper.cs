using System.Globalization;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorRequestMapper
{
    public RegisterVendorCommand Map(RegisterVendorRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        RegisterVendorTradingCharacteristicsRequest trading =
            request.TradingCharacteristics
            ?? throw new ArgumentException(
                "Trading characteristics must be structurally valid before mapping.",
                nameof(request));
        RegisterVendorOpeningHoursRequest openingHours =
            trading.OpeningHours
            ?? throw new ArgumentException(
                "Opening hours must be structurally valid before mapping.",
                nameof(request));
        RegisterVendorPrimaryContactRequest contact =
            request.PrimaryContact
            ?? throw new ArgumentException(
                "Primary contact must be structurally valid before mapping.",
                nameof(request));
        RegisterVendorRegistrationDeclarationsRequest declarations =
            request.RegistrationDeclarations
            ?? throw new ArgumentException(
                "Registration declarations must be structurally valid before mapping.",
                nameof(request));

        return new RegisterVendorCommand(
            request.TradingName!,
            request.LegalOperatorName!,
            ParseEnum<LegalOperatorType>(request.LegalOperatorType),
            request.CompanyRegistrationNumber,
            ParseEnum<TradingLocation>(trading.TradingLocation),
            ParseTime(openingHours.StartTime),
            ParseTime(openingHours.EndTime),
            RequireBoolean(trading.ServiceIncludesHotFood),
            RequireBoolean(trading.AlcoholService),
            contact.ContactName!,
            contact.ContactEmail!,
            contact.ContactTelephone!,
            request.AddressResolutionReference!,
            request.Website,
            request.BusinessDescription,
            RequireBoolean(declarations.AuthorisedToRegisterBusiness),
            RequireBoolean(declarations.InformationAccurate),
            RequireBoolean(declarations.AcceptHotJoesPlatformTerms));
    }

    private static TEnum ParseEnum<TEnum>(string? value)
        where TEnum : struct, Enum
    {
        if (!Enum.TryParse(value, ignoreCase: true, out TEnum result))
        {
            throw new ArgumentException(
                $"{typeof(TEnum).Name} must be structurally valid before mapping.");
        }

        return result;
    }

    private static TimeOnly ParseTime(string? value)
    {
        if (!TimeOnly.TryParseExact(
                value,
                "HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out TimeOnly result))
        {
            throw new ArgumentException(
                "Opening hours must be structurally valid before mapping.");
        }

        return result;
    }

    private static bool RequireBoolean(bool? value)
    {
        return value
            ?? throw new ArgumentException(
                "Required booleans must be present before mapping.");
    }
}
