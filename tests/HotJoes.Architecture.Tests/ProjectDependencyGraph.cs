namespace HotJoes.Architecture.Tests;

public sealed class ProjectDependencyGraph
{
    public ProjectDependencyGraph(
        IEnumerable<ProjectDependencyNode> productionProjects)
    {
        ArgumentNullException.ThrowIfNull(productionProjects);

        ProjectDependencyNode[] projects = productionProjects
            .OrderBy(project => project.Name, StringComparer.Ordinal)
            .ToArray();
        string? duplicateName = projects
            .GroupBy(project => project.Name, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1)
            ?.Key;

        if (duplicateName is not null)
        {
            throw new ArgumentException(
                $"Duplicate production project '{duplicateName}'.",
                nameof(productionProjects));
        }

        ProductionProjects = projects;
    }

    public IReadOnlyList<ProjectDependencyNode> ProductionProjects { get; }
}
