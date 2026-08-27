using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor;

public static class VendorEndpointMappings
{
    public static IEndpointRouteBuilder MapVendorEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/vendors", RegisterVendorAsync)
            .Accepts<RegisterVendorRequest>("application/json")
            .Produces<RegisterVendorResponse>(StatusCodes.Status201Created)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        endpoints.MapGet("/vendors/{vendorId}", RetrieveVendorAsync)
            .Produces<RegisteredVendorDetailsResponse>(StatusCodes.Status200OK)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> RegisterVendorAsync(
        HttpContext context,
        IRegisterVendorService service,
        RegisterVendorRequestReader requestReader,
        RegisterVendorRequestMapper requestMapper,
        RegisterVendorResponseMapper responseMapper,
        VendorApiErrorMapper errorMapper,
        CancellationToken cancellationToken)
    {
        if (!context.Request.HasJsonContentType())
        {
            return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
        }

        RegisterVendorRequest? request = await requestReader.ReadAsync(
            context.Request.Body,
            cancellationToken);

        if (request is null)
        {
            return Error(errorMapper.MalformedRequest());
        }

        RegisterVendorResult result = await service.RegisterAsync(
            requestMapper.Map(request),
            cancellationToken);

        if (result is RegisterVendorResult.Success success)
        {
            RegisterVendorResponse response = responseMapper.Map(success);
            context.Response.Headers.Location = $"/vendors/{response.VendorId}";
            return Results.Json(
                response,
                VendorApiJsonOptions.Create(),
                statusCode: StatusCodes.Status201Created);
        }

        return Error(errorMapper.Map(result));
    }

    private static async Task<IResult> RetrieveVendorAsync(
        string vendorId,
        IRetrieveRegisteredVendorService service,
        RegisteredVendorDetailsResponseMapper responseMapper,
        VendorApiErrorMapper errorMapper,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParseExact(vendorId, "D", out Guid parsedVendorId)
            || parsedVendorId == Guid.Empty)
        {
            return Error(errorMapper.MalformedRequest());
        }

        RetrieveRegisteredVendorResult result = await service.RetrieveAsync(
            new VendorId(parsedVendorId),
            cancellationToken);

        if (result is RetrieveRegisteredVendorResult.Found found)
        {
            return Results.Json(
                responseMapper.Map(found.Details),
                VendorApiJsonOptions.Create(),
                statusCode: StatusCodes.Status200OK);
        }

        return Error(errorMapper.Map(result));
    }

    private static IResult Error(VendorApiErrorMapping mapping)
    {
        return Results.Json(
            mapping.Response,
            VendorApiJsonOptions.Create(),
            statusCode: mapping.StatusCode);
    }
}
