namespace HotJoes.Web.Edge.Tests;

public sealed class EdgeBoundaryStructuralTests
{
    [Fact]
    public void AI_GW_005_EdgeHasNoBusinessProjectReferences()
    {
        string project = File.ReadAllText(FindRepositoryFile(
            "src/HotJoes.Web.Edge/HotJoes.Web.Edge.csproj"));

        Assert.DoesNotContain("<ProjectReference", project, StringComparison.Ordinal);
        Assert.Contains("Yarp.ReverseProxy", project, StringComparison.Ordinal);
    }

    [Fact]
    public void AI_GW_004_UntrustedForwardingHeadersAreRemoved()
    {
        string program = File.ReadAllText(FindRepositoryFile(
            "src/HotJoes.Web.Edge/Program.cs"));

        Assert.Contains("Headers.Remove(\"Forwarded\")", program);
        Assert.Contains("Headers.Remove(\"X-Forwarded-For\")", program);
        Assert.Contains("Headers.Remove(\"X-Forwarded-Host\")", program);
        Assert.Contains("Headers.Remove(\"X-Forwarded-Proto\")", program);
    }

    private static string FindRepositoryFile(string relativePath)
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(
                directory.FullName,
                relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not locate repository file '{relativePath}'.");
    }
}
