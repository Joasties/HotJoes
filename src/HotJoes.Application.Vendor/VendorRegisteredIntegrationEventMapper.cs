using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class VendorRegisteredIntegrationEventMapper
{
    public VendorRegisteredIntegrationEvent Map(
        VendorRegistered completedFact,
        HotJoes.Domain.Vendor.Vendor vendor,
        Guid eventId,
        DateTimeOffset occurredAt)
    {
        ArgumentNullException.ThrowIfNull(completedFact);
        ArgumentNullException.ThrowIfNull(vendor);

        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("EventId cannot be empty.", nameof(eventId));
        }

        VendorRegistrationInformation information = vendor.RegisteredInformation;
        TradingCharacteristics trading = information.TradingCharacteristics;
        BusinessAddressSnapshot address = information.BusinessAddressSnapshot;

        return new VendorRegisteredIntegrationEvent(
            eventId,
            "VendorRegistered",
            1,
            occurredAt,
            new VendorRegisteredIntegrationEventPayload(
                vendor.Id.Value,
                vendor.RegisteredAt,
                MapVendorState(vendor.State),
                MapTradingPreference(vendor.TradingPreference),
                MapLegalOperatorType(information.LegalOperatorType),
                new VendorRegisteredTradingCharacteristics(
                    MapTradingLocation(trading.TradingLocation),
                    new VendorRegisteredOpeningHours(
                        trading.OpeningHours.StartTime,
                        trading.OpeningHours.EndTime),
                    trading.ServiceIncludesHotFood,
                    trading.AlcoholService),
                new VendorRegisteredBusinessAddress(
                    information.CanonicalAddressId.Value,
                    address.RecipientOrOrganisationName,
                    address.AddressLine1,
                    address.AddressLine2,
                    address.AddressLine3,
                    address.PostTown,
                    address.Postcode,
                    address.County),
                information.FoodRegistrationAuthority.Value,
                information.PrimaryTradingAuthority?.Value));
    }

    private static string MapVendorState(VendorState state)
    {
        return state switch
        {
            VendorState.PendingActivation => "pendingActivation",
            VendorState.Activated => "activated",
            VendorState.Suspended => "suspended",
            VendorState.Deactivated => "deactivated",
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };
    }

    private static string MapTradingPreference(TradingPreference preference)
    {
        return preference switch
        {
            TradingPreference.Offline => "offline",
            TradingPreference.Online => "online",
            _ => throw new ArgumentOutOfRangeException(nameof(preference))
        };
    }

    private static string MapLegalOperatorType(LegalOperatorType legalOperatorType)
    {
        return legalOperatorType switch
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
            _ => throw new ArgumentOutOfRangeException(nameof(legalOperatorType))
        };
    }

    private static string MapTradingLocation(TradingLocation tradingLocation)
    {
        return tradingLocation switch
        {
            TradingLocation.Restaurant => "restaurant",
            TradingLocation.Stall => "stall",
            TradingLocation.Kitchen => "kitchen",
            _ => throw new ArgumentOutOfRangeException(nameof(tradingLocation))
        };
    }
}
