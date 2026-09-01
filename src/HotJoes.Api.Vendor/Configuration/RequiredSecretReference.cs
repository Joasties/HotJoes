namespace HotJoes.Api.Vendor.Configuration;

public sealed record RequiredSecretReference(
    string Purpose,
    string Name,
    string Version);
