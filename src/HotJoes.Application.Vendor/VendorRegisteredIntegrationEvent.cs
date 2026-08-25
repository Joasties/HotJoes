namespace HotJoes.Application.Vendor;

public sealed record VendorRegisteredIntegrationEvent(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredAt,
    VendorRegisteredIntegrationEventPayload Payload);
