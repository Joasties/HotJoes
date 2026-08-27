using HotJoes.Application.Vendor;

namespace HotJoes.Application.Vendor.Tests;

internal sealed class AcceptingRegisterVendorCommandValidator
    : IRegisterVendorCommandValidator
{
    public RegisterVendorCommandValidationResult Validate(
        RegisterVendorCommand command)
    {
        return RegisterVendorCommandValidationResult.Accepted(command);
    }
}
