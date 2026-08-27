using Microsoft.AspNetCore.Diagnostics;

namespace HotJoes.Api.Vendor;

public sealed class VendorApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<VendorApiExceptionHandler> _logger;

    public VendorApiExceptionHandler(
        ILogger<VendorApiExceptionHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "An unexpected failure occurred while processing a Vendor API request.");

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            new VendorApiErrorResponse(
                "unexpectedFailure",
                "The request could not be completed because an unexpected failure occurred.",
                ValidationErrors: null),
            VendorApiJsonOptions.Create(),
            cancellationToken);

        return true;
    }
}
