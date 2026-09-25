namespace HotJoes.Application.Compliance;

public sealed record ComplianceDeterminationItem(
    RequiredLicenceType RequiredLicenceType,
    bool IsRequired);
