using HotJoes.Application.Address;

namespace HotJoes.Api.Vendor;

public static class AddressSearchEndpointMappings
{
    public static IEndpointRouteBuilder MapAddressSearchEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/address-search", Search)
            .Produces<AddressSearchResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        return endpoints;
    }

    private static IResult Search(
        string? query,
        string? tradingLocation,
        IAddressSearchService service)
    {
        if (string.IsNullOrWhiteSpace(query)
            || query.Trim().Length > 200
            || !TryParseTradingLocation(tradingLocation, out TradingLocation parsed))
        {
            return Results.BadRequest();
        }

        AddressSearchResultResponse[] results = service
            .Search(query, parsed)
            .Select(candidate => new AddressSearchResultResponse(
                candidate.AddressResolutionReference,
                candidate.DisplayLines))
            .ToArray();

        return Results.Json(
            new AddressSearchResponse(results),
            VendorApiJsonOptions.Create());
    }

    private static bool TryParseTradingLocation(
        string? value,
        out TradingLocation tradingLocation)
    {
        tradingLocation = value switch
        {
            "restaurant" => TradingLocation.Restaurant,
            "stall" => TradingLocation.Stall,
            "kitchen" => TradingLocation.Kitchen,
            _ => default
        };
        return value is "restaurant" or "stall" or "kitchen";
    }
}
