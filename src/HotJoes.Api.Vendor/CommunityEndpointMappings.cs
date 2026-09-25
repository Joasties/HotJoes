using HotJoes.Application.Community;

namespace HotJoes.Api.Vendor;

public static class CommunityEndpointMappings
{
    public static IEndpointRouteBuilder MapCommunityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/community-participations", JoinCommunityAsync)
            .Accepts<JoinCommunityRequest>("application/json")
            .Produces<JoinCommunityResponse>(StatusCodes.Status201Created)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<VendorApiErrorResponse>(StatusCodes.Status503ServiceUnavailable);

        return endpoints;
    }

    private static async Task<IResult> JoinCommunityAsync(
        HttpContext context,
        IJoinCommunityService service,
        JoinCommunityRequestReader requestReader,
        JoinCommunityRequestMapper requestMapper,
        JoinCommunityResponseMapper responseMapper,
        CommunityApiErrorMapper errorMapper,
        VendorApiErrorMapper vendorErrorMapper,
        CancellationToken cancellationToken)
    {
        if (!context.Request.HasJsonContentType())
        {
            return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
        }

        JoinCommunityRequest? request = await requestReader.ReadAsync(
            context.Request.Body,
            cancellationToken);

        if (request is null)
        {
            return Error(vendorErrorMapper.MalformedRequest());
        }

        JoinCommunityResult result = await service.JoinAsync(
            requestMapper.Map(request),
            cancellationToken);

        CommunityParticipation? participation = result switch
        {
            JoinCommunityResult.CommunityParticipationRecorded recorded =>
                recorded.Participation,
            JoinCommunityResult.CommunityParticipationAlreadyRecorded replay =>
                replay.Participation,
            _ => null
        };

        if (participation is not null)
        {
            JoinCommunityResponse response = responseMapper.Map(participation);
            context.Response.Headers.Location =
                $"/community-participations/{response.CommunityParticipationId:D}";
            return Results.Json(
                response,
                VendorApiJsonOptions.Create(),
                statusCode: StatusCodes.Status201Created);
        }

        return Error(errorMapper.Map(result));
    }

    private static IResult Error(VendorApiErrorMapping mapping) =>
        Results.Json(
            mapping.Response,
            VendorApiJsonOptions.Create(),
            statusCode: mapping.StatusCode);
}
