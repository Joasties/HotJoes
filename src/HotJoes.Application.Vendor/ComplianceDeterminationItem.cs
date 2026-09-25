namespace HotJoes.Application.Vendor;

public sealed record ComplianceDeterminationItem(
    RequiredLicenceType RequiredLicenceType,
    bool IsRequired);
