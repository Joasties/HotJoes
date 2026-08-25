using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Infrastructure.Persistence;

public sealed class PostgreSqlRegistrationOutcomeDeterminer
    : IRegistrationOutcomeDeterminer
{
    private readonly VendorRegistrationDbContext _dbContext;

    public PostgreSqlRegistrationOutcomeDeterminer(
        VendorRegistrationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async Task<RegistrationOutcomeDetermination> DetermineAsync(
        VendorRegistrationIdentity identity,
        RegistrationSemanticFingerprint fingerprint,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(fingerprint);

        string normalizedTradingName =
            identity.NormalizedTradingName.Trim().ToLowerInvariant();
        string normalizedLegalOperatorName =
            identity.NormalizedLegalOperatorName.Trim().ToLowerInvariant();
        string canonicalAddressId = identity.CanonicalAddressId.Value;

        PersistedOutcome? persistedOutcome = await (
            from vendor in _dbContext
                .Set<VendorRegistrationRecord>()
                .AsNoTracking()
            join outcome in _dbContext
                .Set<VendorRegistrationOutcomeRecord>()
                .AsNoTracking()
                on vendor.VendorId equals outcome.VendorId
            where vendor.NormalizedTradingName == normalizedTradingName
                && vendor.NormalizedLegalOperatorName ==
                    normalizedLegalOperatorName
                && vendor.CanonicalAddressId == canonicalAddressId
            select new PersistedOutcome(
                outcome.VendorId,
                outcome.FingerprintVersion,
                outcome.SemanticFingerprintSha256,
                outcome.ResultVendorState))
            .SingleOrDefaultAsync(cancellationToken);

        if (persistedOutcome is null)
        {
            return RegistrationOutcomeDetermination.FirstProcessingRequired();
        }

        byte[] suppliedDigest = Convert.FromHexString(
            fingerprint.Sha256Digest);
        bool equivalent =
            persistedOutcome.FingerprintVersion == fingerprint.Version
            && persistedOutcome.SemanticFingerprintSha256
                .AsSpan()
                .SequenceEqual(suppliedDigest);

        if (!equivalent)
        {
            return RegistrationOutcomeDetermination.ConflictDetected();
        }

        if (persistedOutcome.ResultVendorState != "pendingActivation")
        {
            throw new InvalidOperationException(
                "The persisted original registration result is not supported.");
        }

        RegisterVendorResult.Success originalResult = CreateOriginalResult(
            persistedOutcome.VendorId);

        return RegistrationOutcomeDetermination.Replay(originalResult);
    }

    private static RegisterVendorResult.Success CreateOriginalResult(
        Guid vendorId)
    {
        return (RegisterVendorResult.Success)RegisterVendorResult.Succeeded(
            new VendorId(vendorId));
    }

    private sealed record PersistedOutcome(
        Guid VendorId,
        short FingerprintVersion,
        byte[] SemanticFingerprintSha256,
        string ResultVendorState);
}
