using HotJoes.Domain.Vendor;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RegisteredVendorDetailsMapper
{
    public RegisteredVendorDetails Map(VendorAggregate vendor)
    {
        ArgumentNullException.ThrowIfNull(vendor);

        VendorRegistrationInformation information =
            vendor.RegisteredInformation;
        TradingCharacteristics characteristics =
            information.TradingCharacteristics;
        BusinessAddressSnapshot address =
            information.BusinessAddressSnapshot;

        return new RegisteredVendorDetails(
            vendor.Id,
            vendor.RegisteredAt,
            vendor.State,
            vendor.TradingPreference,
            information.LegalOperatorType,
            information.LegalOperatorName.Value,
            information.CompanyRegistrationNumber?.Value,
            information.TradingName.Value,
            new RegisteredVendorTradingCharacteristics(
                characteristics.TradingLocation,
                new RegisteredVendorOpeningHours(
                    characteristics.OpeningHours.StartTime,
                    characteristics.OpeningHours.EndTime),
                characteristics.ServiceIncludesHotFood,
                characteristics.AlcoholService),
            information.PrimaryContact.ContactName,
            information.PrimaryContact.EmailAddress.Value,
            information.PrimaryContact.TelephoneNumber.Value,
            information.CanonicalAddressId.Value,
            new RegisteredVendorBusinessAddress(
                address.AddressLine1,
                address.AddressLine2,
                address.AddressLine3,
                address.PostTown,
                address.Postcode,
                address.County,
                address.RecipientOrOrganisationName),
            information.FoodRegistrationAuthority.Value,
            information.PrimaryTradingAuthority?.Value,
            vendor.Website?.AbsoluteUri,
            vendor.BusinessDescription);
    }
}
