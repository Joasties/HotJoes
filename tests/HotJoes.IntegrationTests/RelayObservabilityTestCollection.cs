namespace HotJoes.IntegrationTests;

[CollectionDefinition(
    Name,
    DisableParallelization = true)]
public sealed class RelayObservabilityTestCollection
{
    public const string Name = "Relay observability";
}
