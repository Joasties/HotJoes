namespace HotJoes.Application.Compliance;

public interface IComplianceDeterminationService
{
    ComplianceDeterminationResult Determine(
        ComplianceDeterminationRequest request);
}
