using System.Text;
using HotJoes.Infrastructure.CommunityConsumer;
using HotJoes.Infrastructure.CommunityRelay;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(RabbitMqCollection.Name)]
public sealed class RabbitMqCommunityConsumerTests
{
    private readonly RabbitMqFixture fixture;
    public RabbitMqCommunityConsumerTests(RabbitMqFixture fixture) =>
        this.fixture = fixture;

    [Fact]
    public async Task VR_COMMUNITY_022_EquivalentRedeliveryCreatesOneDurableReceipt()
    {
        string suffix = Guid.NewGuid().ToString("N");
        var options = new CommunityRabbitMqConsumerOptions(
            fixture.ConnectionString,
            $"hotjoes.community.participation.{suffix}",
            ExchangeType.Direct,
            $"hotjoes.community.consumer.{suffix}",
            $"community.participation.recorded.{suffix}");
        Guid eventId = Guid.NewGuid();
        byte[] bytes = Event(eventId);
        var store = new InMemoryReceiptStore();
        var processor = new CommunityDeliveryProcessor(
            new DeterministicCommunityParticipationRecordedStub(), store);

        await PublishAsync(options, eventId, bytes, bytes);
        await using var consumer = await RabbitMqCommunityConsumer.CreateAsync(
            options, processor);

        Assert.Equal(CommunityConsumerRunOutcome.AcknowledgedNewReceipt,
            await consumer.RunOnceAsync(DateTimeOffset.UtcNow));
        Assert.Equal(CommunityConsumerRunOutcome.AcknowledgedEquivalentDuplicate,
            await consumer.RunOnceAsync(DateTimeOffset.UtcNow));
        Assert.Equal(CommunityConsumerRunOutcome.NoDelivery,
            await consumer.RunOnceAsync(DateTimeOffset.UtcNow));
        Assert.Single(store.Receipts);
    }

    [Fact]
    public async Task VR_COMMUNITY_022_ProcessingFailureLeavesDeliveryAvailable()
    {
        string suffix = Guid.NewGuid().ToString("N");
        var options = new CommunityRabbitMqConsumerOptions(
            fixture.ConnectionString,
            $"hotjoes.community.participation.{suffix}",
            ExchangeType.Direct,
            $"hotjoes.community.consumer.{suffix}",
            $"community.participation.recorded.{suffix}");
        Guid eventId = Guid.NewGuid();
        byte[] bytes = Event(eventId);
        await PublishAsync(options, eventId, bytes);

        await using (var failing = await RabbitMqCommunityConsumer.CreateAsync(
            options, new CommunityDeliveryProcessor(new FailingStub(),
                new InMemoryReceiptStore())))
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                failing.RunOnceAsync(DateTimeOffset.UtcNow));
        }

        var store = new InMemoryReceiptStore();
        await using var restarted = await RabbitMqCommunityConsumer.CreateAsync(
            options, new CommunityDeliveryProcessor(
                new DeterministicCommunityParticipationRecordedStub(), store));
        Assert.Equal(CommunityConsumerRunOutcome.AcknowledgedNewReceipt,
            await restarted.RunOnceAsync(DateTimeOffset.UtcNow));
        Assert.Single(store.Receipts);
    }

    private static byte[] Event(Guid eventId) => Encoding.UTF8.GetBytes(
        """
        {"eventId":"__EVENT_ID__","eventType":"CommunityParticipationRecorded","eventVersion":1,"occurredAt":"2026-09-23T17:00:00.0000000Z","payload":{"communityParticipationId":"b590cd67-9bf2-46fd-b4aa-02f25f404275","vendorId":"7ecf7027-d8f8-4bfb-8559-c65896e52e48","joinedAt":"2026-09-23T17:00:00.0000000Z","contactPreference":"email"}}
        """.Replace(
            "__EVENT_ID__",
            eventId.ToString("D"),
            StringComparison.Ordinal));

    private static async Task PublishAsync(CommunityRabbitMqConsumerOptions options,
        Guid eventId, byte[] first, byte[]? second = null)
    {
        var publisherOptions = new CommunityRabbitMqPublisherOptions(
            options.ConnectionString, options.ExchangeName,
            options.ExchangeType, options.QueueName, options.RoutingKey);
        await using var publisher = await RabbitMqCommunityOutboxEventPublisher
            .CreateAsync(publisherOptions);
        await publisher.PublishAsync(new CommunityOutboxPublication(eventId, 1, first));
        if (second is not null)
            await publisher.PublishAsync(new CommunityOutboxPublication(eventId, 1, second));
    }

    private sealed class FailingStub : ICommunityParticipationRecordedProcessor
    {
        public Task ProcessAsync(CommunityParticipationRecordedMessage message,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Controlled processing failure.");
    }

    private sealed class InMemoryReceiptStore : ICommunityReceiptStore
    {
        public Dictionary<Guid, byte[]> Receipts { get; } = [];
        public Task<CommunityReceiptOutcome> ClassifyAsync(
            CommunityReceiptCandidate candidate,
            CancellationToken cancellationToken = default)
        {
            if (!Receipts.TryGetValue(candidate.EventId, out byte[]? bytes))
            {
                Receipts[candidate.EventId] = candidate.SerializedEvent.ToArray();
                return Task.FromResult(CommunityReceiptOutcome.Recorded);
            }
            return Task.FromResult(bytes.SequenceEqual(candidate.SerializedEvent.ToArray())
                ? CommunityReceiptOutcome.EquivalentDuplicate
                : CommunityReceiptOutcome.ConflictingBytes);
        }
    }
}
