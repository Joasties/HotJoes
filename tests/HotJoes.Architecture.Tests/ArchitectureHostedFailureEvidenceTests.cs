namespace HotJoes.Architecture.Tests;

public sealed class ArchitectureHostedFailureEvidenceTests
{
    [Fact]
    public void AI_CI_002_DeliberateHostedArchitectureFailure_PreventsMerge()
    {
        Assert.Fail(
            "Deliberate CON-039 hosted architecture-test failure evidence.");
    }
}
