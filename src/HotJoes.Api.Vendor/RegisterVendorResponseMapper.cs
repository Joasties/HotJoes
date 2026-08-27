using HotJoes.Application.Vendor;

namespace HotJoes.Api.Vendor;

public sealed class RegisterVendorResponseMapper
{
    public RegisterVendorResponse Map(RegisterVendorResult.Success result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new RegisterVendorResponse(
            result.VendorId.Value.ToString("D").ToLowerInvariant(),
            ToLowerCamelCase(result.VendorState.ToString()));
    }

    private static string ToLowerCamelCase(string value)
    {
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
