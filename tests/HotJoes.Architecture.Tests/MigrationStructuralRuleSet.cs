using System.Text.RegularExpressions;

namespace HotJoes.Architecture.Tests;

public static class MigrationStructuralRuleSet
{
    private static readonly string[] ApprovedDesignTimePaths =
    [
        "src/HotJoes.Infrastructure.Persistence/" +
        "VendorRegistrationDesignTimeDbContextFactory.cs",
        "src/HotJoes.Infrastructure.ComplianceConsumer/" +
        "ComplianceReceiptDesignTimeDbContextFactory.cs"
    ];

    private static readonly Regex AutomaticDatabaseOperation = new(
        @"\.\s*(?:EnsureCreated|Migrate)(?:Async)?\s*\(",
        RegexOptions.CultureInvariant);

    private static readonly Regex RuntimeMigrator = new(
        @"\bIMigrator\b",
        RegexOptions.CultureInvariant);

    private static readonly Regex SchemaDdl = new(
        @"\b(?:CREATE|ALTER|DROP)\s+" +
        @"(?:TABLE|SCHEMA|INDEX|CONSTRAINT)\b",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ArchitectureSourceCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var violations = new List<ArchitectureViolation>();

        foreach (SourceFileDescriptor file in catalog.Files.Where(
            file => !IsApprovedMigrationBoundary(file.RelativePath)))
        {
            AddIfMatched(
                file,
                AutomaticDatabaseOperation,
                "Production source invokes automatic database creation or " +
                "migration.",
                violations);
            AddIfMatched(
                file,
                RuntimeMigrator,
                "Production source accesses the EF migration service.",
                violations);
            AddIfMatched(
                file,
                SchemaDdl,
                "Production source contains ad hoc schema DDL.",
                violations);
        }

        return violations
            .OrderBy(violation => violation.ProjectName, StringComparer.Ordinal)
            .ThenBy(
                violation => violation.Description,
                StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsApprovedMigrationBoundary(string relativePath)
    {
        return relativePath.Contains(
                "/Migrations/",
                StringComparison.Ordinal) ||
            ApprovedDesignTimePaths.Contains(
                relativePath,
                StringComparer.Ordinal);
    }

    private static void AddIfMatched(
        SourceFileDescriptor file,
        Regex pattern,
        string description,
        ICollection<ArchitectureViolation> violations)
    {
        if (!pattern.IsMatch(file.Content))
        {
            return;
        }

        violations.Add(new ArchitectureViolation(
            "AI-MIG-002",
            file.RelativePath,
            description));
    }
}
