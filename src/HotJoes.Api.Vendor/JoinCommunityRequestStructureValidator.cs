using System.Text.Json;

namespace HotJoes.Api.Vendor;

public sealed class JoinCommunityRequestStructureValidator
{
    private static readonly HashSet<string> ContactPreferences =
        new(StringComparer.Ordinal)
        {
            "email",
            "sms",
            "whatsApp"
        };

    public bool IsValid(JsonElement root)
    {
        return root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("vendorId", out JsonElement vendorId)
            && vendorId.ValueKind == JsonValueKind.String
            && Guid.TryParseExact(vendorId.GetString(), "D", out _)
            && root.TryGetProperty(
                "contactPreference",
                out JsonElement preference)
            && preference.ValueKind == JsonValueKind.String
            && ContactPreferences.Contains(preference.GetString()!);
    }
}
