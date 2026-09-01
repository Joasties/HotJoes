using System.Diagnostics;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace HotJoes.Infrastructure.Persistence;

public sealed class PostgreSqlNewVendorRegistrationCommitter
    : INewVendorRegistrationCommitter
{
    private const string CompositeIdentityConstraint =
        "uq_vendor_registrations_identity";

    private readonly VendorRegistrationDbContext _dbContext;
    private readonly VendorRegisteredIntegrationEventSerializer _serializer;
    private readonly ILogger<PostgreSqlNewVendorRegistrationCommitter> _logger;

    public PostgreSqlNewVendorRegistrationCommitter(
        VendorRegistrationDbContext dbContext,
        VendorRegisteredIntegrationEventSerializer serializer,
        ILogger<PostgreSqlNewVendorRegistrationCommitter>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(serializer);

        _dbContext = dbContext;
        _serializer = serializer;
        _logger = logger ??
            NullLogger<PostgreSqlNewVendorRegistrationCommitter>.Instance;
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
        Activity? currentActivity = Activity.Current;
        bool hasW3CTraceContext = currentActivity is
        {
            IdFormat: ActivityIdFormat.W3C,
            Id: not null
        };

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
            TraceParent = hasW3CTraceContext ? currentActivity!.Id : null,
            TraceState = hasW3CTraceContext
                ? currentActivity!.TraceStateString
                : null,
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
            RecordPersistenceOutcome(
                commit,
                serializedEvent,
                "concurrentConflict",
                LogLevel.Warning);
            throw new ConcurrentVendorRegistrationException();
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            RecordPersistenceOutcome(
                commit,
                serializedEvent,
                "failed",
                LogLevel.Warning);
            throw;
        }

        RecordPersistenceOutcome(
            commit,
            serializedEvent,
            "committed",
            LogLevel.Information);
    }

    private void RecordPersistenceOutcome(
        NewVendorRegistrationCommit commit,
        SerializedIntegrationEvent serializedEvent,
        string outcome,
        LogLevel level)
    {
        if (level == LogLevel.Information)
        {
            _logger.LogInformation(
                "Vendor {VendorId} persistence {PersistenceOutcome} for " +
                "{EventType} event {EventId} version {EventVersion}",
                commit.Vendor.Id.Value,
                outcome,
                "VendorRegistered",
                serializedEvent.EventId,
                serializedEvent.EventVersion);
        }
        else
        {
            _logger.LogWarning(
                "Vendor {VendorId} persistence {PersistenceOutcome} for " +
                "{EventType} event {EventId} version {EventVersion}",
                commit.Vendor.Id.Value,
                outcome,
                "VendorRegistered",
                serializedEvent.EventId,
                serializedEvent.EventVersion);
        }

        RegistrationPersistenceMetrics.RecordPersistenceOutcome(outcome);
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
