using System.Reflection;
using System.Text.RegularExpressions;

namespace HotJoes.Architecture.Tests;

public sealed class ArchitectureHarnessCatalog
{
    private const string SolutionFileName = "HotJoes.sln";

    private ArchitectureHarnessCatalog(
        IEnumerable<string> ruleSetNames,
        IEnumerable<string> executableObligationIds,
        IEnumerable<string> solutionProjectPaths)
    {
        RuleSetNames = Normalize(ruleSetNames);
        ExecutableObligationIds = Normalize(executableObligationIds);
        SolutionProjectPaths = Normalize(solutionProjectPaths);
    }

    public IReadOnlyList<string> RuleSetNames { get; }

    public IReadOnlyList<string> ExecutableObligationIds { get; }

    public IReadOnlyList<string> SolutionProjectPaths { get; }

    public static ArchitectureHarnessCatalog LoadCurrent()
    {
        Assembly assembly = typeof(ArchitectureHarnessCatalog).Assembly;

        string[] ruleSetNames = assembly
            .GetTypes()
            .Where(IsProductionBoundaryRuleSet)
            .Select(type => type.Name)
            .ToArray();
        string[] executableObligationIds = assembly
            .GetTypes()
            .Where(type => type.IsClass &&
                type.Name.EndsWith("Tests", StringComparison.Ordinal))
            .SelectMany(type => type.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.DeclaredOnly))
            .Where(IsExecutableTest)
            .Select(method => ParseObligationId(method.Name))
            .Where(id => id is not null)
            .Select(id => id!)
            .ToArray();
        string[] solutionProjectPaths = LoadSolutionProjectPaths(
            FindRepositoryRoot());

        return new ArchitectureHarnessCatalog(
            ruleSetNames,
            executableObligationIds,
            solutionProjectPaths);
    }

    public static ArchitectureHarnessCatalog FromValues(
        IEnumerable<string> ruleSetNames,
        IEnumerable<string> executableObligationIds,
        IEnumerable<string> solutionProjectPaths)
    {
        ArgumentNullException.ThrowIfNull(ruleSetNames);
        ArgumentNullException.ThrowIfNull(executableObligationIds);
        ArgumentNullException.ThrowIfNull(solutionProjectPaths);

        return new ArchitectureHarnessCatalog(
            ruleSetNames,
            executableObligationIds,
            solutionProjectPaths);
    }

    private static bool IsProductionBoundaryRuleSet(Type type)
    {
        return type.IsClass &&
            type.IsAbstract &&
            type.IsSealed &&
            type.Name.EndsWith("RuleSet", StringComparison.Ordinal) &&
            type != typeof(ArchitectureHarnessCompletenessRuleSet);
    }

    private static bool IsExecutableTest(MethodInfo method)
    {
        return method.CustomAttributes.Any(attribute =>
            attribute.AttributeType.FullName is
                "Xunit.FactAttribute" or "Xunit.TheoryAttribute");
    }

    private static string? ParseObligationId(string methodName)
    {
        Match match = Regex.Match(
            methodName,
            @"^(?<id>AI_[A-Z]+_\d{3})_",
            RegexOptions.CultureInvariant);

        return match.Success
            ? match.Groups["id"].Value.Replace('_', '-')
            : null;
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(
                    directory.FullName,
                    SolutionFileName)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not locate {SolutionFileName} from " +
            $"'{AppContext.BaseDirectory}'.");
    }

    private static string[] LoadSolutionProjectPaths(string repositoryRoot)
    {
        string solutionPath = Path.Combine(
            repositoryRoot,
            SolutionFileName);

        return File.ReadLines(solutionPath)
            .Select(line => Regex.Match(
                line,
                "^Project\\(\"[^\"]+\"\\) = \"[^\"]+\", " +
                "\"(?<path>[^\"]+\\.csproj)\", \"[^\"]+\"$",
                RegexOptions.CultureInvariant))
            .Where(match => match.Success)
            .Select(match => match.Groups["path"].Value.Replace('\\', '/'))
            .ToArray();
    }

    private static string[] Normalize(IEnumerable<string> values)
    {
        return values
            .Select(value => value ?? throw new ArgumentException(
                "Catalog values must not contain null entries.",
                nameof(values)))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }
}
