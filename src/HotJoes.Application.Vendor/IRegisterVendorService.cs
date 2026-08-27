namespace HotJoes.Application.Vendor;

public interface IRegisterVendorService
{
    Task<RegisterVendorResult> RegisterAsync(
        RegisterVendorCommand command,
        CancellationToken cancellationToken = default);
}
