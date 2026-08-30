namespace HotJoes.Domain.Vendor.Tests;

public sealed class DomainHostedFailureEvidenceTests
{
    [Fact]
    public void DeliberateHostedUnitFailure_PreventsMerge()
    {
        Assert.Fail("Deliberate CON-039 hosted unit-test failure evidence.");
    }
}
