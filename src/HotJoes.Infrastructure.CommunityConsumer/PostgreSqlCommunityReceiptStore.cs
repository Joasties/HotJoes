using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
namespace HotJoes.Infrastructure.CommunityConsumer;
public sealed class PostgreSqlCommunityReceiptStore : ICommunityReceiptStore
{
    private readonly CommunityReceiptDbContext context;
    public PostgreSqlCommunityReceiptStore(CommunityReceiptDbContext context) => this.context = context;
    public async Task<CommunityReceiptOutcome> ClassifyAsync(CommunityReceiptCandidate candidate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        byte[] hash = SHA256.HashData(candidate.SerializedEvent.Span);
        CommunityReceiptRecord? existing = await context.Set<CommunityReceiptRecord>().AsNoTracking().SingleOrDefaultAsync(x => x.EventId == candidate.EventId, cancellationToken);
        if (existing is not null) return existing.SerializedEventSha256.SequenceEqual(hash) ? CommunityReceiptOutcome.EquivalentDuplicate : CommunityReceiptOutcome.ConflictingBytes;
        context.Set<CommunityReceiptRecord>().Add(new CommunityReceiptRecord
        {
            EventId = candidate.EventId, EventType = candidate.EventType,
            EventVersion = candidate.EventVersion, CommunityParticipationId = candidate.CommunityParticipationId,
            VendorId = candidate.VendorId, ReceivedAtUtc = candidate.ReceivedAtUtc,
            SerializedEventSha256 = hash
        });
        try { await context.SaveChangesAsync(cancellationToken); return CommunityReceiptOutcome.Recorded; }
        catch (DbUpdateException)
        {
            context.ChangeTracker.Clear();
            existing = await context.Set<CommunityReceiptRecord>().AsNoTracking().SingleAsync(x => x.EventId == candidate.EventId, cancellationToken);
            return existing.SerializedEventSha256.SequenceEqual(hash) ? CommunityReceiptOutcome.EquivalentDuplicate : CommunityReceiptOutcome.ConflictingBytes;
        }
    }
}
