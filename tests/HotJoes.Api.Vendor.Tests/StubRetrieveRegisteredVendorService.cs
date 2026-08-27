using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class StubRetrieveRegisteredVendorService
    : IRetrieveRegisteredVendorService
{
    public RetrieveRegisteredVendorResult NextResult { get; set; } =
        RetrieveRegisteredVendorResult.VendorNotFound();

    public Exception? NextException { get; set; }

    public int InvocationCount { get; private set; }

    public VendorId? LastVendorId { get; private set; }

    public bool LastCancellationTokenCanBeCanceled { get; private set; }

    public Task<RetrieveRegisteredVendorResult> RetrieveAsync(
        VendorId vendorId,
        CancellationToken cancellationToken)
    {
        InvocationCount++;
        LastVendorId = vendorId;
        LastCancellationTokenCanBeCanceled = cancellationToken.CanBeCanceled;

        if (NextException is not null)
        {
            return Task.FromException<RetrieveRegisteredVendorResult>(NextException);
        }

        return Task.FromResult(NextResult);
    }
}
