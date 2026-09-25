using System.Text.Json;

namespace HotJoes.Api.Vendor;

public sealed class JoinCommunityRequestReader
{
    private readonly JoinCommunityRequestStructureValidator _validator;

    public JoinCommunityRequestReader(
        JoinCommunityRequestStructureValidator validator)
    {
        ArgumentNullException.ThrowIfNull(validator);
        _validator = validator;
    }

    public async Task<JoinCommunityRequest?> ReadAsync(
        Stream requestBody,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(requestBody);

        try
        {
            using JsonDocument document = await JsonDocument.ParseAsync(
                requestBody,
                cancellationToken: cancellationToken);

            if (!_validator.IsValid(document.RootElement))
            {
                return null;
            }

            return document.RootElement.Deserialize<JoinCommunityRequest>(
                VendorApiJsonOptions.Create());
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
