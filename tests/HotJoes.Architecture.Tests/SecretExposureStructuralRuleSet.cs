using System.Text.RegularExpressions;

namespace HotJoes.Architecture.Tests;

public static class SecretExposureStructuralRuleSet
{
    private const string ObligationId = "AI-SEC-001";

    private static readonly Regex JsonStringProperty = new(
        "\\\"(?<name>[^\\\"]+)\\\"\\s*:\\s*" +
            "\\\"(?<value>[^\\\"]*)\\\"",
        RegexOptions.CultureInvariant);

    private static readonly Regex Identifier = new(
        "\\b[A-Za-z_][A-Za-z0-9_]*\\b",
        RegexOptions.CultureInvariant);

    private static readonly Regex LoggingCall = new(
        "\\b(?:logger|_logger)\\s*\\.\\s*Log" +
            "(?:Trace|Debug|Information|Warning|Error|Critical)" +
            "\\s*\\((?<arguments>[\\s\\S]*?)\\)\\s*;",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex CredentialBearingUri = new(
        "\\b[a-z][a-z0-9+.-]*://" +
            "[^\\s\\\"'@/:]+:[^\\s\\\"'@]+@",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly string[] SensitiveIdentifierFragments =
    [
        "Password",
        "ClientSecret",
        "ConnectionString",
        "PrivateKey",
        "AccessToken",
        "Credential"
    ];

    private static readonly string[] PermittedReferenceSuffixes =
    [
        "SecretName",
        "SecretReference",
        "KeyVaultUri"
    ];

    public static IReadOnlyList<ArchitectureViolation> EvaluateCurrent()
    {
        return Evaluate(ArchitectureSourceCatalog.LoadProductionSources());
    }

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ArchitectureSourceCatalog sources)
    {
        ArgumentNullException.ThrowIfNull(sources);

        var violations = new List<ArchitectureViolation>();

        foreach (SourceFileDescriptor file in sources.Files)
        {
            AddConfigurationDocumentViolations(file, violations);
            AddConfigurationSnapshotViolations(file, violations);
            AddApiContractViolations(file, violations);
            AddLoggingViolations(file, violations);
            AddCredentialUriViolations(file, violations);
        }

        return violations
            .OrderBy(violation => violation.ProjectName, StringComparer.Ordinal)
            .ThenBy(
                violation => violation.Description,
                StringComparer.Ordinal)
            .ToArray();
    }

    private static void AddConfigurationDocumentViolations(
        SourceFileDescriptor file,
        ICollection<ArchitectureViolation> violations)
    {
        if (!file.RelativePath.EndsWith(
                ".json",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        foreach (Match property in JsonStringProperty.Matches(file.Content))
        {
            string name = property.Groups["name"].Value;
            string value = property.Groups["value"].Value;

            if (value.Length > 0 && IsSecretValueIdentifier(name))
            {
                Add(
                    file,
                    violations,
                    "Configuration document contains a secret-bearing " +
                        $"value for key '{name}'.");
            }
        }
    }

    private static void AddConfigurationSnapshotViolations(
        SourceFileDescriptor file,
        ICollection<ArchitectureViolation> violations)
    {
        if (!file.RelativePath.EndsWith(
                "ConfigurationSnapshot.cs",
                StringComparison.Ordinal))
        {
            return;
        }

        AddSensitiveIdentifierViolations(
            file,
            violations,
            "Configuration snapshot owns secret-bearing member");
    }

    private static void AddApiContractViolations(
        SourceFileDescriptor file,
        ICollection<ArchitectureViolation> violations)
    {
        if (!file.RelativePath.Contains(
                "/Contracts/",
                StringComparison.Ordinal) ||
            !file.RelativePath.EndsWith(
                ".cs",
                StringComparison.Ordinal))
        {
            return;
        }

        AddSensitiveIdentifierViolations(
            file,
            violations,
            "API contract exposes secret-bearing member");
    }

    private static void AddLoggingViolations(
        SourceFileDescriptor file,
        ICollection<ArchitectureViolation> violations)
    {
        foreach (Match loggingCall in LoggingCall.Matches(file.Content))
        {
            if (Identifiers(loggingCall.Groups["arguments"].Value)
                .Any(IsSecretValueIdentifier))
            {
                Add(
                    file,
                    violations,
                    "Logging call contains a secret-bearing value.");
            }
        }
    }

    private static void AddCredentialUriViolations(
        SourceFileDescriptor file,
        ICollection<ArchitectureViolation> violations)
    {
        if (CredentialBearingUri.IsMatch(file.Content))
        {
            Add(
                file,
                violations,
                "Source contains a credential-bearing URI; the detected " +
                    "value has been withheld.");
        }
    }

    private static void AddSensitiveIdentifierViolations(
        SourceFileDescriptor file,
        ICollection<ArchitectureViolation> violations,
        string description)
    {
        foreach (string identifier in Identifiers(file.Content)
            .Where(IsSecretValueIdentifier)
            .Distinct(StringComparer.Ordinal))
        {
            Add(
                file,
                violations,
                $"{description} '{identifier}'.");
        }
    }

    private static IEnumerable<string> Identifiers(string content)
    {
        return Identifier.Matches(content).Select(match => match.Value);
    }

    private static bool IsSecretValueIdentifier(string identifier)
    {
        if (PermittedReferenceSuffixes.Any(suffix =>
            identifier.EndsWith(
                suffix,
                StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        return SensitiveIdentifierFragments.Any(fragment =>
            identifier.Contains(
                fragment,
                StringComparison.OrdinalIgnoreCase));
    }

    private static void Add(
        SourceFileDescriptor file,
        ICollection<ArchitectureViolation> violations,
        string description)
    {
        violations.Add(new ArchitectureViolation(
            ObligationId,
            ProjectName(file.RelativePath),
            $"Source '{file.RelativePath}': {description}"));
    }

    private static string ProjectName(string relativePath)
    {
        string[] segments = relativePath.Split('/');

        return segments.Length > 1 && segments[0] == "src"
            ? segments[1]
            : relativePath;
    }
}
