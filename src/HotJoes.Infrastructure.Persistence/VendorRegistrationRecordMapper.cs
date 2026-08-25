using HotJoes.Domain.Vendor;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.Infrastructure.Persistence;

internal static class VendorRegistrationRecordMapper
{
    public static VendorRegistrationRecord ToRecord(VendorAggregate vendor)
    {
        ArgumentNullException.ThrowIfNull(vendor);

        VendorRegistrationInformation information =
            vendor.RegisteredInformation;
        BusinessAddressSnapshot address = information.BusinessAddressSnapshot;
        TradingCharacteristics characteristics = information.TradingCharacteristics;

        return new VendorRegistrationRecord
        {
            VendorId = vendor.Id.Value,
            VendorState = ToPersistenceValue(vendor.State),
            TradingPreference = ToPersistenceValue(vendor.TradingPreference),
            RegisteredAtUtc = vendor.RegisteredAt,
            LegalOperatorType = ToPersistenceValue(information.LegalOperatorType),
            LegalOperatorName = information.LegalOperatorName.Value,
            NormalizedLegalOperatorName = NormalizeIdentityName(
                information.LegalOperatorName.Value),
            TradingName = information.TradingName.Value,
            NormalizedTradingName = NormalizeIdentityName(
                information.TradingName.Value),
            CompanyRegistrationNumber = information.CompanyRegistrationNumber?.Value,
            ContactName = information.PrimaryContact.ContactName,
            ContactEmail = information.PrimaryContact.EmailAddress.Value,
            ContactTelephone = information.PrimaryContact.TelephoneNumber.Value,
            CanonicalAddressId = information.CanonicalAddressId.Value,
            RecipientOrOrganisationName = address.RecipientOrOrganisationName,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            AddressLine3 = address.AddressLine3,
            PostTown = address.PostTown,
            Postcode = address.Postcode,
            County = address.County,
            FoodRegistrationAuthority = information.FoodRegistrationAuthority.Value,
            PrimaryTradingAuthority = information.PrimaryTradingAuthority?.Value,
            TradingLocation = ToPersistenceValue(characteristics.TradingLocation),
            OpeningHoursStart = characteristics.OpeningHours.StartTime,
            OpeningHoursEnd = characteristics.OpeningHours.EndTime,
            ServiceIncludesHotFood = characteristics.ServiceIncludesHotFood,
            AlcoholService = characteristics.AlcoholService,
            Website = vendor.Website?.AbsoluteUri,
            BusinessDescription = vendor.BusinessDescription
        };
    }

    public static VendorAggregate ToDomain(VendorRegistrationRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        var registrationInformation = new VendorRegistrationInformation(
            ToLegalOperatorType(record.LegalOperatorType),
            new VendorName(record.LegalOperatorName),
            new VendorName(record.TradingName),
            record.CompanyRegistrationNumber is null
                ? null
                : new CompanyRegistrationNumber(record.CompanyRegistrationNumber),
            new PrimaryContact(
                record.ContactName,
                new EmailAddress(record.ContactEmail),
                new TelephoneNumber(record.ContactTelephone)),
            new CanonicalAddressId(record.CanonicalAddressId),
            new BusinessAddressSnapshot(
                record.AddressLine1,
                record.AddressLine2,
                record.AddressLine3,
                record.PostTown,
                record.Postcode,
                record.County,
                record.RecipientOrOrganisationName),
            new FoodRegistrationAuthority(record.FoodRegistrationAuthority),
            record.PrimaryTradingAuthority is null
                ? null
                : new PrimaryTradingAuthority(record.PrimaryTradingAuthority),
            new TradingCharacteristics(
                ToTradingLocation(record.TradingLocation),
                new OpeningHours(
                    record.OpeningHoursStart,
                    record.OpeningHoursEnd),
                record.ServiceIncludesHotFood,
                record.AlcoholService));

        return VendorAggregate.Rehydrate(
            new VendorId(record.VendorId),
            registrationInformation,
            record.Website is null
                ? null
                : new Uri(record.Website, UriKind.Absolute),
            record.BusinessDescription,
            record.RegisteredAtUtc,
            ToVendorState(record.VendorState),
            ToTradingPreference(record.TradingPreference));
    }

    private static string NormalizeIdentityName(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static string ToPersistenceValue(VendorState value)
    {
        return value switch
        {
            VendorState.PendingActivation => "pendingActivation",
            VendorState.Activated => "activated",
            VendorState.Suspended => "suspended",
            VendorState.Deactivated => "deactivated",
            _ => throw UnsupportedValue(value)
        };
    }

    private static string ToPersistenceValue(TradingPreference value)
    {
        return value switch
        {
            TradingPreference.Offline => "offline",
            TradingPreference.Online => "online",
            _ => throw UnsupportedValue(value)
        };
    }

    private static string ToPersistenceValue(LegalOperatorType value)
    {
        return value switch
        {
            LegalOperatorType.SoleTrader => "soleTrader",
            LegalOperatorType.GeneralPartnership => "generalPartnership",
            LegalOperatorType.LimitedCompany => "limitedCompany",
            LegalOperatorType.LimitedLiabilityPartnership =>
                "limitedLiabilityPartnership",
            LegalOperatorType.CharitableCommunityGroup =>
                "charitableCommunityGroup",
            LegalOperatorType.CharitableIncorporatedOrganisation =>
                "charitableIncorporatedOrganisation",
            _ => throw UnsupportedValue(value)
        };
    }

    private static string ToPersistenceValue(TradingLocation value)
    {
        return value switch
        {
            TradingLocation.Restaurant => "restaurant",
            TradingLocation.Stall => "stall",
            TradingLocation.Kitchen => "kitchen",
            _ => throw UnsupportedValue(value)
        };
    }

    private static VendorState ToVendorState(string value)
    {
        return value switch
        {
            "pendingActivation" => VendorState.PendingActivation,
            "activated" => VendorState.Activated,
            "suspended" => VendorState.Suspended,
            "deactivated" => VendorState.Deactivated,
            _ => throw UnsupportedValue(nameof(VendorRegistrationRecord.VendorState), value)
        };
    }

    private static TradingPreference ToTradingPreference(string value)
    {
        return value switch
        {
            "offline" => TradingPreference.Offline,
            "online" => TradingPreference.Online,
            _ => throw UnsupportedValue(
                nameof(VendorRegistrationRecord.TradingPreference),
                value)
        };
    }

    private static LegalOperatorType ToLegalOperatorType(string value)
    {
        return value switch
        {
            "soleTrader" => LegalOperatorType.SoleTrader,
            "generalPartnership" => LegalOperatorType.GeneralPartnership,
            "limitedCompany" => LegalOperatorType.LimitedCompany,
            "limitedLiabilityPartnership" =>
                LegalOperatorType.LimitedLiabilityPartnership,
            "charitableCommunityGroup" =>
                LegalOperatorType.CharitableCommunityGroup,
            "charitableIncorporatedOrganisation" =>
                LegalOperatorType.CharitableIncorporatedOrganisation,
            _ => throw UnsupportedValue(
                nameof(VendorRegistrationRecord.LegalOperatorType),
                value)
        };
    }

    private static TradingLocation ToTradingLocation(string value)
    {
        return value switch
        {
            "restaurant" => TradingLocation.Restaurant,
            "stall" => TradingLocation.Stall,
            "kitchen" => TradingLocation.Kitchen,
            _ => throw UnsupportedValue(
                nameof(VendorRegistrationRecord.TradingLocation),
                value)
        };
    }

    private static InvalidOperationException UnsupportedValue<T>(T value)
        where T : struct, Enum
    {
        return new InvalidOperationException(
            $"Unsupported persisted {typeof(T).Name} value '{value}'.");
    }

    private static InvalidOperationException UnsupportedValue(
        string propertyName,
        string value)
    {
        return new InvalidOperationException(
            $"Unsupported persisted {propertyName} value '{value}'.");
    }
}
