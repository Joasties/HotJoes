namespace HotJoes.Application.Vendor;

public interface IRegisterVendorCommandValidator
{
    RegisterVendorCommandValidationResult Validate(RegisterVendorCommand command);
}
