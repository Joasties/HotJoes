using HotJoes.Application.Vendor;
using ApplicationRequest = HotJoes.Application.Vendor.DetermineRequiredLicenceTypesRequest;

namespace HotJoes.Api.Vendor.Tests;

public sealed class StubDetermineRequiredLicenceTypesService
    : IDetermineRequiredLicenceTypesService
{
    public DetermineRequiredLicenceTypesResult NextResult { get; set; } =
        DetermineRequiredLicenceTypesResult.ReferenceIsInvalid();

    public int InvocationCount { get; private set; }

    public ApplicationRequest? LastRequest { get; private set; }

    public bool LastCancellationTokenCanBeCanceled { get; private set; }

    public Task<DetermineRequiredLicenceTypesResult> DetermineAsync(
        ApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        InvocationCount++;
        LastRequest = request;
        LastCancellationTokenCanBeCanceled = cancellationToken.CanBeCanceled;
        return Task.FromResult(NextResult);
    }
}
