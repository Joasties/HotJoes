using HotJoes.Domain.Vendor;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.Infrastructure.Persistence;

public sealed class PostgreSqlVendorRepository : IVendorRepository
{
    private readonly VendorRegistrationDbContext _dbContext;

    public PostgreSqlVendorRepository(
        VendorRegistrationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async Task<VendorAggregate?> FindAsync(
        VendorId vendorId,
        CancellationToken cancellationToken)
    {
        VendorRegistrationRecord? record = await _dbContext
            .Set<VendorRegistrationRecord>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                candidate => candidate.VendorId == vendorId.Value,
                cancellationToken);

        return record is null
            ? null
            : VendorRegistrationRecordMapper.ToDomain(record);
    }

    public async Task AddAsync(
        VendorAggregate vendor,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(vendor);

        VendorRegistrationRecord record =
            VendorRegistrationRecordMapper.ToRecord(vendor);

        await _dbContext
            .Set<VendorRegistrationRecord>()
            .AddAsync(record, cancellationToken);
    }
}
