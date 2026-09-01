using System.Text.RegularExpressions;

namespace HotJoes.Architecture.Tests;

public static class ApiAddressStructuralRuleSet
{
    private const string EndpointPath =
        "src/HotJoes.Api.Vendor/VendorEndpointMappings.cs";
    private const string MapperPath =
        "src/HotJoes.Api.Vendor/VendorApiErrorMapper.cs";
    private const string ProgramPath =
        "src/HotJoes.Api.Vendor/Program.cs";

    private static readonly string[] ProhibitedEndpointTokens =
    [
        "IAddressResolver",
        "IVendorRepository",
        "DbContext",
        "TransactionScope",
        "IDbContextTransaction",
        "VendorRegisteredIntegrationEvent",
        "Outbox",
        "RabbitMq",
        "HotJoes.Infrastructure."
    ];

    private static readonly string[] SupersededFailureTokens =
    [
        "RegistrationDeclarationFailure",
        "ConditionalRuleFailure"
    ];

    private static readonly string[] CircuitBreakerTokens =
    [
        "Polly",
        "CircuitBreaker",
        "AddResilienceHandler",
        "CircuitBreakerStrategyOptions"
    ];

    public static IReadOnlyList<ArchitectureViolation> Evaluate(
        ArchitectureSourceCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var violations = new List<ArchitectureViolation>();
        EvaluateEndpoints(catalog, violations);
        EvaluateControlledMapping(catalog, violations);
        EvaluateCentralExceptionHandling(catalog, violations);
        EvaluateAddressResilience(catalog, violations);

        return violations
            .OrderBy(violation => violation.ObligationId)
            .ThenBy(violation => violation.ProjectName)
            .ThenBy(violation => violation.Description)
            .ToArray();
    }

    private static void EvaluateEndpoints(
        ArchitectureSourceCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        SourceFileDescriptor? endpoint = Find(catalog, EndpointPath);

        if (endpoint is null)
        {
            Add(
                violations,
                "AI-API-001",
                EndpointPath,
                "Controlled endpoint source is missing.");
            return;
        }

        foreach (string token in ProhibitedEndpointTokens)
        {
            if (endpoint.Content.Contains(
                    token,
                    StringComparison.Ordinal))
            {
                Add(
                    violations,
                    "AI-API-001",
                    endpoint.RelativePath,
                    $"Endpoint contains prohibited collaborator evidence " +
                    $"'{token}'.");
            }
        }

        if (Regex.IsMatch(
                endpoint.Content,
                @"\bcatch\s*(?:\(|\{)",
                RegexOptions.CultureInvariant))
        {
            Add(
                violations,
                "AI-API-003",
                endpoint.RelativePath,
                "Endpoint contains local exception handling.");
        }

        if (Regex.IsMatch(
                endpoint.Content,
                @"\bnew\s+VendorApiErrorResponse\s*\(",
                RegexOptions.CultureInvariant))
        {
            Add(
                violations,
                "AI-API-003",
                endpoint.RelativePath,
                "Endpoint constructs an API error payload locally.");
        }
    }

    private static void EvaluateControlledMapping(
        ArchitectureSourceCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        int mapperDeclarations = catalog.Files.Sum(file => Count(
            file.Content,
            @"\bclass\s+VendorApiErrorMapper\b"));

        if (mapperDeclarations != 1)
        {
            Add(
                violations,
                "AI-API-003",
                "HotJoes.Api.Vendor",
                $"Expected one VendorApiErrorMapper declaration but found " +
                $"{mapperDeclarations}.");
        }

        SourceFileDescriptor? mapper = Find(catalog, MapperPath);

        if (mapper is null)
        {
            Add(
                violations,
                "AI-API-003",
                MapperPath,
                "Controlled API error mapper source is missing.");
            return;
        }

        int validationBranches = Count(
            mapper.Content,
            @"RegisterVendorResult\s*\.\s*RequestValidationFailure" +
            @"\s+\w+\s*=>");

        if (validationBranches != 1)
        {
            Add(
                violations,
                "AI-API-003",
                mapper.RelativePath,
                $"Expected one RequestValidationFailure mapping branch but " +
                $"found {validationBranches}.");
        }

        foreach (string token in SupersededFailureTokens)
        {
            if (mapper.Content.Contains(token, StringComparison.Ordinal))
            {
                Add(
                    violations,
                    "AI-API-003",
                    mapper.RelativePath,
                    $"Mapper contains superseded failure '{token}'.");
            }
        }
    }

    private static void EvaluateCentralExceptionHandling(
        ArchitectureSourceCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        int handlerDeclarations = catalog.Files.Sum(file => Count(
            file.Content,
            @"\bclass\s+VendorApiExceptionHandler\b"));

        if (handlerDeclarations != 1)
        {
            Add(
                violations,
                "AI-API-003",
                "HotJoes.Api.Vendor",
                $"Expected one VendorApiExceptionHandler declaration but " +
                $"found {handlerDeclarations}.");
        }

        SourceFileDescriptor? program = Find(catalog, ProgramPath);

        if (program is null ||
            Count(
                program.Content,
                @"AddExceptionHandler\s*<\s*VendorApiExceptionHandler\s*>") != 1 ||
            Count(program.Content, @"\bUseExceptionHandler\s*\(") != 1)
        {
            Add(
                violations,
                "AI-API-003",
                ProgramPath,
                "Program does not compose exactly one central Vendor API " +
                "exception handler boundary.");
        }
    }

    private static void EvaluateAddressResilience(
        ArchitectureSourceCatalog catalog,
        ICollection<ArchitectureViolation> violations)
    {
        IEnumerable<SourceFileDescriptor> applicableFiles =
            catalog.Files.Where(file =>
                file.RelativePath.StartsWith(
                    "src/HotJoes.Infrastructure.Vendor.Address/",
                    StringComparison.Ordinal) ||
                file.RelativePath == ProgramPath);

        foreach (SourceFileDescriptor file in applicableFiles)
        {
            foreach (string token in CircuitBreakerTokens)
            {
                if (file.Content.Contains(
                        token,
                        StringComparison.OrdinalIgnoreCase))
                {
                    Add(
                        violations,
                        "AI-ADDR-007",
                        file.RelativePath,
                        $"Address boundary contains circuit-breaker evidence " +
                        $"'{token}'.");
                }
            }
        }
    }

    private static SourceFileDescriptor? Find(
        ArchitectureSourceCatalog catalog,
        string relativePath)
    {
        return catalog.Files.SingleOrDefault(file =>
            file.RelativePath == relativePath);
    }

    private static int Count(string content, string pattern)
    {
        return Regex.Matches(
            content,
            pattern,
            RegexOptions.CultureInvariant).Count;
    }

    private static void Add(
        ICollection<ArchitectureViolation> violations,
        string obligationId,
        string projectName,
        string description)
    {
        violations.Add(new ArchitectureViolation(
            obligationId,
            projectName,
            description));
    }
}
