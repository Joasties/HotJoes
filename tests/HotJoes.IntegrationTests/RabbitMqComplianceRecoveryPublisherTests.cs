using System.Text;
using HotJoes.Infrastructure.ComplianceConsumer;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(RabbitMqCollection.Name)]
public sealed class RabbitMqComplianceRecoveryPublisherTests
{
    private static readonly Guid RetryEventId = Guid.Parse(
        "6e39c58b-f7c0-4e43-99fd-1cb8495806a8");

    private static readonly Guid DeadLetterEventId = Guid.Parse(
        "c77288f6-d36b-47aa-a308-d3c1a93a2bd4");

    private readonly RabbitMqFixture _fixture;

    public RabbitMqComplianceRecoveryPublisherTests(RabbitMqFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PublishAsync_RetryAndDeadLetter_AreDurablePersistentAndPreserveExactMessage()
    {
        RabbitMqRecoveryOptions options = CreateOptions();
        byte[] retryBytes = CreateBytes(RetryEventId);
        byte[] deadLetterBytes = CreateBytes(DeadLetterEventId);

        await using (RabbitMqComplianceRecoveryPublisher publisher =
            await RabbitMqComplianceRecoveryPublisher.CreateAsync(options))
        {
            await publisher.PublishAsync(
                ComplianceRecoveryRoute.Retry,
                new ComplianceRecoveryPublication(
                    RetryEventId,
                    eventVersion: 1,
                    retryBytes,
                    automaticAttempt: 2,
                    failureCategory: "receiptUnavailable"));
            await publisher.PublishAsync(
                ComplianceRecoveryRoute.DeadLetter,
                new ComplianceRecoveryPublication(
                    DeadLetterEventId,
                    eventVersion: 1,
                    deadLetterBytes,
                    automaticAttempt: 3,
                    failureCategory: "attemptsExhausted"));
        }

        await _fixture.RestartAsync();

        await using IConnection connection = await CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclarePassiveAsync(options.RetryExchangeName);
        await channel.ExchangeDeclarePassiveAsync(
            options.DeadLetterExchangeName);
        await channel.QueueDeclarePassiveAsync(options.RetryQueueName);
        await channel.QueueDeclarePassiveAsync(options.DeadLetterQueueName);

        BasicGetResult retry = Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(
                options.RetryQueueName,
                autoAck: true));
        BasicGetResult deadLetter = Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(
                options.DeadLetterQueueName,
                autoAck: true));

        AssertMessage(
            retry,
            RetryEventId,
            retryBytes,
            automaticAttempt: 2,
            failureCategory: "receiptUnavailable");
        AssertMessage(
            deadLetter,
            DeadLetterEventId,
            deadLetterBytes,
            automaticAttempt: 3,
            failureCategory: "attemptsExhausted");
    }

    [Fact]
    public async Task PublishAsync_RetryDelay_ReturnsExactMessageToPrimaryRoute()
    {
        RabbitMqRecoveryOptions options = CreateOptions(
            TimeSpan.FromMilliseconds(250));
        string primaryQueueName = $"primary-observation-{Guid.NewGuid():N}";
        byte[] serializedEvent = CreateBytes(RetryEventId);

        await using IConnection connection = await CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await using (RabbitMqComplianceRecoveryPublisher publisher =
            await RabbitMqComplianceRecoveryPublisher.CreateAsync(options))
        {
            await channel.QueueDeclareAsync(
                primaryQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);
            await channel.QueueBindAsync(
                primaryQueueName,
                options.PrimaryExchangeName,
                options.PrimaryRoutingKey);

            await publisher.PublishAsync(
                ComplianceRecoveryRoute.Retry,
                new ComplianceRecoveryPublication(
                    RetryEventId,
                    eventVersion: 1,
                    serializedEvent,
                    automaticAttempt: 1,
                    failureCategory: "receiptUnavailable"));
        }

        BasicGetResult message = await WaitForMessageAsync(
            channel,
            primaryQueueName);

        AssertMessage(
            message,
            RetryEventId,
            serializedEvent,
            automaticAttempt: 1,
            failureCategory: "receiptUnavailable");
    }

    private RabbitMqRecoveryOptions CreateOptions(
        TimeSpan? retryDelay = null)
    {
        string suffix = Guid.NewGuid().ToString("N");
        return new RabbitMqRecoveryOptions(
            _fixture.ConnectionString,
            primaryExchangeName: $"hotjoes.vendor.registered.{suffix}",
            primaryRoutingKey: $"vendor.registered.{suffix}",
            retryExchangeName: $"hotjoes.compliance.retry.{suffix}",
            retryQueueName: $"hotjoes.compliance.retry.{suffix}",
            retryRoutingKey: $"compliance.retry.{suffix}",
            deadLetterExchangeName: $"hotjoes.compliance.dead-letter.{suffix}",
            deadLetterQueueName: $"hotjoes.compliance.dead-letter.{suffix}",
            deadLetterRoutingKey: $"compliance.dead-letter.{suffix}",
            retryDelay: retryDelay ?? TimeSpan.FromMinutes(5));
    }

    private async Task<IConnection> CreateConnectionAsync()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_fixture.ConnectionString),
            AutomaticRecoveryEnabled = false
        };
        return await factory.CreateConnectionAsync();
    }

    private static byte[] CreateBytes(Guid eventId)
    {
        return Encoding.UTF8.GetBytes($$"""
            {"eventId":"{{eventId:D}}","eventType":"VendorRegistered","eventVersion":1}
            """);
    }

    private static async Task<BasicGetResult> WaitForMessageAsync(
        IChannel channel,
        string queueName)
    {
        using var timeout = new CancellationTokenSource(
            TimeSpan.FromSeconds(10));

        while (true)
        {
            BasicGetResult? message = await channel.BasicGetAsync(
                queueName,
                autoAck: true,
                timeout.Token);

            if (message is not null)
            {
                return message;
            }

            await Task.Delay(
                TimeSpan.FromMilliseconds(50),
                timeout.Token);
        }
    }

    private static void AssertMessage(
        BasicGetResult message,
        Guid expectedEventId,
        byte[] expectedBytes,
        int automaticAttempt,
        string failureCategory)
    {
        Assert.Equal(expectedBytes, message.Body.ToArray());
        Assert.Equal(
            expectedEventId.ToString("D"),
            message.BasicProperties.MessageId);
        Assert.Equal(
            "application/json",
            message.BasicProperties.ContentType);
        Assert.True(message.BasicProperties.Persistent);
        Assert.NotNull(message.BasicProperties.Headers);
        Assert.Equal(
            automaticAttempt,
            Convert.ToInt32(
                message.BasicProperties.Headers[
                    "x-hotjoes-automatic-attempt"]));
        Assert.Equal(
            failureCategory,
            Encoding.UTF8.GetString(
                Assert.IsType<byte[]>(
                    message.BasicProperties.Headers[
                        "x-hotjoes-failure-category"])));
        Assert.Equal(
            1,
            Convert.ToInt32(
                message.BasicProperties.Headers[
                    "x-hotjoes-event-version"]));
    }
}
