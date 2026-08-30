namespace HotJoes.IntegrationTests;

[CollectionDefinition(
    Name,
    DisableParallelization = true)]
public sealed class TracePropagationIntegrationCollection
    : ICollectionFixture<PostgreSqlFixture>,
      ICollectionFixture<CompliancePostgreSqlFixture>,
      ICollectionFixture<RabbitMqFixture>
{
    public const string Name = "Trace propagation integration";
}
