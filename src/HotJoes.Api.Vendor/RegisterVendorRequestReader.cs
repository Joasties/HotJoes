using System.Text.Json;

namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorRequestReader
{
    private readonly RegisterVendorRequestStructureValidator _structureValidator;

    public RegisterVendorRequestReader(
        RegisterVendorRequestStructureValidator structureValidator)
    {
        ArgumentNullException.ThrowIfNull(structureValidator);
        _structureValidator = structureValidator;
    }

    public async Task<RegisterVendorRequest?> ReadAsync(
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

            return document.RootElement.Deserialize<RegisterVendorRequest>(
                VendorApiJsonOptions.Create());
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
