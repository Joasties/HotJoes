using System.Globalization;
using HotJoes.Application.Vendor;

namespace HotJoes.Api.Vendor;

public sealed class RegisteredVendorDetailsResponseMapper
{
    public RegisteredVendorDetailsResponse Map(RegisteredVendorDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);

        return new RegisteredVendorDetailsResponse(
            details.VendorId.Value.ToString("D").ToLowerInvariant(),
            details.RegisteredAt.UtcDateTime.ToString("O", CultureInfo.InvariantCulture),
            ToLowerCamelCase(details.VendorState.ToString()),
            ToLowerCamelCase(details.TradingPreference.ToString()),
            details.TradingName,
            ToLowerCamelCase(details.LegalOperatorType.ToString()),
            details.LegalOperatorName,
            details.CompanyRegistrationNumber,
            Map(details.TradingCharacteristics),
            new RegisteredVendorPrimaryContactResponse(
                details.ContactName,
                details.ContactEmail,
                details.ContactTelephone),
            details.CanonicalAddressId,
            Map(details.BusinessAddress),
            details.FoodRegistrationAuthority,
            details.PrimaryTradingAuthority,
            details.Website,
            details.BusinessDescription);
    }

    private static RegisteredVendorTradingCharacteristicsResponse Map(
        RegisteredVendorTradingCharacteristics trading)
    {
        return new RegisteredVendorTradingCharacteristicsResponse(
            ToLowerCamelCase(trading.TradingLocation.ToString()),
            new RegisteredVendorOpeningHoursResponse(
                trading.OpeningHours.StartTime.ToString(
                    "HH:mm:ss",
                    CultureInfo.InvariantCulture),
                trading.OpeningHours.EndTime.ToString(
                    "HH:mm:ss",
                    CultureInfo.InvariantCulture)),
            trading.ServiceIncludesHotFood,
            trading.AlcoholService);
    }

    private static RegisteredVendorBusinessAddressResponse Map(
        RegisteredVendorBusinessAddress address)
    {
        return new RegisteredVendorBusinessAddressResponse(
            address.AddressLine1,
            address.AddressLine2,
            address.AddressLine3,
            address.PostTown,
            address.Postcode,
            address.County,
            address.RecipientOrOrganisationName);
    }

    private static string ToLowerCamelCase(string value)
    {
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
