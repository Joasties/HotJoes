namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorPrimaryContactRequest
{
    public string? ContactName { get; init; }

    public string? ContactEmail { get; init; }

    public string? ContactTelephone { get; init; }
}
