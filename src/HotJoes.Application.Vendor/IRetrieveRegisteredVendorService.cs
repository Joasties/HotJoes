using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public interface IRetrieveRegisteredVendorService
{
    Task<RetrieveRegisteredVendorResult> RetrieveAsync(
        VendorId vendorId,
        CancellationToken cancellationToken);
}
