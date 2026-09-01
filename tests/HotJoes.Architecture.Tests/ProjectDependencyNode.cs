namespace HotJoes.Architecture.Tests;

public sealed class ProjectDependencyNode
{
    public ProjectDependencyNode(
        string name,
        string projectPath,
        IEnumerable<string> projectReferences,
        IEnumerable<string> packageReferences)
    {
        Name = RequireValue(name, nameof(name));
        ProjectPath = RequireValue(projectPath, nameof(projectPath));
        ProjectReferences = projectReferences
            .Select(reference => RequireValue(
                reference,
                nameof(projectReferences)))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        PackageReferences = packageReferences
            .Select(reference => RequireValue(
                reference,
                nameof(packageReferences)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public string Name { get; }

    public string ProjectPath { get; }

    public IReadOnlyList<string> ProjectReferences { get; }

    public IReadOnlyList<string> PackageReferences { get; }

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Architecture metadata value must not be empty.",
                parameterName);
        }

        return value;
    }
}
