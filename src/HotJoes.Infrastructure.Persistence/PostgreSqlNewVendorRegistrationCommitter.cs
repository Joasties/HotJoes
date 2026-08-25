using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace HotJoes.Infrastructure.Persistence;

public sealed class PostgreSqlNewVendorRegistrationCommitter
    : INewVendorRegistrationCommitter
{
    private const string CompositeIdentityConstraint =
        "uq_vendor_registrations_identity";

    private readonly VendorRegistrationDbContext _dbContext;
    private readonly VendorRegisteredIntegrationEventSerializer _serializer;

    public PostgreSqlNewVendorRegistrationCommitter(
        VendorRegistrationDbContext dbContext,
        VendorRegisteredIntegrationEventSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(serializer);

        _dbContext = dbContext;
        _serializer = serializer;
    }

    public async Task CommitAsync(
        NewVendorRegistrationCommit commit,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commit);

        SerializedIntegrationEvent serializedEvent =
            _serializer.Serialize(commit.IntegrationEvent);
        VendorRegistrationRecord vendorRecord =
            VendorRegistrationRecordMapper.ToRecord(commit.Vendor);

        vendorRecord.NormalizedTradingName =
            commit.Identity.NormalizedTradingName.ToLowerInvariant();
        vendorRecord.NormalizedLegalOperatorName =
            commit.Identity.NormalizedLegalOperatorName.ToLowerInvariant();
        vendorRecord.CanonicalAddressId =
            commit.Identity.CanonicalAddressId.Value;

        var outcomeRecord = new VendorRegistrationOutcomeRecord
        {
            VendorId = commit.OriginalResult.VendorId.Value,
            FingerprintVersion = commit.Fingerprint.Version,
            SemanticFingerprintSha256 = Convert.FromHexString(
                commit.Fingerprint.Sha256Digest),
            ResultVendorState = ToPersistenceValue(
                commit.OriginalResult.VendorState)
        };
        var outboxRecord = new VendorRegistrationOutboxRecord
        {
            EventId = serializedEvent.EventId,
            VendorId = commit.Vendor.Id.Value,
            EventVersion = serializedEvent.EventVersion,
            SerializedEvent = serializedEvent.SerializedEvent.ToArray(),
            PublishedAtUtc = null
        };

        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.Add(vendorRecord);
            _dbContext.Add(outcomeRecord);
            _dbContext.Add(outboxRecord);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsCompositeIdentityConflict(exception))
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw new ConcurrentVendorRegistrationException();
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    private static bool IsCompositeIdentityConflict(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            && postgresException.SqlState ==
                PostgresErrorCodes.UniqueViolation
            && postgresException.ConstraintName ==
                CompositeIdentityConstraint;
    }

    private static string ToPersistenceValue(VendorState value)
    {
        return value switch
        {
            VendorState.PendingActivation => "pendingActivation",
            VendorState.Activated => "activated",
            VendorState.Suspended => "suspended",
            VendorState.Deactivated => "deactivated",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }
}
