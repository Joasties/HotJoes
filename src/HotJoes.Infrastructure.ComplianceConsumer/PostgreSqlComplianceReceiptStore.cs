using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class PostgreSqlComplianceReceiptStore
    : IComplianceReceiptStore
{
    private readonly ComplianceReceiptDbContext _context;

    public PostgreSqlComplianceReceiptStore(
        ComplianceReceiptDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ComplianceReceiptOutcome> RecordAsync(
        ComplianceReceiptCandidate candidate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        byte[] digest = SHA256.HashData(candidate.SerializedEvent.Span);
        ComplianceReceiptRecord? existing = await FindAsync(
            candidate.EventId,
            cancellationToken);

        if (existing is not null)
        {
            return Classify(existing, digest);
        }

        var receipt = new ComplianceReceiptRecord
        {
            EventId = candidate.EventId,
            EventType = candidate.EventType,
            EventVersion = candidate.EventVersion,
            ReceivedAtUtc = candidate.ReceivedAtUtc,
            SerializedEventSha256 = digest
        };
        _context.Set<ComplianceReceiptRecord>().Add(receipt);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return ComplianceReceiptOutcome.Recorded;
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            _context.ChangeTracker.Clear();
            existing = await FindAsync(candidate.EventId, cancellationToken);

            if (existing is null)
            {
                throw;
            }

            return Classify(existing, digest);
        }
    }

    private Task<ComplianceReceiptRecord?> FindAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        return _context.Set<ComplianceReceiptRecord>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                receipt => receipt.EventId == eventId,
                cancellationToken);
    }

    private static ComplianceReceiptOutcome Classify(
        ComplianceReceiptRecord existing,
        ReadOnlySpan<byte> digest)
    {
        return CryptographicOperations.FixedTimeEquals(
            existing.SerializedEventSha256,
            digest)
            ? ComplianceReceiptOutcome.EquivalentDuplicate
            : ComplianceReceiptOutcome.ConflictingBytes;
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgres &&
            postgres.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
