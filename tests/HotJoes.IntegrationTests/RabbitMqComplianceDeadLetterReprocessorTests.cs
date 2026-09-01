using System.Text;
using HotJoes.Infrastructure.ComplianceConsumer;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(RabbitMqCollection.Name)]
public sealed class RabbitMqComplianceDeadLetterReprocessorTests
{
    private static readonly Guid EventId = Guid.Parse(
        "2a45d171-787d-4666-b947-2bf63653552a");

    private readonly RabbitMqFixture _fixture;

    public RabbitMqComplianceDeadLetterReprocessorTests(
        RabbitMqFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RunOnceAsync_ExplicitInvocation_RepublishesOriginalMessageAndAcknowledgesDeadLetter()
    {
        ReprocessingTopology topology = CreateTopology();
        byte[] serializedEvent = CreateSerializedEvent();
        await SeedDeadLetterAsync(topology, serializedEvent);
        await DeclarePrimaryQueueAsync(topology);
        Assert.Equal(
            1u,
            await GetMessageCountAsync(
                topology.RecoveryOptions.DeadLetterQueueName));

        await using (RabbitMqComplianceDeadLetterReprocessor reprocessor =
            await RabbitMqComplianceDeadLetterReprocessor.CreateAsync(
                topology.RecoveryOptions))
        {
            Assert.Equal(
                ComplianceDeadLetterReprocessOutcome.Reprocessed,
                await reprocessor.RunOnceAsync());
            Assert.Equal(
                ComplianceDeadLetterReprocessOutcome.NoDelivery,
                await reprocessor.RunOnceAsync());
        }

        BasicGetResult primaryMessage = await GetRequiredMessageAsync(
            topology.PrimaryQueueName);
        AssertPreservedMessage(primaryMessage, serializedEvent);
        Assert.Null(await TryGetMessageAsync(
            topology.RecoveryOptions.DeadLetterQueueName));
    }

    [Fact]
    public async Task RunOnceAsync_PrimaryRepublicationFails_PreservesDeadLetterForLaterExplicitRetry()
    {
        ReprocessingTopology topology = CreateTopology();
        byte[] serializedEvent = CreateSerializedEvent();
        await SeedDeadLetterAsync(topology, serializedEvent);

        await using (RabbitMqComplianceDeadLetterReprocessor reprocessor =
            await RabbitMqComplianceDeadLetterReprocessor.CreateAsync(
                topology.RecoveryOptions))
        {
            await Assert.ThrowsAnyAsync<Exception>(() =>
                reprocessor.RunOnceAsync());
        }

        await DeclarePrimaryQueueAsync(topology);

        await using (RabbitMqComplianceDeadLetterReprocessor restarted =
            await RabbitMqComplianceDeadLetterReprocessor.CreateAsync(
                topology.RecoveryOptions))
        {
            Assert.Equal(
                ComplianceDeadLetterReprocessOutcome.Reprocessed,
                await restarted.RunOnceAsync());
        }

        BasicGetResult primaryMessage = await GetRequiredMessageAsync(
            topology.PrimaryQueueName);
        AssertPreservedMessage(primaryMessage, serializedEvent);
        Assert.Null(await TryGetMessageAsync(
            topology.RecoveryOptions.DeadLetterQueueName));
    }

    private ReprocessingTopology CreateTopology()
    {
        string suffix = Guid.NewGuid().ToString("N");
        string primaryExchange =
            $"hotjoes.vendor.registered.{suffix}";
        string primaryRoutingKey = $"vendor.registered.{suffix}";

        return new ReprocessingTopology(
            new RabbitMqRecoveryOptions(
                _fixture.ConnectionString,
                primaryExchange,
                primaryRoutingKey,
                $"hotjoes.compliance.retry.{suffix}",
                $"hotjoes.compliance.retry.{suffix}",
                $"compliance.retry.{suffix}",
                $"hotjoes.compliance.dead-letter.{suffix}",
                $"hotjoes.compliance.dead-letter.{suffix}",
                $"compliance.dead-letter.{suffix}",
                TimeSpan.FromSeconds(30)),
            $"hotjoes.compliance.vendor-registered.{suffix}");
    }

    private static async Task SeedDeadLetterAsync(
        ReprocessingTopology topology,
        byte[] serializedEvent)
    {
        await using RabbitMqComplianceRecoveryPublisher publisher =
            await RabbitMqComplianceRecoveryPublisher.CreateAsync(
                topology.RecoveryOptions);
        await publisher.PublishAsync(
            ComplianceRecoveryRoute.DeadLetter,
            new ComplianceRecoveryPublication(
                EventId,
                eventVersion: 1,
                serializedEvent,
                automaticAttempt: 3,
                failureCategory: "attemptsExhausted"));
    }

    private async Task DeclarePrimaryQueueAsync(
        ReprocessingTopology topology)
    {
        await using IConnection connection = await CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            topology.PrimaryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await channel.QueueBindAsync(
            topology.PrimaryQueueName,
            topology.RecoveryOptions.PrimaryExchangeName,
            topology.RecoveryOptions.PrimaryRoutingKey);
    }

    private async Task<BasicGetResult> GetRequiredMessageAsync(
        string queueName)
    {
        using var timeout = new CancellationTokenSource(
            TimeSpan.FromSeconds(10));

        while (true)
        {
            BasicGetResult? message = await TryGetMessageAsync(
                queueName,
                timeout.Token);

            if (message is not null)
            {
                return message;
            }

            await Task.Delay(50, timeout.Token);
        }
    }

    private async Task<BasicGetResult?> TryGetMessageAsync(
        string queueName,
        CancellationToken cancellationToken = default)
    {
        await using IConnection connection = await CreateConnectionAsync(
            cancellationToken);
        await using IChannel channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);
        return await channel.BasicGetAsync(
            queueName,
            autoAck: true,
            cancellationToken);
    }

    private async Task<uint> GetMessageCountAsync(string queueName)
    {
        await using IConnection connection = await CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        QueueDeclareOk queue = await channel.QueueDeclarePassiveAsync(
            queueName);
        return queue.MessageCount;
    }

    private async Task<IConnection> CreateConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_fixture.ConnectionString),
            AutomaticRecoveryEnabled = false
        };
        return await factory.CreateConnectionAsync(cancellationToken);
    }

    private static byte[] CreateSerializedEvent()
    {
        return Encoding.UTF8.GetBytes($$"""
            {
              "eventId": "{{EventId:D}}",
              "eventType": "VendorRegistered",
              "eventVersion": 1,
              "occurredAt": "2026-08-28T18:00:00.0000000Z",
              "payload": {
                "vendorId": "4e512746-8714-4e31-8b29-e8a262dd54b2"
              }
            }
            """);
    }

    private static void AssertPreservedMessage(
        BasicGetResult message,
        byte[] expectedBytes)
    {
        Assert.Equal(expectedBytes, message.Body.ToArray());
        Assert.Equal(
            EventId.ToString("D"),
            message.BasicProperties.MessageId);
        Assert.Equal(
            "application/json",
            message.BasicProperties.ContentType);
        Assert.True(message.BasicProperties.Persistent);
        Assert.NotNull(message.BasicProperties.Headers);
        Assert.Equal(
            1,
            Convert.ToInt32(
                message.BasicProperties.Headers[
                    "x-hotjoes-event-version"]));
        Assert.Equal(
            3,
            Convert.ToInt32(
                message.BasicProperties.Headers[
                    "x-hotjoes-automatic-attempt"]));
        Assert.Equal(
            "attemptsExhausted",
            Encoding.UTF8.GetString(
                Assert.IsType<byte[]>(
                    message.BasicProperties.Headers[
                        "x-hotjoes-failure-category"])));
    }

    private sealed record ReprocessingTopology(
        RabbitMqRecoveryOptions RecoveryOptions,
        string PrimaryQueueName);
}
