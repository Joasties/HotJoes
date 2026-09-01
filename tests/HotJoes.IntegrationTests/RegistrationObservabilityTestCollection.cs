namespace HotJoes.IntegrationTests;

[CollectionDefinition(
    Name,
    DisableParallelization = true)]
public sealed class RegistrationObservabilityTestCollection
    : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "Registration observability";
}
