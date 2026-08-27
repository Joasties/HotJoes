namespace HotJoes.Api.Vendor;

public sealed record RegisteredVendorDetailsResponse(
    string VendorId,
    string RegisteredAt,
    string VendorState,
    string TradingPreference,
    string TradingName,
    string LegalOperatorType,
    string LegalOperatorName,
    string? CompanyRegistrationNumber,
    RegisteredVendorTradingCharacteristicsResponse TradingCharacteristics,
    RegisteredVendorPrimaryContactResponse PrimaryContact,
    string CanonicalAddressId,
    RegisteredVendorBusinessAddressResponse BusinessAddressSnapshot,
    string FoodRegistrationAuthority,
    string? PrimaryTradingAuthority,
    string? Website,
    string? BusinessDescription);
