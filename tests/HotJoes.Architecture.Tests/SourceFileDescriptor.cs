namespace HotJoes.Architecture.Tests;

public sealed class SourceFileDescriptor
{
    public SourceFileDescriptor(string relativePath, string content)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException(
                "Source path must not be empty.",
                nameof(relativePath));
        }

        ArgumentNullException.ThrowIfNull(content);

        RelativePath = relativePath.Replace('\\', '/');
        Content = content;
    }

    public string RelativePath { get; }

    public string Content { get; }
}
