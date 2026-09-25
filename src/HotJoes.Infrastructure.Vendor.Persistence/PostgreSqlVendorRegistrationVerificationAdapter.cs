using HotJoes.Application.Community;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.Vendor.Persistence;

public sealed class PostgreSqlVendorRegistrationVerificationAdapter
    : IVendorRegistrationVerificationPort
{
    private readonly VendorRegistrationDbContext _dbContext;

    public PostgreSqlVendorRegistrationVerificationAdapter(
        VendorRegistrationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async ValueTask<VendorRegistrationVerificationResult> VerifyAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            bool exists = await _dbContext
                .Set<VendorRegistrationRecord>()
                .AsNoTracking()
                .AnyAsync(
                    record => record.VendorId == vendorId,
                    cancellationToken);

            return exists
                ? VendorRegistrationVerificationResult.Registered()
                : VendorRegistrationVerificationResult.NotFound();
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return VendorRegistrationVerificationResult.TemporarilyUnavailable();
        }
    }
}
