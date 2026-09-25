using RabbitMQ.Client;
namespace HotJoes.Infrastructure.CommunityConsumer;
public sealed class RabbitMqCommunityConsumer : IAsyncDisposable
{
    private readonly CommunityRabbitMqConsumerOptions options;
    private readonly CommunityDeliveryProcessor processor;
    private readonly IConnection connection;
    private readonly IChannel channel;
    private readonly CommunityDeliveryRecoveryHandler? recovery;
    private RabbitMqCommunityConsumer(CommunityRabbitMqConsumerOptions options,
        CommunityDeliveryProcessor processor, IConnection connection, IChannel channel,
        CommunityDeliveryRecoveryHandler? recovery)
    { this.options = options; this.processor = processor; this.connection = connection; this.channel = channel; this.recovery = recovery; }

    public static async Task<RabbitMqCommunityConsumer> CreateAsync(
        CommunityRabbitMqConsumerOptions options, CommunityDeliveryProcessor processor,
        CommunityDeliveryRecoveryHandler? recovery = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options); ArgumentNullException.ThrowIfNull(processor);
        IConnection? connection = null; IChannel? channel = null;
        try
        {
            var factory = new ConnectionFactory { Uri = options.ConnectionUri, AutomaticRecoveryEnabled = false };
            connection = await factory.CreateConnectionAsync(cancellationToken);
            channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await channel.ExchangeDeclareAsync(options.ExchangeName, options.ExchangeType, true, false, cancellationToken: cancellationToken);
            await channel.QueueDeclareAsync(options.QueueName, true, false, false, cancellationToken: cancellationToken);
            await channel.QueueBindAsync(options.QueueName, options.ExchangeName, options.RoutingKey, cancellationToken: cancellationToken);
            await channel.BasicQosAsync(0, 1, false, cancellationToken);
            return new(options, processor, connection, channel, recovery);
        }
        catch { if (channel is not null) await channel.DisposeAsync(); if (connection is not null) await connection.DisposeAsync(); throw; }
    }

    public async Task<CommunityConsumerRunOutcome> RunOnceAsync(DateTimeOffset receivedAtUtc,
        CancellationToken cancellationToken = default)
    {
        BasicGetResult? delivery = await channel.BasicGetAsync(options.QueueName, false, cancellationToken);
        if (delivery is null) return CommunityConsumerRunOutcome.NoDelivery;
        var acknowledgement = new Acknowledgement(channel, delivery.DeliveryTag);
        CommunityDeliveryOutcome outcome;
        try
        {
            outcome = await processor.ProcessAsync(delivery.Body,
                receivedAtUtc, acknowledgement, cancellationToken);
        }
        catch when (recovery is not null)
        {
            await Recover(delivery, acknowledgement, "processingUnavailable", true,
                cancellationToken);
            return CommunityConsumerRunOutcome.NoDelivery;
        }
        if (recovery is not null && outcome is CommunityDeliveryOutcome.InvalidContract or CommunityDeliveryOutcome.ConflictingBytes)
        {
            await Recover(delivery, acknowledgement,
                outcome == CommunityDeliveryOutcome.InvalidContract ? "invalidContract" : "conflictingBytes",
                false, cancellationToken);
            return CommunityConsumerRunOutcome.NoDelivery;
        }
        return outcome switch
        {
            CommunityDeliveryOutcome.AcknowledgedNewReceipt => CommunityConsumerRunOutcome.AcknowledgedNewReceipt,
            CommunityDeliveryOutcome.AcknowledgedEquivalentDuplicate => CommunityConsumerRunOutcome.AcknowledgedEquivalentDuplicate,
            CommunityDeliveryOutcome.InvalidContract => CommunityConsumerRunOutcome.InvalidContract,
            CommunityDeliveryOutcome.ConflictingBytes => CommunityConsumerRunOutcome.ConflictingBytes,
            _ => throw new InvalidOperationException("Unsupported Community delivery outcome.")
        };
    }

    public async ValueTask DisposeAsync() { await channel.DisposeAsync(); await connection.DisposeAsync(); }

    private Task<CommunityRecoveryRoute> Recover(BasicGetResult delivery,
        ICommunityDeliveryAcknowledgement acknowledgement, string category,
        bool retryable, CancellationToken cancellationToken)
    {
        Guid eventId = Guid.TryParseExact(delivery.BasicProperties.MessageId,
            "D", out Guid parsed) ? parsed : Guid.NewGuid();
        int attempt = 1;
        if (delivery.BasicProperties.Headers?.TryGetValue(
            "x-hotjoes-automatic-attempt", out object? value) == true)
            attempt = Convert.ToInt32(value);
        return recovery!.RecoverAsync(eventId, 1, delivery.Body, attempt,
            category, retryable, acknowledgement, cancellationToken);
    }

    private sealed class Acknowledgement : ICommunityDeliveryAcknowledgement
    {
        private readonly IChannel channel; private readonly ulong tag;
        public Acknowledgement(IChannel channel, ulong tag) { this.channel = channel; this.tag = tag; }
        public Task AcknowledgeAsync(CancellationToken cancellationToken = default) =>
            channel.BasicAckAsync(tag, false, cancellationToken).AsTask();
    }
}
