namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorRequest
{
    public string? TradingName { get; init; }

    public string? LegalOperatorName { get; init; }

    public string? LegalOperatorType { get; init; }

    public string? CompanyRegistrationNumber { get; init; }

    public RegisterVendorTradingCharacteristicsRequest? TradingCharacteristics { get; init; }

    public RegisterVendorPrimaryContactRequest? PrimaryContact { get; init; }

    public string? AddressResolutionReference { get; init; }

    public string? Website { get; init; }

    public string? BusinessDescription { get; init; }

    public RegisterVendorRegistrationDeclarationsRequest? RegistrationDeclarations { get; init; }
}
