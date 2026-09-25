using HotJoes.Infrastructure.CommunityConsumer;
using RabbitMQ.Client;

namespace HotJoes.IntegrationTests;

[Collection(RabbitMqCollection.Name)]
public sealed class RabbitMqCommunityRecoveryPublisherTests
{
    private readonly RabbitMqFixture fixture;
    public RabbitMqCommunityRecoveryPublisherTests(RabbitMqFixture fixture) =>
        this.fixture = fixture;

    [Theory]
    [InlineData(CommunityRecoveryRoute.Retry)]
    [InlineData(CommunityRecoveryRoute.DeadLetter)]
    public async Task AI_COMMUNITY_003_RecoveryRoutePreservesImmutableBytesAndMetadata(
        CommunityRecoveryRoute route)
    {
        string suffix = Guid.NewGuid().ToString("N");
        var options = new CommunityRabbitMqRecoveryOptions(
            fixture.ConnectionString,
            $"hotjoes.community.primary.{suffix}",
            $"community.primary.{suffix}",
            $"hotjoes.community.retry.{suffix}",
            $"hotjoes.community.retry.queue.{suffix}",
            $"community.retry.{suffix}",
            $"hotjoes.community.deadletter.{suffix}",
            $"hotjoes.community.deadletter.queue.{suffix}",
            $"community.deadletter.{suffix}",
            TimeSpan.FromSeconds(30));
        Guid eventId = Guid.NewGuid();
        byte[] bytes = [0, 1, 2, 3, 254, 255];
        var publication = new CommunityRecoveryPublication(eventId, 1, bytes,
            automaticAttempt: 2, failureCategory: "processingUnavailable");

        await using (var publisher =
            await RabbitMqCommunityRecoveryPublisher.CreateAsync(options))
            await publisher.PublishAsync(route, publication);

        string queue = route == CommunityRecoveryRoute.Retry
            ? options.RetryQueueName : options.DeadLetterQueueName;
        var factory = new ConnectionFactory
        { Uri = new Uri(fixture.ConnectionString), AutomaticRecoveryEnabled = false };
        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        BasicGetResult delivery = Assert.IsType<BasicGetResult>(
            await channel.BasicGetAsync(queue, autoAck: true));
        Assert.Equal(bytes, delivery.Body.ToArray());
        Assert.Equal(eventId.ToString("D"), delivery.BasicProperties.MessageId);
        Assert.True(delivery.BasicProperties.Persistent);
        Assert.NotNull(delivery.BasicProperties.Headers);
        Assert.Equal(2L, Convert.ToInt64(
            delivery.BasicProperties.Headers["x-hotjoes-automatic-attempt"]));
        Assert.Equal(1L, Convert.ToInt64(
            delivery.BasicProperties.Headers["x-hotjoes-event-version"]));
    }
}
