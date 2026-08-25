namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredIntegrationEventPayload(
    Guid VendorId,
    DateTimeOffset RegisteredAt,
    string VendorState,
    string TradingPreference,
    string LegalOperatorType,
    VendorRegisteredTradingCharacteristics TradingCharacteristics,
    VendorRegisteredBusinessAddress BusinessAddress,
    string FoodRegistrationAuthority,
    string? PrimaryTradingAuthority);
