using HotJoes.Application.Vendor;

namespace HotJoes.Api.Vendor;

public sealed class DetermineRequiredLicenceTypesResponseMapper
{
    public DetermineRequiredLicenceTypesResponse Map(
        ComplianceDetermination determination)
    {
        ArgumentNullException.ThrowIfNull(determination);

        return new DetermineRequiredLicenceTypesResponse(
            determination.RuleSetVersion,
            Array.AsReadOnly(
                determination.Items
                    .Select(item => new RequiredLicenceTypeResponse(
                        ToLowerCamelCase(item.RequiredLicenceType.ToString()),
                        item.IsRequired))
                    .ToArray()));
    }

    private static string ToLowerCamelCase(string value) =>
        char.ToLowerInvariant(value[0]) + value[1..];
}
