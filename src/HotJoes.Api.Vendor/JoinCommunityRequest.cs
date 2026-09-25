namespace HotJoes.Api.Vendor;

public sealed class JoinCommunityRequest
{
    public Guid? VendorId { get; init; }

    public string? ContactPreference { get; init; }
}
