namespace HotJoes.Application.Vendor;

public abstract class ComplianceDeterminationPortResult
{
    private ComplianceDeterminationPortResult()
    {
    }

    public static ComplianceDeterminationPortResult Succeeded(
        ComplianceDetermination determination)
    {
        ArgumentNullException.ThrowIfNull(determination);
        return new Success(determination);
    }

    public static ComplianceDeterminationPortResult Unsupported() =>
        new UnsupportedDetermination();

    public static ComplianceDeterminationPortResult TemporarilyUnavailable() =>
        new ComplianceDeterminationTemporarilyUnavailable();

    public sealed class Success : ComplianceDeterminationPortResult
    {
        internal Success(ComplianceDetermination determination)
        {
            Determination = determination;
        }

        public ComplianceDetermination Determination { get; }
    }

    public sealed class UnsupportedDetermination
        : ComplianceDeterminationPortResult
    {
        internal UnsupportedDetermination()
        {
        }
    }

    public sealed class ComplianceDeterminationTemporarilyUnavailable
        : ComplianceDeterminationPortResult
    {
        internal ComplianceDeterminationTemporarilyUnavailable()
        {
        }
    }
}
