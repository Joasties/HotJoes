using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class DetermineRequiredLicenceTypesService
    : IDetermineRequiredLicenceTypesService
{
    private readonly DetermineRequiredLicenceTypesRequestValidator _validator;
    private readonly AddressResolutionInvoker _addressResolutionInvoker;
    private readonly IComplianceDeterminationPort _compliance;

    public DetermineRequiredLicenceTypesService(
        DetermineRequiredLicenceTypesRequestValidator validator,
        AddressResolutionInvoker addressResolutionInvoker,
        IComplianceDeterminationPort compliance)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(addressResolutionInvoker);
        ArgumentNullException.ThrowIfNull(compliance);

        _validator = validator;
        _addressResolutionInvoker = addressResolutionInvoker;
        _compliance = compliance;
    }

    public Task<DetermineRequiredLicenceTypesResult> DetermineAsync(
        DetermineRequiredLicenceTypesRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        DetermineRequiredLicenceTypesRequestValidationResult validation =
            _validator.Validate(request);

        DetermineRequiredLicenceTypesResult result = validation switch
        {
            DetermineRequiredLicenceTypesRequestValidationResult.Invalid invalid =>
                DetermineRequiredLicenceTypesResult.ValidationFailed(
                    invalid.Errors),
            DetermineRequiredLicenceTypesRequestValidationResult.Accepted accepted =>
                DetermineValidated(accepted.Request),
            _ => throw new InvalidOperationException(
                "The determination request validation result is not supported.")
        };

        return Task.FromResult(result);
    }

    private DetermineRequiredLicenceTypesResult DetermineValidated(
        DetermineRequiredLicenceTypesRequest request)
    {
        AddressResolutionResult addressResult =
            _addressResolutionInvoker.Resolve(
                request.AddressResolutionReference,
                request.TradingLocation);

        return addressResult switch
        {
            AddressResolutionResult.Success success =>
                DetermineWithAuthoritativeAddress(request, success.Values),
            AddressResolutionResult.InvalidReference =>
                DetermineRequiredLicenceTypesResult.ReferenceIsInvalid(),
            AddressResolutionResult.InvalidAddressResult =>
                DetermineRequiredLicenceTypesResult.AddressResultIsInvalid(),
            AddressResolutionResult.AddressServiceTemporarilyUnavailable =>
                DetermineRequiredLicenceTypesResult
                    .AddressIsTemporarilyUnavailable(),
            _ => throw new InvalidOperationException(
                "The Address Resolution result is not supported.")
        };
    }

    private DetermineRequiredLicenceTypesResult DetermineWithAuthoritativeAddress(
        DetermineRequiredLicenceTypesRequest request,
        AddressAuthoritativeValues address)
    {
        if (request.TradingLocation == TradingLocation.Stall
            && address.PrimaryTradingAuthority is null)
        {
            return DetermineRequiredLicenceTypesResult.AddressResultIsInvalid();
        }

        return DetermineWithCompliance(request, address);
    }

    private DetermineRequiredLicenceTypesResult DetermineWithCompliance(
        DetermineRequiredLicenceTypesRequest request,
        AddressAuthoritativeValues address)
    {
        var complianceRequest = new ComplianceDeterminationRequest(
            request.LegalOperatorType,
            request.TradingLocation,
            request.WeeklyOpeningHours,
            request.ServiceIncludesHotFood,
            request.AlcoholService,
            address.BusinessAddressSnapshot,
            address.FoodRegistrationAuthority,
            address.PrimaryTradingAuthority);

        ComplianceDeterminationPortResult complianceResult =
            _compliance.Determine(complianceRequest);

        return complianceResult switch
        {
            ComplianceDeterminationPortResult.Success success =>
                DetermineRequiredLicenceTypesResult.Succeeded(
                    success.Determination),
            ComplianceDeterminationPortResult.UnsupportedDetermination =>
                DetermineRequiredLicenceTypesResult.Unsupported(),
            ComplianceDeterminationPortResult
                .ComplianceDeterminationTemporarilyUnavailable =>
                DetermineRequiredLicenceTypesResult
                    .ComplianceIsTemporarilyUnavailable(),
            _ => throw new InvalidOperationException(
                "The Compliance Determination result is not supported.")
        };
    }
}
