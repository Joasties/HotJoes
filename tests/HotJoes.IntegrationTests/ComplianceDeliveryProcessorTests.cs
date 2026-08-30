using System.Text;
using System.Text.Json.Nodes;
using HotJoes.Infrastructure.ComplianceConsumer;

namespace HotJoes.IntegrationTests;

public sealed class ComplianceDeliveryProcessorTests
{
    private static readonly Guid EventId = Guid.Parse(
        "81d2a757-fefd-42c9-bd82-e3ebc3a09146");

    private static readonly DateTimeOffset ReceivedAtUtc = new(
        2026,
        8,
        28,
        19,
        0,
        0,
        TimeSpan.Zero);

    private static readonly byte[] ValidSerializedEvent = """
        {
          "eventId": "81d2a757-fefd-42c9-bd82-e3ebc3a09146",
          "eventType": "VendorRegistered",
          "eventVersion": 1,
          "occurredAt": "2026-08-28T18:00:00.0000000Z",
          "payload": {
            "vendorId": "4e512746-8714-4e31-8b29-e8a262dd54b2",
            "registeredAt": "2026-08-28T18:00:00.0000000Z",
            "vendorState": "pendingActivation",
            "tradingPreference": "online",
            "legalOperatorType": "soleTrader",
            "tradingCharacteristics": {
              "tradingLocation": "restaurant",
              "openingHours": {
                "startTime": "09:00:00",
                "endTime": "17:00:00"
              },
              "serviceIncludesHotFood": true,
              "alcoholService": false
            },
            "businessAddress": {
              "canonicalAddressId": "address-123",
              "recipientOrOrganisationName": null,
              "addressLine1": "2 High Street",
              "addressLine2": null,
              "addressLine3": null,
              "postTown": "LEATHERHEAD",
              "postcode": "KT22 7QS",
              "county": null
            },
            "foodRegistrationAuthority": "Mole Valley District Council",
            "primaryTradingAuthority": null
          }
        }
        """u8.ToArray();

    [Fact]
    public async Task ProcessAsync_ValidFirstDelivery_PersistsBeforeAcknowledgement()
    {
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.Recorded,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            ValidSerializedEvent,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(
            ComplianceDeliveryOutcome.AcknowledgedNewReceipt,
            outcome);
        Assert.Equal(["persist", "acknowledge"], calls);
        ComplianceReceiptCandidate candidate = Assert.Single(
            store.Candidates);
        Assert.Equal(EventId, candidate.EventId);
        Assert.Equal("VendorRegistered", candidate.EventType);
        Assert.Equal(1, candidate.EventVersion);
        Assert.Equal(ReceivedAtUtc, candidate.ReceivedAtUtc);
        Assert.Equal(
            ValidSerializedEvent,
            candidate.SerializedEvent.ToArray());
    }

    [Fact]
    public async Task ProcessAsync_EquivalentDuplicate_AcknowledgesAfterClassification()
    {
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.EquivalentDuplicate,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            ValidSerializedEvent,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(
            ComplianceDeliveryOutcome.AcknowledgedEquivalentDuplicate,
            outcome);
        Assert.Equal(["persist", "acknowledge"], calls);
        Assert.Equal(1, acknowledgement.CallCount);
    }

    [Fact]
    public async Task ProcessAsync_ReceiptFailure_DoesNotAcknowledge()
    {
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            new InvalidOperationException("receipt unavailable"),
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            processor.ProcessAsync(
                ValidSerializedEvent,
                ReceivedAtUtc,
                acknowledgement));

        Assert.Equal(["persist"], calls);
        Assert.Equal(0, acknowledgement.CallCount);
    }

    [Fact]
    public async Task ProcessAsync_ConflictingBytes_DoesNotAcknowledge()
    {
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.ConflictingBytes,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            ValidSerializedEvent,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(ComplianceDeliveryOutcome.ConflictingBytes, outcome);
        Assert.Equal(["persist"], calls);
        Assert.Equal(0, acknowledgement.CallCount);
    }

    [Theory]
    [InlineData("eventId")]
    [InlineData("eventType")]
    [InlineData("eventVersion")]
    [InlineData("occurredAt")]
    [InlineData("payload")]
    public async Task ProcessAsync_MissingRequiredEnvelopeMember_DoesNotPersistOrAcknowledge(
        string memberName)
    {
        byte[] invalidEvent = RemoveMember(
            ValidSerializedEvent,
            memberName);
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.Recorded,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            invalidEvent,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(ComplianceDeliveryOutcome.InvalidContract, outcome);
        Assert.Empty(store.Candidates);
        Assert.Empty(calls);
        Assert.Equal(0, acknowledgement.CallCount);
    }

    [Theory]
    [InlineData("vendorId")]
    [InlineData("registeredAt")]
    [InlineData("vendorState")]
    [InlineData("tradingPreference")]
    [InlineData("legalOperatorType")]
    [InlineData("tradingCharacteristics")]
    [InlineData("businessAddress")]
    [InlineData("foodRegistrationAuthority")]
    public async Task ProcessAsync_MissingRequiredPayloadMember_DoesNotPersistOrAcknowledge(
        string memberName)
    {
        byte[] invalidEvent = RemovePayloadMember(
            ValidSerializedEvent,
            memberName);
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.Recorded,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            invalidEvent,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(ComplianceDeliveryOutcome.InvalidContract, outcome);
        Assert.Empty(store.Candidates);
        Assert.Empty(calls);
        Assert.Equal(0, acknowledgement.CallCount);
    }

    [Theory]
    [InlineData("payload.tradingCharacteristics", "tradingLocation")]
    [InlineData("payload.tradingCharacteristics", "openingHours")]
    [InlineData("payload.tradingCharacteristics", "serviceIncludesHotFood")]
    [InlineData("payload.tradingCharacteristics", "alcoholService")]
    [InlineData("payload.tradingCharacteristics.openingHours", "startTime")]
    [InlineData("payload.tradingCharacteristics.openingHours", "endTime")]
    [InlineData("payload.businessAddress", "canonicalAddressId")]
    [InlineData("payload.businessAddress", "addressLine1")]
    [InlineData("payload.businessAddress", "postTown")]
    [InlineData("payload.businessAddress", "postcode")]
    public async Task ProcessAsync_MissingRequiredNestedMember_DoesNotPersistOrAcknowledge(
        string parentPath,
        string memberName)
    {
        byte[] invalidEvent = RemoveNestedMember(
            ValidSerializedEvent,
            parentPath,
            memberName);
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.Recorded,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            invalidEvent,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(ComplianceDeliveryOutcome.InvalidContract, outcome);
        Assert.Empty(store.Candidates);
        Assert.Empty(calls);
        Assert.Equal(0, acknowledgement.CallCount);
    }

    [Theory]
    [InlineData("OtherEvent", 1)]
    [InlineData("VendorRegistered", 2)]
    public async Task ProcessAsync_UnsupportedTypeOrVersion_DoesNotPersistOrAcknowledge(
        string eventType,
        int eventVersion)
    {
        JsonObject document = ParseEvent(ValidSerializedEvent);
        document["eventType"] = eventType;
        document["eventVersion"] = eventVersion;
        byte[] invalidEvent = Encoding.UTF8.GetBytes(document.ToJsonString());
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.Recorded,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            invalidEvent,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(ComplianceDeliveryOutcome.InvalidContract, outcome);
        Assert.Empty(store.Candidates);
        Assert.Empty(calls);
        Assert.Equal(0, acknowledgement.CallCount);
    }

    [Fact]
    public async Task ProcessAsync_CompatibleUnknownMember_PersistsExactOriginalBytes()
    {
        JsonObject document = ParseEvent(ValidSerializedEvent);
        document["compatibleFutureMember"] = "ignored";
        byte[] eventWithUnknownMember = Encoding.UTF8.GetBytes(
            document.ToJsonString());
        var calls = new List<string>();
        var store = new RecordingReceiptStore(
            ComplianceReceiptOutcome.Recorded,
            calls);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new ComplianceDeliveryProcessor(store);

        ComplianceDeliveryOutcome outcome = await processor.ProcessAsync(
            eventWithUnknownMember,
            ReceivedAtUtc,
            acknowledgement);

        Assert.Equal(
            ComplianceDeliveryOutcome.AcknowledgedNewReceipt,
            outcome);
        ComplianceReceiptCandidate candidate = Assert.Single(
            store.Candidates);
        Assert.Equal(
            eventWithUnknownMember,
            candidate.SerializedEvent.ToArray());
        Assert.Equal(["persist", "acknowledge"], calls);
    }

    private static byte[] RemoveMember(
        byte[] serializedEvent,
        string memberName)
    {
        JsonObject document = ParseEvent(serializedEvent);
        Assert.True(document.Remove(memberName));
        return Encoding.UTF8.GetBytes(document.ToJsonString());
    }

    private static byte[] RemovePayloadMember(
        byte[] serializedEvent,
        string memberName)
    {
        JsonObject document = ParseEvent(serializedEvent);
        JsonObject payload = Assert.IsType<JsonObject>(document["payload"]);
        Assert.True(payload.Remove(memberName));
        return Encoding.UTF8.GetBytes(document.ToJsonString());
    }

    private static byte[] RemoveNestedMember(
        byte[] serializedEvent,
        string parentPath,
        string memberName)
    {
        JsonObject document = ParseEvent(serializedEvent);
        JsonObject parent = document;

        foreach (string segment in parentPath.Split('.'))
        {
            parent = Assert.IsType<JsonObject>(parent[segment]);
        }

        Assert.True(parent.Remove(memberName));
        return Encoding.UTF8.GetBytes(document.ToJsonString());
    }

    private static JsonObject ParseEvent(byte[] serializedEvent)
    {
        return Assert.IsType<JsonObject>(JsonNode.Parse(serializedEvent));
    }

    private sealed class RecordingReceiptStore : IComplianceReceiptStore
    {
        private readonly ComplianceReceiptOutcome? _outcome;
        private readonly Exception? _exception;
        private readonly List<string> _calls;

        public RecordingReceiptStore(
            ComplianceReceiptOutcome outcome,
            List<string> calls)
        {
            _outcome = outcome;
            _calls = calls;
        }

        public RecordingReceiptStore(
            Exception exception,
            List<string> calls)
        {
            _exception = exception;
            _calls = calls;
        }

        public List<ComplianceReceiptCandidate> Candidates { get; } = [];

        public Task<ComplianceReceiptOutcome> RecordAsync(
            ComplianceReceiptCandidate candidate,
            CancellationToken cancellationToken = default)
        {
            _calls.Add("persist");
            Candidates.Add(candidate);

            if (_exception is not null)
            {
                throw _exception;
            }

            return Task.FromResult(
                _outcome ?? throw new InvalidOperationException(
                    "The test receipt outcome was not configured."));
        }
    }

    private sealed class RecordingAcknowledgement
        : IComplianceDeliveryAcknowledgement
    {
        private readonly List<string> _calls;

        public RecordingAcknowledgement(List<string> calls)
        {
            _calls = calls;
        }

        public int CallCount { get; private set; }

        public Task AcknowledgeAsync(
            CancellationToken cancellationToken = default)
        {
            _calls.Add("acknowledge");
            CallCount++;
            return Task.CompletedTask;
        }
    }
}
