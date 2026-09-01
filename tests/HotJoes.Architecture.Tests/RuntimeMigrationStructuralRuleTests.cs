namespace HotJoes.Architecture.Tests;

public sealed class RuntimeMigrationStructuralRuleTests
{
    [Fact]
    public void AI_MIG_002_ApprovedProductionSources_HaveNoViolations()
    {
        ArchitectureSourceCatalog catalog =
            ArchitectureSourceCatalog.LoadProductionSources();

        IReadOnlyList<ArchitectureViolation> violations =
            MigrationStructuralRuleSet.Evaluate(catalog);

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData("database.EnsureCreated();")]
    [InlineData("await database.EnsureCreatedAsync();")]
    [InlineData("database.Migrate();")]
    [InlineData("await database.MigrateAsync();")]
    [InlineData("context.GetService<IMigrator>().Migrate();")]
    [InlineData("services.GetRequiredService<IMigrator>().Migrate();")]
    public void AI_MIG_002_RuntimeMigrationMechanism_IsDetected(
        string prohibitedSource)
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Api.Vendor/Program.cs",
                prohibitedSource));

        AssertViolation(MigrationStructuralRuleSet.Evaluate(catalog));
    }

    [Theory]
    [InlineData("CREATE TABLE runtime_schema(id integer);")]
    [InlineData("ALTER TABLE runtime_schema ADD COLUMN value text;")]
    [InlineData("DROP TABLE runtime_schema;")]
    [InlineData("CREATE INDEX ix_runtime ON runtime_schema(id);")]
    [InlineData("DROP SCHEMA public CASCADE;")]
    public void AI_MIG_002_AdHocRuntimeSchemaSql_IsDetected(
        string prohibitedSql)
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                "src/HotJoes.Infrastructure.VendorRelay/Worker.cs",
                $$"""
                context.Database.ExecuteSqlRaw("{{prohibitedSql}}");
                """));

        AssertViolation(MigrationStructuralRuleSet.Evaluate(catalog));
    }

    [Theory]
    [InlineData(
        "src/HotJoes.Infrastructure.Persistence/Migrations/Example.cs")]
    [InlineData(
        "src/HotJoes.Infrastructure.Persistence/" +
        "VendorRegistrationDesignTimeDbContextFactory.cs")]
    [InlineData(
        "src/HotJoes.Infrastructure.ComplianceConsumer/Migrations/Example.cs")]
    [InlineData(
        "src/HotJoes.Infrastructure.ComplianceConsumer/" +
        "ComplianceReceiptDesignTimeDbContextFactory.cs")]
    public void AI_MIG_002_ApprovedMigrationBoundary_IsExcluded(
        string approvedPath)
    {
        ArchitectureSourceCatalog catalog = Catalog(
            File(
                approvedPath,
                "database.Migrate(); CREATE TABLE approved(id integer);"));

        Assert.Empty(MigrationStructuralRuleSet.Evaluate(catalog));
    }

    private static ArchitectureSourceCatalog Catalog(
        params SourceFileDescriptor[] files)
    {
        return ArchitectureSourceCatalog.FromFiles(files);
    }

    private static SourceFileDescriptor File(string path, string content)
    {
        return new SourceFileDescriptor(path, content);
    }

    private static void AssertViolation(
        IReadOnlyList<ArchitectureViolation> violations)
    {
        Assert.Contains(
            violations,
            violation => violation.ObligationId == "AI-MIG-002");
    }
}
