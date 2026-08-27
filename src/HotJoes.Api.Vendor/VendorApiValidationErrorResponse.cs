namespace HotJoes.Api.Vendor;

public sealed record VendorApiValidationErrorResponse(
    string Field,
    string Code,
    string Message);
