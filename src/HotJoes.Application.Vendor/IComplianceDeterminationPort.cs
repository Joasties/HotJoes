namespace HotJoes.Application.Vendor;

public interface IComplianceDeterminationPort
{
    ComplianceDeterminationPortResult Determine(
        ComplianceDeterminationRequest request);
}
