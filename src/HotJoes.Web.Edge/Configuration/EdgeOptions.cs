namespace HotJoes.Web.Edge.Configuration;

public sealed class EdgeOptions
{
    public const string SectionName = "Edge";

    public string? VendorApiBaseAddress { get; init; }
}
