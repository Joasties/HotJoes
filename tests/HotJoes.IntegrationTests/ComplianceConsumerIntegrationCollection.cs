namespace HotJoes.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ComplianceConsumerIntegrationCollection
    : ICollectionFixture<RabbitMqFixture>,
      ICollectionFixture<CompliancePostgreSqlFixture>
{
    public const string Name = "Compliance consumer integration";
}
