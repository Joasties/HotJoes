using System.Text.Json;

namespace HotJoes.Infrastructure.CommunityConsumer;

public sealed class CommunityDeliveryProcessor
{
    private const string EventType = "CommunityParticipationRecorded";
    private readonly ICommunityParticipationRecordedProcessor processor;
    private readonly ICommunityReceiptStore store;
    public CommunityDeliveryProcessor(ICommunityParticipationRecordedProcessor processor, ICommunityReceiptStore store)
    { this.processor = processor; this.store = store; }

    public async Task<CommunityDeliveryOutcome> ProcessAsync(ReadOnlyMemory<byte> serializedEvent,
        DateTimeOffset receivedAtUtc, ICommunityDeliveryAcknowledgement acknowledgement,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(acknowledgement);
        if (!TryRead(serializedEvent, out CommunityParticipationRecordedMessage? message))
            return CommunityDeliveryOutcome.InvalidContract;
        CommunityParticipationRecordedMessage validMessage = message!;
        await processor.ProcessAsync(validMessage, cancellationToken);
        var candidate = new CommunityReceiptCandidate(validMessage.EventId, EventType, 1,
            validMessage.CommunityParticipationId, validMessage.VendorId, receivedAtUtc,
            serializedEvent.Span);
        CommunityReceiptOutcome receipt = await store.ClassifyAsync(candidate, cancellationToken);
        if (receipt == CommunityReceiptOutcome.ConflictingBytes)
            return CommunityDeliveryOutcome.ConflictingBytes;
        await acknowledgement.AcknowledgeAsync(cancellationToken);
        return receipt == CommunityReceiptOutcome.Recorded
            ? CommunityDeliveryOutcome.AcknowledgedNewReceipt
            : CommunityDeliveryOutcome.AcknowledgedEquivalentDuplicate;
    }

    private static bool TryRead(ReadOnlyMemory<byte> bytes, out CommunityParticipationRecordedMessage? message)
    {
        message = null;
        try
        {
            using JsonDocument document = JsonDocument.Parse(bytes);
            JsonElement root = document.RootElement;
            if (!GuidValue(root, "eventId", out Guid eventId) ||
                !StringValue(root, "eventType", out string? type) || type != EventType ||
                !IntValue(root, "eventVersion", out int version) || version != 1 ||
                !UtcValue(root, "occurredAt", out _) ||
                !root.TryGetProperty("payload", out JsonElement payload) || payload.ValueKind != JsonValueKind.Object ||
                !GuidValue(payload, "communityParticipationId", out Guid participationId) ||
                !GuidValue(payload, "vendorId", out Guid vendorId) ||
                !UtcValue(payload, "joinedAt", out DateTimeOffset joinedAt) ||
                !StringValue(payload, "contactPreference", out string? preference) ||
                !TryPreference(preference, out CommunityContactPreference contactPreference)) return false;
            message = new(eventId, participationId, vendorId, joinedAt, contactPreference);
            return true;
        }
        catch (JsonException) { return false; }
    }

    private static bool GuidValue(JsonElement parent, string name, out Guid value)
    {
        value = default;
        return parent.TryGetProperty(name, out JsonElement item) && item.ValueKind == JsonValueKind.String &&
            Guid.TryParseExact(item.GetString(), "D", out value) && value != Guid.Empty && item.GetString() == value.ToString("D");
    }
    private static bool StringValue(JsonElement parent, string name, out string? value)
    {
        value = null;
        if (!parent.TryGetProperty(name, out JsonElement item) || item.ValueKind != JsonValueKind.String) return false;
        value = item.GetString(); return !string.IsNullOrWhiteSpace(value);
    }
    private static bool IntValue(JsonElement parent, string name, out int value)
    { value = default; return parent.TryGetProperty(name, out JsonElement item) && item.TryGetInt32(out value); }
    private static bool UtcValue(JsonElement parent, string name, out DateTimeOffset value)
    {
        value = default;
        return parent.TryGetProperty(name, out JsonElement item) && item.ValueKind == JsonValueKind.String &&
            DateTimeOffset.TryParse(item.GetString(), System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind, out value) && value.Offset == TimeSpan.Zero;
    }
    private static bool TryPreference(string? text, out CommunityContactPreference value)
    {
        value = text switch { "email" => CommunityContactPreference.Email, "sms" => CommunityContactPreference.Sms, "whatsApp" => CommunityContactPreference.WhatsApp, _ => default };
        return value != default;
    }
}
