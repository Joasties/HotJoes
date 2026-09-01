namespace HotJoes.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class MigrationPostgreSqlCollection
    : ICollectionFixture<MigrationPostgreSqlFixture>
{
    public const string Name = "Migration PostgreSQL";
}
