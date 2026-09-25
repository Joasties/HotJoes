using System.Text;
using System.Text.Json.Nodes;
using HotJoes.Infrastructure.CommunityConsumer;

namespace HotJoes.IntegrationTests;

public sealed class CommunityDeliveryProcessorTests
{
    private static readonly Guid EventId = Guid.Parse("7bb04edf-b6b0-4e52-8600-c9ac9b71cc36");
    private static readonly Guid ParticipationId = Guid.Parse("b590cd67-9bf2-46fd-b4aa-02f25f404275");
    private static readonly Guid VendorId = Guid.Parse("7ecf7027-d8f8-4bfb-8559-c65896e52e48");
    private static readonly DateTimeOffset ReceivedAt = new(2026, 9, 23, 18, 0, 0, TimeSpan.Zero);
    private static readonly byte[] ValidEvent = Encoding.UTF8.GetBytes("""
        {"eventId":"7bb04edf-b6b0-4e52-8600-c9ac9b71cc36","eventType":"CommunityParticipationRecorded","eventVersion":1,"occurredAt":"2026-09-23T17:00:00.0000000Z","payload":{"communityParticipationId":"b590cd67-9bf2-46fd-b4aa-02f25f404275","vendorId":"7ecf7027-d8f8-4bfb-8559-c65896e52e48","joinedAt":"2026-09-23T17:00:00.0000000Z","contactPreference":"email"}}
        """);

    [Fact]
    public async Task VR_COMMUNITY_020_FirstDeliveryProcessesPersistsThenAcknowledges()
    {
        var calls = new List<string>();
        var stub = new RecordingStub(calls);
        var store = new RecordingStore(calls, CommunityReceiptOutcome.Recorded);
        var acknowledgement = new RecordingAcknowledgement(calls);
        var processor = new CommunityDeliveryProcessor(stub, store);

        CommunityDeliveryOutcome outcome = await processor.ProcessAsync(
            ValidEvent, ReceivedAt, acknowledgement);

        Assert.Equal(CommunityDeliveryOutcome.AcknowledgedNewReceipt, outcome);
        Assert.Equal(["process", "persist", "acknowledge"], calls);
        CommunityParticipationRecordedMessage message = Assert.Single(stub.Messages);
        Assert.Equal(EventId, message.EventId);
        Assert.Equal(ParticipationId, message.CommunityParticipationId);
        Assert.Equal(VendorId, message.VendorId);
        Assert.Equal(CommunityContactPreference.Email, message.ContactPreference);
        CommunityReceiptCandidate receipt = Assert.Single(store.Candidates);
        Assert.Equal(ValidEvent, receipt.SerializedEvent.ToArray());
    }

    [Fact]
    public async Task VR_COMMUNITY_020_EquivalentDuplicateReprocessesDeterministicallyAndAcknowledges()
    {
        var calls = new List<string>();
        var stub = new RecordingStub(calls);
        var store = new RecordingStore(calls,
            CommunityReceiptOutcome.EquivalentDuplicate);
        var acknowledgement = new RecordingAcknowledgement(calls);

        var outcome = await new CommunityDeliveryProcessor(stub, store)
            .ProcessAsync(ValidEvent, ReceivedAt, acknowledgement);

        Assert.Equal(CommunityDeliveryOutcome.AcknowledgedEquivalentDuplicate, outcome);
        Assert.Equal(["process", "persist", "acknowledge"], calls);
        Assert.Single(stub.Messages);
    }

    [Fact]
    public async Task VR_COMMUNITY_020_ConflictingBytesAreIntegrityFailureWithoutAcknowledgement()
    {
        var calls = new List<string>();
        var acknowledgement = new RecordingAcknowledgement(calls);
        var outcome = await new CommunityDeliveryProcessor(
            new RecordingStub(calls),
            new RecordingStore(calls, CommunityReceiptOutcome.ConflictingBytes))
            .ProcessAsync(ValidEvent, ReceivedAt, acknowledgement);

        Assert.Equal(CommunityDeliveryOutcome.ConflictingBytes, outcome);
        Assert.Equal(["process", "persist"], calls);
        Assert.Equal(0, acknowledgement.Count);
    }

    [Theory]
    [InlineData("eventId")]
    [InlineData("eventType")]
    [InlineData("eventVersion")]
    [InlineData("occurredAt")]
    [InlineData("payload")]
    public async Task VR_COMMUNITY_021_InvalidEnvelopeIsNotProcessedPersistedOrAcknowledged(string member)
    {
        JsonObject json = JsonNode.Parse(ValidEvent)!.AsObject();
        Assert.True(json.Remove(member));
        var calls = new List<string>();
        var acknowledgement = new RecordingAcknowledgement(calls);
        var outcome = await new CommunityDeliveryProcessor(
            new RecordingStub(calls),
            new RecordingStore(calls, CommunityReceiptOutcome.Recorded))
            .ProcessAsync(Encoding.UTF8.GetBytes(json.ToJsonString()), ReceivedAt,
                acknowledgement);
        Assert.Equal(CommunityDeliveryOutcome.InvalidContract, outcome);
        Assert.Empty(calls);
    }

    [Theory]
    [InlineData("communityParticipationId")]
    [InlineData("vendorId")]
    [InlineData("joinedAt")]
    [InlineData("contactPreference")]
    public async Task VR_COMMUNITY_021_InvalidPayloadIsRejected(string member)
    {
        JsonObject json = JsonNode.Parse(ValidEvent)!.AsObject();
        Assert.True(json["payload"]!.AsObject().Remove(member));
        var calls = new List<string>();
        var outcome = await new CommunityDeliveryProcessor(
            new RecordingStub(calls),
            new RecordingStore(calls, CommunityReceiptOutcome.Recorded))
            .ProcessAsync(Encoding.UTF8.GetBytes(json.ToJsonString()), ReceivedAt,
                new RecordingAcknowledgement(calls));
        Assert.Equal(CommunityDeliveryOutcome.InvalidContract, outcome);
        Assert.Empty(calls);
    }

    [Theory]
    [InlineData("fax")]
    [InlineData("Email")]
    public async Task VR_COMMUNITY_021_UnknownOrNonCanonicalPreferenceIsRejected(string preference)
    {
        JsonObject json = JsonNode.Parse(ValidEvent)!.AsObject();
        json["payload"]!.AsObject()["contactPreference"] = preference;
        var calls = new List<string>();
        var outcome = await new CommunityDeliveryProcessor(
            new RecordingStub(calls),
            new RecordingStore(calls, CommunityReceiptOutcome.Recorded))
            .ProcessAsync(Encoding.UTF8.GetBytes(json.ToJsonString()), ReceivedAt,
                new RecordingAcknowledgement(calls));
        Assert.Equal(CommunityDeliveryOutcome.InvalidContract, outcome);
        Assert.Empty(calls);
    }

    [Fact]
    public async Task VR_COMMUNITY_021_UnknownCompatibleMembersPreserveExactBytes()
    {
        JsonObject json = JsonNode.Parse(ValidEvent)!.AsObject();
        json["futureMember"] = true;
        byte[] bytes = Encoding.UTF8.GetBytes(json.ToJsonString());
        var calls = new List<string>();
        var store = new RecordingStore(calls, CommunityReceiptOutcome.Recorded);
        await new CommunityDeliveryProcessor(new RecordingStub(calls), store)
            .ProcessAsync(bytes, ReceivedAt, new RecordingAcknowledgement(calls));
        Assert.Equal(bytes, Assert.Single(store.Candidates).SerializedEvent.ToArray());
    }

    private sealed class RecordingStub : ICommunityParticipationRecordedProcessor
    {
        private readonly List<string> calls;
        public RecordingStub(List<string> calls) => this.calls = calls;
        public List<CommunityParticipationRecordedMessage> Messages { get; } = [];
        public Task ProcessAsync(CommunityParticipationRecordedMessage message,
            CancellationToken cancellationToken = default)
        {
            calls.Add("process");
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingStore : ICommunityReceiptStore
    {
        private readonly List<string> calls;
        private readonly CommunityReceiptOutcome outcome;
        public RecordingStore(List<string> calls, CommunityReceiptOutcome outcome)
        { this.calls = calls; this.outcome = outcome; }
        public List<CommunityReceiptCandidate> Candidates { get; } = [];
        public Task<CommunityReceiptOutcome> ClassifyAsync(
            CommunityReceiptCandidate candidate,
            CancellationToken cancellationToken = default)
        {
            calls.Add("persist");
            Candidates.Add(candidate);
            return Task.FromResult(outcome);
        }
    }

    private sealed class RecordingAcknowledgement : ICommunityDeliveryAcknowledgement
    {
        private readonly List<string> calls;
        public RecordingAcknowledgement(List<string> calls) => this.calls = calls;
        public int Count { get; private set; }
        public Task AcknowledgeAsync(CancellationToken cancellationToken = default)
        { calls.Add("acknowledge"); Count++; return Task.CompletedTask; }
    }
}
