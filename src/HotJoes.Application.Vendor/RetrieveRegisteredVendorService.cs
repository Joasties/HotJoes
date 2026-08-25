using HotJoes.Domain.Vendor;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RetrieveRegisteredVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly RegisteredVendorDetailsMapper _detailsMapper;

    public RetrieveRegisteredVendorService(
        IVendorRepository vendorRepository,
        RegisteredVendorDetailsMapper detailsMapper)
    {
        ArgumentNullException.ThrowIfNull(vendorRepository);
        ArgumentNullException.ThrowIfNull(detailsMapper);

        _vendorRepository = vendorRepository;
        _detailsMapper = detailsMapper;
    }

    public async Task<RetrieveRegisteredVendorResult> RetrieveAsync(
        VendorId vendorId,
        CancellationToken cancellationToken)
    {
        VendorAggregate? vendor = await _vendorRepository.FindAsync(
            vendorId,
            cancellationToken);

        if (vendor is null)
        {
            return RetrieveRegisteredVendorResult.VendorNotFound();
        }

        RegisteredVendorDetails details = _detailsMapper.Map(vendor);
        return RetrieveRegisteredVendorResult.VendorFound(details);
    }
}
