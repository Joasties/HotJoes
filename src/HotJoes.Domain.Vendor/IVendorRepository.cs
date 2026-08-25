namespace HotJoes.Domain.Vendor;

public interface IVendorRepository
{
    Task<Vendor?> FindAsync(
        VendorId vendorId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Vendor vendor,
        CancellationToken cancellationToken);
}
