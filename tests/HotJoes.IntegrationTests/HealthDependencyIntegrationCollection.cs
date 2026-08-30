namespace HotJoes.IntegrationTests;

[CollectionDefinition(
    Name,
    DisableParallelization = true)]
public sealed class HealthDependencyIntegrationCollection
    : ICollectionFixture<HealthDependencyFixture>
{
    public const string Name = "Health dependency integration";
}
