namespace HotJoes.Architecture.Tests;

public sealed class ArchitectureSourceCatalog
{
    private static readonly string[] ControlledRelativePaths =
    [
        "src/HotJoes.Api.Vendor/HotJoes.Api.Vendor.csproj",
        "src/HotJoes.Api.Vendor/Program.cs",
        "src/HotJoes.Api.Vendor/VendorApiErrorMapper.cs",
        "src/HotJoes.Api.Vendor/VendorApiExceptionHandler.cs",
        "src/HotJoes.Api.Vendor/VendorEndpointMappings.cs",
        "src/HotJoes.Infrastructure.Vendor.Address/AddressResolutionAdapter.cs",
        "src/HotJoes.Infrastructure.Vendor.Address/HotJoes.Infrastructure.Vendor.Address.csproj"
    ];

    private ArchitectureSourceCatalog(
        IEnumerable<SourceFileDescriptor> files)
    {
        Files = files
            .OrderBy(file => file.RelativePath, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyList<SourceFileDescriptor> Files { get; }

    public static ArchitectureSourceCatalog LoadControlledSources()
    {
        string repositoryRoot = FindRepositoryRoot();
        SourceFileDescriptor[] files = ControlledRelativePaths
            .Select(relativePath => Load(repositoryRoot, relativePath))
            .ToArray();

        return new ArchitectureSourceCatalog(files);
    }

    public static ArchitectureSourceCatalog LoadProductionSources()
    {
        string repositoryRoot = FindRepositoryRoot();
        string sourceRoot = Path.Combine(repositoryRoot, "src");

        if (!Directory.Exists(sourceRoot))
        {
            throw new InvalidOperationException(
                $"Production source root '{sourceRoot}' does not exist.");
        }

        SourceFileDescriptor[] files = Directory
            .EnumerateFiles(
                sourceRoot,
                "*",
                SearchOption.AllDirectories)
            .Select(absolutePath => new
            {
                AbsolutePath = absolutePath,
                RelativePath = Path.GetRelativePath(
                        repositoryRoot,
                        absolutePath)
                    .Replace('\\', '/')
            })
            .Where(file => IsGovernedSource(file.RelativePath))
            .Select(file => new SourceFileDescriptor(
                file.RelativePath,
                File.ReadAllText(file.AbsolutePath)))
            .ToArray();

        return new ArchitectureSourceCatalog(files);
    }

    public static ArchitectureSourceCatalog FromFiles(
        IEnumerable<SourceFileDescriptor> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        return new ArchitectureSourceCatalog(files);
    }

    private static SourceFileDescriptor Load(
        string repositoryRoot,
        string relativePath)
    {
        string absolutePath = Path.Combine(
            repositoryRoot,
            relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(absolutePath))
        {
            throw new InvalidOperationException(
                $"Controlled source file '{relativePath}' does not exist.");
        }

        return new SourceFileDescriptor(
            relativePath,
            File.ReadAllText(absolutePath));
    }

    private static bool IsBuildArtifact(string relativePath)
    {
        return relativePath.Contains("/bin/", StringComparison.Ordinal) ||
            relativePath.Contains("/obj/", StringComparison.Ordinal);
    }

    private static bool IsGovernedSource(string relativePath)
    {
        return !IsBuildArtifact(relativePath) &&
            (relativePath.EndsWith(
                    ".cs",
                    StringComparison.OrdinalIgnoreCase) ||
                relativePath.EndsWith(
                    ".json",
                    StringComparison.OrdinalIgnoreCase));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "HotJoes.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate HotJoes.sln from the architecture-test " +
            $"assembly path '{AppContext.BaseDirectory}'.");
    }
}
