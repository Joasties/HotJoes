using System.Text.Json;
using System.Text.Json.Serialization;

namespace HotJoes.Api.Vendor;

public static class VendorApiJsonOptions
{
    public static JsonSerializerOptions Create()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
