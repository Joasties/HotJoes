namespace HotJoes.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ReliablePublicationCollection
    : ICollectionFixture<PostgreSqlFixture>,
      ICollectionFixture<RabbitMqFixture>
{
    public const string Name = "PostgreSQL and RabbitMQ";
}
