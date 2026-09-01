namespace HotJoes.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class CompliancePostgreSqlCollection
    : ICollectionFixture<CompliancePostgreSqlFixture>
{
    public const string Name = "Compliance PostgreSQL";
}
