using System.Text.Json;

namespace HotJoes.Api.Vendor;

public sealed class DetermineRequiredLicenceTypesRequestReader
{
    private readonly DetermineRequiredLicenceTypesRequestStructureValidator
        _structureValidator;

    public DetermineRequiredLicenceTypesRequestReader(
        DetermineRequiredLicenceTypesRequestStructureValidator structureValidator)
    {
        ArgumentNullException.ThrowIfNull(structureValidator);
        _structureValidator = structureValidator;
    }

    public async Task<DetermineRequiredLicenceTypesRequest?> ReadAsync(
        Stream requestBody,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(requestBody);

        try
        {
            using JsonDocument document = await JsonDocument.ParseAsync(
                requestBody,
                cancellationToken: cancellationToken);

            if (!_structureValidator.IsValid(document.RootElement))
            {
                return null;
            }

            return document.RootElement.Deserialize<
                DetermineRequiredLicenceTypesRequest>(
                    VendorApiJsonOptions.Create());
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
