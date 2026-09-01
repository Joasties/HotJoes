using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotJoes.Infrastructure.Persistence;

public sealed class PostgreSqlRegistrationOutcomeDeterminer
    : IRegistrationOutcomeDeterminer
{
    private readonly VendorRegistrationDbContext _dbContext;
    private readonly ILogger<PostgreSqlRegistrationOutcomeDeterminer> _logger;

    public PostgreSqlRegistrationOutcomeDeterminer(
        VendorRegistrationDbContext dbContext,
        ILogger<PostgreSqlRegistrationOutcomeDeterminer>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
        _logger = logger ??
            NullLogger<PostgreSqlRegistrationOutcomeDeterminer>.Instance;
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
            RecordFirstProcessing();
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
            RecordPersistedOutcome(
                persistedOutcome.VendorId,
                "conflict");
            return RegistrationOutcomeDetermination.ConflictDetected();
        }

        if (persistedOutcome.ResultVendorState != "pendingActivation")
        {
            throw new InvalidOperationException(
                "The persisted original registration result is not supported.");
        }

        RegisterVendorResult.Success originalResult = CreateOriginalResult(
            persistedOutcome.VendorId);

        RecordPersistedOutcome(
            persistedOutcome.VendorId,
            "equivalentReplay");

        return RegistrationOutcomeDetermination.Replay(originalResult);
    }

    private void RecordFirstProcessing()
    {
        const string outcome = "firstProcessing";

        _logger.LogInformation(
            "Vendor registration idempotency outcome {IdempotencyOutcome}",
            outcome);
        RegistrationPersistenceMetrics.RecordIdempotencyOutcome(outcome);
    }

    private void RecordPersistedOutcome(
        Guid vendorId,
        string outcome)
    {
        _logger.LogInformation(
            "Vendor {VendorId} registration idempotency outcome " +
            "{IdempotencyOutcome}",
            vendorId,
            outcome);
        RegistrationPersistenceMetrics.RecordIdempotencyOutcome(outcome);
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
