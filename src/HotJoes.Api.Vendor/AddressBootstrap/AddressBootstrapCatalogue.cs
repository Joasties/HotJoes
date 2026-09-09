using System.Text.Json;
using System.Text.Json.Serialization;
using HotJoes.Application.Address;

namespace HotJoes.Api.Vendor.AddressBootstrap;

public sealed class AddressBootstrapCatalogue
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    private readonly IReadOnlyList<AddressBootstrapCatalogueEntry> _entries;

    private AddressBootstrapCatalogue(
        string revision,
        IReadOnlyList<AddressBootstrapCatalogueEntry> entries)
    {
        Revision = revision;
        _entries = entries;
    }

    public string Revision { get; }

    public static AddressBootstrapCatalogue Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using FileStream stream = File.OpenRead(path);
        CatalogueDocument? document = JsonSerializer.Deserialize<CatalogueDocument>(
            stream,
            SerializerOptions);

        if (document is null || string.IsNullOrWhiteSpace(document.Revision))
        {
            throw new InvalidOperationException(
                "The Address bootstrap catalogue revision is required.");
        }

        if (document.Entries is not { Count: > 0 })
        {
            throw new InvalidOperationException(
                "The Address bootstrap catalogue must contain at least one entry.");
        }

        var references = new HashSet<string>(StringComparer.Ordinal);
        var entries = new List<AddressBootstrapCatalogueEntry>(
            document.Entries.Count);

        foreach (CatalogueEntryDocument source in document.Entries)
        {
            AddressBootstrapCatalogueEntry entry = CreateEntry(source);

            if (!references.Add(entry.Reference))
            {
                throw new InvalidOperationException(
                    $"Duplicate Address bootstrap reference '{entry.Reference}'.");
            }

            entries.Add(entry);
        }

        return new AddressBootstrapCatalogue(
            document.Revision,
            entries.AsReadOnly());
    }

    public StubAddressApplication CreateAddressApplication()
    {
        var application = new StubAddressApplication();

        foreach (AddressBootstrapCatalogueEntry entry in _entries)
        {
            application.AddKnownResult(
                entry.Reference,
                entry.TradingLocation,
                entry.Result);
        }

        return application;
    }

    private static AddressBootstrapCatalogueEntry CreateEntry(
        CatalogueEntryDocument source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source.Reference);

        TradingLocation tradingLocation = source.TradingLocation switch
        {
            "restaurant" => TradingLocation.Restaurant,
            "stall" => TradingLocation.Stall,
            "kitchen" => TradingLocation.Kitchen,
            _ => throw new InvalidOperationException(
                $"Unsupported Trading Location '{source.TradingLocation}'.")
        };

        if (tradingLocation == TradingLocation.Stall
            && string.IsNullOrWhiteSpace(source.PrimaryTradingAuthority))
        {
            throw new InvalidOperationException(
                "A Stall catalogue entry requires a Primary Trading Authority.");
        }

        try
        {
            string canonicalAddressId = Require(
                source.CanonicalAddressId,
                nameof(source.CanonicalAddressId));
            string addressLine2 = Require(
                source.AddressLine2,
                nameof(source.AddressLine2));
            string postTown = Require(
                source.PostTown,
                nameof(source.PostTown));
            string postcode = Require(
                source.Postcode,
                nameof(source.Postcode));
            string foodRegistrationAuthority = Require(
                source.FoodRegistrationAuthority,
                nameof(source.FoodRegistrationAuthority));
            var result = new CompleteAddressResult(
                canonicalAddressId,
                source.AddressLine1,
                addressLine2,
                source.AddressLine3,
                source.AddressLine4,
                postTown,
                postcode,
                source.County,
                foodRegistrationAuthority,
                source.PrimaryTradingAuthority);

            return new AddressBootstrapCatalogueEntry(
                source.Reference,
                tradingLocation,
                result);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(
                $"Address bootstrap entry '{source.Reference}' is incomplete.",
                exception);
        }
    }

    private static string Require(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"Catalogue field '{field}' is required.",
                field);
        }

        return value;
    }

    private sealed record CatalogueDocument(
        string? Revision,
        IReadOnlyList<CatalogueEntryDocument>? Entries);

    private sealed record CatalogueEntryDocument(
        string? Reference,
        string? TradingLocation,
        string? CanonicalAddressId,
        string? AddressLine1,
        string? AddressLine2,
        string? AddressLine3,
        string? AddressLine4,
        string? PostTown,
        string? Postcode,
        string? County,
        string? FoodRegistrationAuthority,
        string? PrimaryTradingAuthority);
}
