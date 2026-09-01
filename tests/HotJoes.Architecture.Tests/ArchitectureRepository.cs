using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HotJoes.Architecture.Tests;

public sealed partial class ArchitectureRepository
{
    private const string SolutionFileName = "HotJoes.sln";

    private readonly string _rootPath;

    private ArchitectureRepository(string rootPath)
    {
        _rootPath = rootPath;
    }

    public static ArchitectureRepository FindFromTestAssembly()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(
                    directory.FullName,
                    SolutionFileName)))
            {
                return new ArchitectureRepository(directory.FullName);
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not locate {SolutionFileName} from " +
            $"'{AppContext.BaseDirectory}'.");
    }

    public ProjectDependencyGraph LoadProjectGraph()
    {
        string solutionPath = Path.Combine(_rootPath, SolutionFileName);
        var projects = new List<ProjectDependencyNode>();

        foreach (string line in File.ReadLines(solutionPath))
        {
            Match match = SolutionProjectLine().Match(line);

            if (!match.Success)
            {
                continue;
            }

            string relativePath = NormalizePath(
                match.Groups["path"].Value);

            if (!relativePath.StartsWith(
                    "src/",
                    StringComparison.Ordinal))
            {
                continue;
            }

            string absolutePath = Path.GetFullPath(
                Path.Combine(_rootPath, relativePath));
            projects.Add(LoadProject(
                match.Groups["name"].Value,
                relativePath,
                absolutePath));
        }

        return new ProjectDependencyGraph(projects);
    }

    private static ProjectDependencyNode LoadProject(
        string projectName,
        string relativePath,
        string absolutePath)
    {
        if (!File.Exists(absolutePath))
        {
            throw new InvalidOperationException(
                $"Solution project '{relativePath}' does not exist.");
        }

        XDocument project = XDocument.Load(absolutePath);
        string projectDirectory = Path.GetDirectoryName(absolutePath) ??
            throw new InvalidOperationException(
                $"Project '{relativePath}' has no containing directory.");
        string[] projectReferences = project
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFileNameWithoutExtension(
                Path.GetFullPath(
                    Path.Combine(
                        projectDirectory,
                        NormalizeFileSystemPath(value!)))))
            .ToArray();
        string[] packageReferences = project
            .Descendants("PackageReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToArray();

        return new ProjectDependencyNode(
            projectName,
            relativePath,
            projectReferences,
            packageReferences);
    }

    private static string NormalizePath(string value)
    {
        return value.Replace('\\', '/');
    }

    private static string NormalizeFileSystemPath(string value)
    {
        return value.Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }

    [GeneratedRegex(
        "^Project\\(\"[^\"]+\"\\) = \"(?<name>[^\"]+)\", " +
        "\"(?<path>[^\"]+\\.csproj)\", \"[^\"]+\"$",
        RegexOptions.CultureInvariant)]
    private static partial Regex SolutionProjectLine();
}
