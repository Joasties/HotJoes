namespace HotJoes.Infrastructure.Persistence;

internal sealed class VendorRegistrationRecord
{
    public Guid VendorId { get; set; }

    public string VendorState { get; set; } = null!;

    public string TradingPreference { get; set; } = null!;

    public DateTimeOffset RegisteredAtUtc { get; set; }

    public string LegalOperatorType { get; set; } = null!;

    public string LegalOperatorName { get; set; } = null!;

    public string NormalizedLegalOperatorName { get; set; } = null!;

    public string TradingName { get; set; } = null!;

    public string NormalizedTradingName { get; set; } = null!;

    public string? CompanyRegistrationNumber { get; set; }

    public string ContactName { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public string ContactTelephone { get; set; } = null!;

    public string CanonicalAddressId { get; set; } = null!;

    public string? RecipientOrOrganisationName { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string? AddressLine3 { get; set; }

    public string PostTown { get; set; } = null!;

    public string Postcode { get; set; } = null!;

    public string? County { get; set; }

    public string FoodRegistrationAuthority { get; set; } = null!;

    public string? PrimaryTradingAuthority { get; set; }

    public string TradingLocation { get; set; } = null!;

    public TimeOnly OpeningHoursStart { get; set; }

    public TimeOnly OpeningHoursEnd { get; set; }

    public bool ServiceIncludesHotFood { get; set; }

    public bool AlcoholService { get; set; }

    public string? Website { get; set; }

    public string? BusinessDescription { get; set; }
}
