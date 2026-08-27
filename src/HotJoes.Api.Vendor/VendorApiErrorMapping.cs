namespace HotJoes.Api.Vendor;

public sealed record VendorApiErrorMapping(
    int StatusCode,
    VendorApiErrorResponse Response);
