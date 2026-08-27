namespace HotJoes.Api.Vendor;

public sealed record VendorApiErrorResponse(
    string Code,
    string Message,
    IReadOnlyList<VendorApiValidationErrorResponse>? ValidationErrors);
