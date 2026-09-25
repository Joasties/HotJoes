using System.Diagnostics;
using HotJoes.Application.Community;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace HotJoes.Infrastructure.Community.Persistence;

public sealed class PostgreSqlCommunityParticipationCommitter
    : ICommunityParticipationCommitter
{
    private const string VendorIdConstraint =
        "uq_community_participations_vendor_id";

    private readonly CommunityPersistenceDbContext _dbContext;
    private readonly CommunityParticipationRecordedIntegrationEventMapper
        _eventMapper;
    private readonly CommunityParticipationRecordedIntegrationEventSerializer
        _eventSerializer;
    private readonly ICommunityPersistenceIdentityGenerator _identityGenerator;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<PostgreSqlCommunityParticipationCommitter> _logger;

    public PostgreSqlCommunityParticipationCommitter(
        CommunityPersistenceDbContext dbContext,
        CommunityParticipationRecordedIntegrationEventMapper eventMapper,
        CommunityParticipationRecordedIntegrationEventSerializer eventSerializer,
        ICommunityPersistenceIdentityGenerator identityGenerator,
        TimeProvider timeProvider,
        ILogger<PostgreSqlCommunityParticipationCommitter>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(eventMapper);
        ArgumentNullException.ThrowIfNull(eventSerializer);
        ArgumentNullException.ThrowIfNull(identityGenerator);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _dbContext = dbContext;
        _eventMapper = eventMapper;
        _eventSerializer = eventSerializer;
        _identityGenerator = identityGenerator;
        _timeProvider = timeProvider;
        _logger = logger ??
            NullLogger<PostgreSqlCommunityParticipationCommitter>.Instance;
    }

    public async ValueTask<CommunityParticipationCommitResult> CommitAsync(
        JoinCommunityRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            CommunityParticipationRecord? existing = await _dbContext
                .Set<CommunityParticipationRecord>()
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    record => record.VendorId == request.VendorId,
                    cancellationToken);

            if (existing is not null)
            {
                return Classify(existing, request.ContactPreference);
            }

            return await CommitFirstAsync(request, cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            RecordOutcome(request.VendorId, "unavailable", LogLevel.Warning);
            return CommunityParticipationCommitResult.TemporarilyUnavailable();
        }
    }

    private async Task<CommunityParticipationCommitResult> CommitFirstAsync(
        JoinCommunityRequest request,
        CancellationToken cancellationToken)
    {
        Guid participationId =
            _identityGenerator.NewCommunityParticipationId();
        Guid eventId = _identityGenerator.NewEventId();
        DateTimeOffset joinedAt = _timeProvider.GetUtcNow().ToUniversalTime();
        var participation = new CommunityParticipation(
            participationId,
            request.VendorId,
            request.ContactPreference,
            joinedAt);
        CommunityParticipationRecordedIntegrationEvent integrationEvent =
            _eventMapper.Map(participation, eventId, joinedAt);
        SerializedCommunityIntegrationEvent serializedEvent =
            _eventSerializer.Serialize(integrationEvent);
        Activity? activity = Activity.Current;
        bool hasTraceContext = activity is
        {
            IdFormat: ActivityIdFormat.W3C,
            Id: not null
        };

        var participationRecord = new CommunityParticipationRecord
        {
            CommunityParticipationId = participationId,
            VendorId = request.VendorId,
            ContactPreference = ToPersistenceValue(request.ContactPreference),
            JoinedAtUtc = joinedAt
        };
        var outboxRecord = new CommunityParticipationOutboxRecord
        {
            EventId = serializedEvent.EventId,
            CommunityParticipationId = participationId,
            EventVersion = serializedEvent.EventVersion,
            SerializedEvent = serializedEvent.SerializedEvent.ToArray(),
            TraceParent = hasTraceContext ? activity!.Id : null,
            TraceState = hasTraceContext ? activity!.TraceStateString : null,
            PublishedAtUtc = null
        };

        await using IDbContextTransaction transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _dbContext.Add(participationRecord);
            _dbContext.Add(outboxRecord);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            RecordOutcome(request.VendorId, "recorded", LogLevel.Information);
            return CommunityParticipationCommitResult.Recorded(participation);
        }
        catch (DbUpdateException exception)
            when (IsVendorIdConflict(exception))
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _dbContext.ChangeTracker.Clear();
            CommunityParticipationRecord existing = await _dbContext
                .Set<CommunityParticipationRecord>()
                .AsNoTracking()
                .SingleAsync(
                    record => record.VendorId == request.VendorId,
                    cancellationToken);
            CommunityParticipationCommitResult result = Classify(
                existing,
                request.ContactPreference);
            RecordOutcome(
                request.VendorId,
                result is CommunityParticipationCommitResult.EquivalentReplay
                    ? "alreadyRecorded"
                    : "conflict",
                LogLevel.Information);
            return result;
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _dbContext.ChangeTracker.Clear();
            RecordOutcome(request.VendorId, "unavailable", LogLevel.Warning);
            return CommunityParticipationCommitResult.TemporarilyUnavailable();
        }
    }

    private static CommunityParticipationCommitResult Classify(
        CommunityParticipationRecord existing,
        ContactPreference requestedPreference)
    {
        ContactPreference persistedPreference = FromPersistenceValue(
            existing.ContactPreference);

        if (persistedPreference != requestedPreference)
        {
            return CommunityParticipationCommitResult.Conflict();
        }

        return CommunityParticipationCommitResult.AlreadyRecorded(
            new CommunityParticipation(
                existing.CommunityParticipationId,
                existing.VendorId,
                persistedPreference,
                existing.JoinedAtUtc));
    }

    private static bool IsVendorIdConflict(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
            && postgresException.ConstraintName == VendorIdConstraint;
    }

    private static string ToPersistenceValue(ContactPreference preference)
    {
        return preference switch
        {
            ContactPreference.Email => "email",
            ContactPreference.Sms => "sms",
            ContactPreference.WhatsApp => "whatsApp",
            _ => throw new ArgumentOutOfRangeException(nameof(preference))
        };
    }

    private static ContactPreference FromPersistenceValue(string preference)
    {
        return preference switch
        {
            "email" => ContactPreference.Email,
            "sms" => ContactPreference.Sms,
            "whatsApp" => ContactPreference.WhatsApp,
            _ => throw new InvalidOperationException(
                "Stored Contact Preference is not supported.")
        };
    }

    private void RecordOutcome(
        Guid vendorId,
        string outcome,
        LogLevel level)
    {
        if (level == LogLevel.Information)
        {
            _logger.LogInformation(
                "Community participation persistence {PersistenceOutcome} " +
                "for Vendor {VendorId}",
                outcome,
                vendorId);
        }
        else
        {
            _logger.LogWarning(
                "Community participation persistence {PersistenceOutcome} " +
                "for Vendor {VendorId}",
                outcome,
                vendorId);
        }
    }
}
