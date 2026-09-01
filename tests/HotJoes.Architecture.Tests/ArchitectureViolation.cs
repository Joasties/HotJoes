namespace HotJoes.Architecture.Tests;

public sealed record ArchitectureViolation(
    string ObligationId,
    string ProjectName,
    string Description);
