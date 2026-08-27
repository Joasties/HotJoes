namespace HotJoes.Api.Vendor;

public sealed record RegisteredVendorPrimaryContactResponse(
    string ContactName,
    string ContactEmail,
    string ContactTelephone);
