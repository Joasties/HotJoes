using System.Text;
using HotJoes.Infrastructure.CommunityRelay;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(RabbitMqCollection.Name)]
public sealed class RabbitMqCommunityOutboxEventPublisherTests
{
    private const string TraceParent =
        "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
    private const string TraceState = "community=hotjoes";
    private static readonly Guid EventId = Guid.Parse(
        "2f0622f7-07a8-4026-b3aa-89d678789331");

    private readonly RabbitMqFixture fixture;

    public RabbitMqCommunityOutboxEventPublisherTests(
        RabbitMqFixture fixture) => this.fixture = fixture;

    [Fact]
    public async Task VR_COMMUNITY_019_ConfirmedPersistentCommunityMessageSurvivesRestart()
    {
        string suffix = Guid.NewGuid().ToString("N");
        string exchangeName = $"hotjoes.community.participation.{suffix}";
        string queueName = $"hotjoes.community.participation-recorded.{suffix}";
        string routingKey = $"community.participation.recorded.{suffix}";
        byte[] storedBytes = [0, 1, 2, 3, 254, 255];
        var options = new CommunityRabbitMqPublisherOptions(
            fixture.ConnectionString,
            exchangeName,
            ExchangeType.Direct,
            queueName,
            routingKey);

        await using (RabbitMqCommunityOutboxEventPublisher publisher =
            await RabbitMqCommunityOutboxEventPublisher.CreateAsync(options))
        {
            CommunityOutboxPublicationConfirmation confirmation =
                await publisher.PublishAsync(new CommunityOutboxPublication(
                    EventId,
                    eventVersion: 1,
                    storedBytes,
                    TraceParent,
                    TraceState));
            Assert.Equal(
                CommunityOutboxPublicationConfirmation.Confirmed,
                confirmation);
        }

        await fixture.RestartAsync();

        var factory = new ConnectionFactory
        {
            Uri = new Uri(fixture.ConnectionString),
            AutomaticRecoveryEnabled = false
        };
        await using IConnection connection =
            await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclarePassiveAsync(exchangeName);
        await channel.QueueDeclarePassiveAsync(queueName);
        BasicGetResult message = Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(queueName, autoAck: true));

        Assert.Equal(storedBytes, message.Body.ToArray());
        Assert.Equal(EventId.ToString("D"), message.BasicProperties.MessageId);
        Assert.Equal("application/json", message.BasicProperties.ContentType);
        Assert.True(message.BasicProperties.Persistent);
        Assert.Equal(
            TraceParent,
            Header(message.BasicProperties, "traceparent"));
        Assert.Equal(
            TraceState,
            Header(message.BasicProperties, "tracestate"));
        Assert.Null(await channel.BasicGetAsync(queueName, autoAck: true));
    }

    [Fact]
    public async Task VR_COMMUNITY_019_BrokerUnavailableNeverReturnsConfirmation()
    {
        string suffix = Guid.NewGuid().ToString("N");
        var options = new CommunityRabbitMqPublisherOptions(
            fixture.ConnectionString,
            $"hotjoes.community.participation.{suffix}",
            ExchangeType.Direct,
            $"hotjoes.community.participation-recorded.{suffix}",
            $"community.participation.recorded.{suffix}");
        await using RabbitMqCommunityOutboxEventPublisher publisher =
            await RabbitMqCommunityOutboxEventPublisher.CreateAsync(options);

        await fixture.StopAsync();
        try
        {
            using var timeout = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));
            await Assert.ThrowsAsync<CommunityOutboxPublicationException>(() =>
                publisher.PublishAsync(
                    new CommunityOutboxPublication(EventId, 1, [4, 5, 6]),
                    timeout.Token));
        }
        finally
        {
            await fixture.StartAsync();
        }
    }

    private static string Header(
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
