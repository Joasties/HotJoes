namespace HotJoes.Application.Vendor;

public interface IDetermineRequiredLicenceTypesService
{
    Task<DetermineRequiredLicenceTypesResult> DetermineAsync(
        DetermineRequiredLicenceTypesRequest request,
        CancellationToken cancellationToken = default);
}
