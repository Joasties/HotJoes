using System.Text;
using HotJoes.Infrastructure.VendorRelay;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(RabbitMqCollection.Name)]
public sealed class RabbitMqOutboxEventPublisherTests
{
    private const string TraceParent =
        "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
    private const string TraceState = "vendor=hotjoes";

    private static readonly Guid EventId = Guid.Parse(
        "72cf4e9f-536b-4611-b0c1-209d7fa92ee7");

    private readonly RabbitMqFixture _fixture;

    public RabbitMqOutboxEventPublisherTests(RabbitMqFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PublishAsync_ConfirmedPersistentMessageAndDurableTopologySurviveBrokerRestart()
    {
        string suffix = Guid.NewGuid().ToString("N");
        string exchangeName = $"hotjoes.vendor.registered.{suffix}";
        string queueName = $"hotjoes.compliance.vendor-registered.{suffix}";
        string routingKey = $"vendor.registered.{suffix}";
        byte[] serializedEvent = [0, 1, 2, 3, 254, 255];
        var options = new RabbitMqPublisherOptions(
            _fixture.ConnectionString,
            exchangeName,
            ExchangeType.Direct,
            queueName,
            routingKey);

        await using (RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(options))
        {
            OutboxPublicationConfirmation confirmation =
                await publisher.PublishAsync(
                    new OutboxPublication(EventId, 1, serializedEvent));

            Assert.Equal(
                OutboxPublicationConfirmation.Confirmed,
                confirmation);
        }

        await _fixture.RestartAsync();

        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(_fixture.ConnectionString),
            AutomaticRecoveryEnabled = false
        };
        await using IConnection connection =
            await connectionFactory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclarePassiveAsync(exchangeName);
        await channel.QueueDeclarePassiveAsync(queueName);
        BasicGetResult message = Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(queueName, autoAck: true));

        Assert.Equal(serializedEvent, message.Body.ToArray());
        Assert.Equal(EventId.ToString("D"), message.BasicProperties.MessageId);
        Assert.Equal("application/json", message.BasicProperties.ContentType);
        Assert.True(message.BasicProperties.Persistent);
        Assert.Null(await channel.BasicGetAsync(queueName, autoAck: true));
    }

    [Fact]
    public async Task PublishAsync_BrokerUnavailable_DoesNotReturnPositiveConfirmation()
    {
        string suffix = Guid.NewGuid().ToString("N");
        var options = new RabbitMqPublisherOptions(
            _fixture.ConnectionString,
            $"hotjoes.vendor.registered.{suffix}",
            ExchangeType.Direct,
            $"hotjoes.compliance.vendor-registered.{suffix}",
            $"vendor.registered.{suffix}");
        await using RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(options);

        await _fixture.StopAsync();

        try
        {
            using var timeout = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            await Assert.ThrowsAsync<OutboxPublicationException>(() =>
                publisher.PublishAsync(
                    new OutboxPublication(EventId, 1, [4, 5, 6]),
                    timeout.Token));
        }
        finally
        {
            await _fixture.StartAsync();
        }
    }

    [Fact]
    public async Task PublishAsync_PresentAndAbsentTraceMetadata_UsesHeadersWithoutChangingBodies()
    {
        string suffix = Guid.NewGuid().ToString("N");
        string queueName = $"hotjoes.compliance.vendor-registered.{suffix}";
        var options = new RabbitMqPublisherOptions(
            _fixture.ConnectionString,
            $"hotjoes.vendor.registered.{suffix}",
            ExchangeType.Direct,
            queueName,
            $"vendor.registered.{suffix}");
        byte[] withContextBytes = [10, 20, 30, 40];
        byte[] withoutContextBytes = [50, 60, 70, 80];

        await using (RabbitMqOutboxEventPublisher publisher =
            await RabbitMqOutboxEventPublisher.CreateAsync(options))
        {
            await publisher.PublishAsync(new OutboxPublication(
                Guid.NewGuid(),
                1,
                withContextBytes,
                TraceParent,
                TraceState));
            await publisher.PublishAsync(new OutboxPublication(
                Guid.NewGuid(),
                1,
                withoutContextBytes,
                traceParent: null,
                traceState: null));
        }

        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(_fixture.ConnectionString),
            AutomaticRecoveryEnabled = false
        };
        await using IConnection connection =
            await connectionFactory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        BasicGetResult withContext = Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(queueName, autoAck: true));
        BasicGetResult withoutContext = Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(queueName, autoAck: true));

        Assert.Equal(withContextBytes, withContext.Body.ToArray());
        Assert.Equal(
            TraceParent,
            GetHeader(withContext.BasicProperties, "traceparent"));
        Assert.Equal(
            TraceState,
            GetHeader(withContext.BasicProperties, "tracestate"));

        Assert.Equal(withoutContextBytes, withoutContext.Body.ToArray());
        Assert.False(
            withoutContext.BasicProperties.Headers?.ContainsKey(
                "traceparent") ?? false);
        Assert.False(
            withoutContext.BasicProperties.Headers?.ContainsKey(
                "tracestate") ?? false);
    }

    private static string GetHeader(
        IReadOnlyBasicProperties properties,
        string name)
    {
        Assert.NotNull(properties.Headers);
        Assert.True(properties.Headers.TryGetValue(name, out object? value));

        return value switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            ReadOnlyMemory<byte> bytes => Encoding.UTF8.GetString(bytes.Span),
            string text => text,
            _ => throw new Xunit.Sdk.XunitException(
                $"Unexpected RabbitMQ header representation for '{name}'.")
        };
    }
}
