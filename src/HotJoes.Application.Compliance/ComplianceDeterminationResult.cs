namespace HotJoes.Application.Compliance;

public abstract class ComplianceDeterminationResult
{
    private ComplianceDeterminationResult()
    {
    }

    public static ComplianceDeterminationResult Succeeded(
        ComplianceDetermination determination) =>
        new Success(determination);

    public static ComplianceDeterminationResult Unsupported() =>
        new UnsupportedCoverage();

    public static ComplianceDeterminationResult TemporarilyUnavailable() =>
        new TemporaryFailure();

    public sealed class Success : ComplianceDeterminationResult
    {
        internal Success(ComplianceDetermination determination)
        {
            ArgumentNullException.ThrowIfNull(determination);
            Determination = determination;
        }

        public ComplianceDetermination Determination { get; }
    }

    public sealed class UnsupportedCoverage : ComplianceDeterminationResult
    {
        internal UnsupportedCoverage()
        {
        }
    }

    public sealed class TemporaryFailure : ComplianceDeterminationResult
    {
        internal TemporaryFailure()
        {
        }
    }
}
