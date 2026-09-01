using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HotJoes.Infrastructure.ComplianceConsumer;

public sealed class ComplianceDeliveryProcessor
{
    private const string SupportedEventType = "VendorRegistered";
    private const int SupportedEventVersion = 1;

    private readonly IComplianceReceiptStore _receiptStore;
    private readonly ILogger<ComplianceDeliveryProcessor> _logger;

    public ComplianceDeliveryProcessor(
        IComplianceReceiptStore receiptStore,
        ILogger<ComplianceDeliveryProcessor>? logger = null)
    {
        _receiptStore = receiptStore ??
            throw new ArgumentNullException(nameof(receiptStore));
        _logger = logger ?? NullLogger<ComplianceDeliveryProcessor>.Instance;
    }

    public async Task<ComplianceDeliveryOutcome> ProcessAsync(
        ReadOnlyMemory<byte> serializedEvent,
        DateTimeOffset receivedAtUtc,
        IComplianceDeliveryAcknowledgement acknowledgement,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(acknowledgement);

        if (!TryReadEnvelope(serializedEvent, out EventEnvelope envelope))
        {
            RecordInvalidContract();
            return ComplianceDeliveryOutcome.InvalidContract;
        }

        var candidate = new ComplianceReceiptCandidate(
            envelope.EventId,
            SupportedEventType,
            SupportedEventVersion,
            receivedAtUtc,
            serializedEvent.Span);

        ComplianceReceiptOutcome receiptOutcome =
            await _receiptStore.RecordAsync(candidate, cancellationToken);

        switch (receiptOutcome)
        {
            case ComplianceReceiptOutcome.Recorded:
                await acknowledgement.AcknowledgeAsync(cancellationToken);
                RecordReceiptOutcome(
                    envelope.EventId,
                    "newReceipt",
                    LogLevel.Information);
                return ComplianceDeliveryOutcome.AcknowledgedNewReceipt;

            case ComplianceReceiptOutcome.EquivalentDuplicate:
                await acknowledgement.AcknowledgeAsync(cancellationToken);
                RecordReceiptOutcome(
                    envelope.EventId,
                    "equivalentDuplicate",
                    LogLevel.Information);
                ComplianceConsumerMetrics.RecordDuplicateReceipt();
                return ComplianceDeliveryOutcome
                    .AcknowledgedEquivalentDuplicate;

            case ComplianceReceiptOutcome.ConflictingBytes:
                RecordReceiptOutcome(
                    envelope.EventId,
                    "conflictingBytes",
                    LogLevel.Warning);
                return ComplianceDeliveryOutcome.ConflictingBytes;

            default:
                throw new InvalidOperationException(
                    $"Unsupported receipt outcome '{receiptOutcome}'.");
        }
    }

    private void RecordInvalidContract()
    {
        const string outcome = "invalidContract";

        _logger.LogWarning(
            "Compliance receipt {ConsumerReceiptOutcome} for received event",
            outcome);
        ComplianceConsumerMetrics.RecordConsumerOutcome(outcome);
    }

    private void RecordReceiptOutcome(
        Guid eventId,
        string outcome,
        LogLevel level)
    {
        if (level == LogLevel.Information)
        {
            _logger.LogInformation(
                "Compliance receipt {ConsumerReceiptOutcome} for " +
                "{EventType} event {EventId} version {EventVersion}",
                outcome,
                SupportedEventType,
                eventId,
                SupportedEventVersion);
        }
        else
        {
            _logger.LogWarning(
                "Compliance receipt {ConsumerReceiptOutcome} for " +
                "{EventType} event {EventId} version {EventVersion}",
                outcome,
                SupportedEventType,
                eventId,
                SupportedEventVersion);
        }

        ComplianceConsumerMetrics.RecordConsumerOutcome(outcome);
    }

    private static bool TryReadEnvelope(
        ReadOnlyMemory<byte> serializedEvent,
        out EventEnvelope envelope)
    {
        envelope = default;

        if (serializedEvent.IsEmpty)
        {
            return false;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(serializedEvent);
            JsonElement root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object ||
                !TryReadCanonicalGuid(root, "eventId", out Guid eventId) ||
                !HasExactString(root, "eventType", SupportedEventType) ||
                !HasExactInt32(
                    root,
                    "eventVersion",
                    SupportedEventVersion) ||
                !HasUtcTimestamp(root, "occurredAt") ||
                !TryGetObject(root, "payload", out JsonElement payload) ||
                !HasRequiredPayload(payload))
            {
                return false;
            }

            envelope = new EventEnvelope(eventId);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool HasRequiredPayload(JsonElement payload)
    {
        return
            TryReadCanonicalGuid(payload, "vendorId", out _) &&
            HasUtcTimestamp(payload, "registeredAt") &&
            HasNonEmptyString(payload, "vendorState") &&
            HasNonEmptyString(payload, "tradingPreference") &&
            HasNonEmptyString(payload, "legalOperatorType") &&
            TryGetObject(
                payload,
                "tradingCharacteristics",
                out JsonElement trading) &&
            HasRequiredTradingCharacteristics(trading) &&
            TryGetObject(
                payload,
                "businessAddress",
                out JsonElement address) &&
            HasRequiredBusinessAddress(address) &&
            HasNonEmptyString(payload, "foodRegistrationAuthority");
    }

    private static bool HasRequiredTradingCharacteristics(
        JsonElement trading)
    {
        return
            HasNonEmptyString(trading, "tradingLocation") &&
            TryGetObject(
                trading,
                "openingHours",
                out JsonElement openingHours) &&
            HasTime(openingHours, "startTime") &&
            HasTime(openingHours, "endTime") &&
            HasBoolean(trading, "serviceIncludesHotFood") &&
            HasBoolean(trading, "alcoholService");
    }

    private static bool HasRequiredBusinessAddress(JsonElement address)
    {
        return
            HasNonEmptyString(address, "canonicalAddressId") &&
            HasNonEmptyString(address, "addressLine1") &&
            HasNonEmptyString(address, "postTown") &&
            HasNonEmptyString(address, "postcode");
    }

    private static bool TryReadCanonicalGuid(
        JsonElement parent,
        string propertyName,
        out Guid value)
    {
        value = Guid.Empty;

        if (!parent.TryGetProperty(propertyName, out JsonElement property) ||
            property.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        string? text = property.GetString();
        return text is not null &&
            Guid.TryParseExact(text, "D", out value) &&
            value != Guid.Empty &&
            string.Equals(
                text,
                value.ToString("D"),
                StringComparison.Ordinal);
    }

    private static bool HasExactString(
        JsonElement parent,
        string propertyName,
        string expected)
    {
        return parent.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String &&
            string.Equals(
                property.GetString(),
                expected,
                StringComparison.Ordinal);
    }

    private static bool HasExactInt32(
        JsonElement parent,
        string propertyName,
        int expected)
    {
        return parent.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.Number &&
            property.TryGetInt32(out int value) &&
            value == expected;
    }

    private static bool HasUtcTimestamp(
        JsonElement parent,
        string propertyName)
    {
        return parent.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String &&
            property.TryGetDateTimeOffset(out DateTimeOffset value) &&
            value.Offset == TimeSpan.Zero;
    }

    private static bool HasTime(
        JsonElement parent,
        string propertyName)
    {
        return parent.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String &&
            TimeOnly.TryParseExact(
                property.GetString(),
                "HH:mm:ss",
                out _);
    }

    private static bool HasNonEmptyString(
        JsonElement parent,
        string propertyName)
    {
        return parent.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind == JsonValueKind.String &&
            !string.IsNullOrWhiteSpace(property.GetString());
    }

    private static bool HasBoolean(
        JsonElement parent,
        string propertyName)
    {
        return parent.TryGetProperty(propertyName, out JsonElement property) &&
            property.ValueKind is JsonValueKind.True or JsonValueKind.False;
    }

    private static bool TryGetObject(
        JsonElement parent,
        string propertyName,
        out JsonElement value)
    {
        return parent.TryGetProperty(propertyName, out value) &&
            value.ValueKind == JsonValueKind.Object;
    }

    private readonly record struct EventEnvelope(Guid EventId);
}
