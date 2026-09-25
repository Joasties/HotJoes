using HotJoes.Application.Vendor;
using Microsoft.AspNetCore.Http;

namespace HotJoes.Api.Vendor;

public sealed class VendorApiErrorMapper
{
    public VendorApiErrorMapping Map(
        DetermineRequiredLicenceTypesResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result switch
        {
            DetermineRequiredLicenceTypesResult
                .DeterminationRequestValidationFailure failure =>
                DeterminationValidationFailure(failure),
            DetermineRequiredLicenceTypesResult.InvalidReference => Create(
                StatusCodes.Status400BadRequest,
                "invalidAddressReference",
                "The Address reference is invalid or unknown."),
            DetermineRequiredLicenceTypesResult.InvalidAddressResult => Create(
                StatusCodes.Status400BadRequest,
                "invalidAddressResult",
                "The selected Address result is not valid for Compliance determination."),
            DetermineRequiredLicenceTypesResult.UnsupportedDetermination => Create(
                StatusCodes.Status409Conflict,
                "complianceDeterminationUnsupported",
                "The supplied information is outside the supported Compliance determination coverage."),
            DetermineRequiredLicenceTypesResult
                .AddressServiceTemporarilyUnavailable => Create(
                StatusCodes.Status503ServiceUnavailable,
                "addressServiceTemporarilyUnavailable",
                "The Address service is temporarily unavailable. The request may be retried."),
            DetermineRequiredLicenceTypesResult
                .ComplianceDeterminationTemporarilyUnavailable => Create(
                StatusCodes.Status503ServiceUnavailable,
                "complianceDeterminationTemporarilyUnavailable",
                "Compliance determination is temporarily unavailable. The request may be retried."),
            DetermineRequiredLicenceTypesResult.Success => throw new ArgumentException(
                "Successful determination results are not error mappings.",
                nameof(result)),
            _ => throw new ArgumentOutOfRangeException(
                nameof(result),
                result.GetType().FullName,
                "The determination result is not an approved controlled failure.")
        };
    }

    public VendorApiErrorMapping Map(RegisterVendorResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result switch
        {
            RegisterVendorResult.RequestValidationFailure failure =>
                ValidationFailure(failure),
            RegisterVendorResult.InvalidReference => Create(
                StatusCodes.Status400BadRequest,
                "invalidAddressReference",
                "The Address reference is invalid or unknown."),
            RegisterVendorResult.InvalidAddressResult => Create(
                StatusCodes.Status400BadRequest,
                "invalidAddressResult",
                "The selected Address result is not valid for Vendor registration."),
            RegisterVendorResult.AggregateInvariantFailure => Create(
                StatusCodes.Status400BadRequest,
                "aggregateInvariantFailed",
                "The Vendor could not be registered because supplied information conflicts with a Vendor rule."),
            RegisterVendorResult.IdempotencyConflict => Create(
                StatusCodes.Status409Conflict,
                "idempotencyConflict",
                "A Vendor with the same registration identity already exists with different information."),
            RegisterVendorResult.AddressServiceTemporarilyUnavailable => Create(
                StatusCodes.Status503ServiceUnavailable,
                "addressServiceTemporarilyUnavailable",
                "The Address service is temporarily unavailable. The request may be retried."),
            RegisterVendorResult.PersistenceOrAtomicRecordingFailure => Create(
                StatusCodes.Status503ServiceUnavailable,
                "persistenceOrAtomicRecordingFailed",
                "The Vendor registration could not be recorded. The request may be retried."),
            RegisterVendorResult.Success => throw new ArgumentException(
                "Successful registration results are not error mappings.",
                nameof(result)),
            _ => throw new ArgumentOutOfRangeException(
                nameof(result),
                result.GetType().FullName,
                "The RegisterVendor result is not an approved controlled failure.")
        };
    }

    public VendorApiErrorMapping Map(RetrieveRegisteredVendorResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result switch
        {
            RetrieveRegisteredVendorResult.NotFound => Create(
                StatusCodes.Status404NotFound,
                "vendorNotFound",
                "The requested Vendor was not found."),
            RetrieveRegisteredVendorResult.Found => throw new ArgumentException(
                "Successful retrieval results are not error mappings.",
                nameof(result)),
            _ => throw new ArgumentOutOfRangeException(
                nameof(result),
                result.GetType().FullName,
                "The retrieval result is not an approved controlled failure.")
        };
    }

    public VendorApiErrorMapping MalformedRequest()
    {
        return Create(
            StatusCodes.Status400BadRequest,
            "requestMalformed",
            "The request body or route value is malformed or structurally invalid.");
    }

    private static VendorApiErrorMapping ValidationFailure(
        RegisterVendorResult.RequestValidationFailure failure)
    {
        VendorApiValidationErrorResponse[] errors = failure.Errors
            .Select(Map)
            .ToArray();

        return new VendorApiErrorMapping(
            StatusCodes.Status400BadRequest,
            new VendorApiErrorResponse(
                "registrationValidationFailed",
                "The Vendor could not be registered because supplied information is invalid.",
                Array.AsReadOnly(errors)));
    }

    private static VendorApiErrorMapping DeterminationValidationFailure(
        DetermineRequiredLicenceTypesResult
            .DeterminationRequestValidationFailure failure)
    {
        VendorApiValidationErrorResponse[] errors = failure.Errors
            .Select(MapDeterminationValidationError)
            .ToArray();

        return new VendorApiErrorMapping(
            StatusCodes.Status400BadRequest,
            new VendorApiErrorResponse(
                "determinationValidationFailed",
                "Required Licence Types could not be determined because supplied information is invalid.",
                Array.AsReadOnly(errors)));
    }

    private static VendorApiValidationErrorResponse
        MapDeterminationValidationError(RegistrationValidationError error)
    {
        return new VendorApiValidationErrorResponse(
            MapDeterminationField(error.Field),
            MapCode(error.Code),
            error.Message);
    }

    private static string MapDeterminationField(string field)
    {
        return field switch
        {
            nameof(DetermineRequiredLicenceTypesRequest.LegalOperatorType) =>
                "legalOperatorType",
            nameof(DetermineRequiredLicenceTypesRequest.TradingLocation) =>
                "tradingLocation",
            nameof(DetermineRequiredLicenceTypesRequest.WeeklyOpeningHours) =>
                "weeklyOpeningHours.days",
            nameof(DetermineRequiredLicenceTypesRequest.ServiceIncludesHotFood) =>
                "serviceIncludesHotFood",
            nameof(DetermineRequiredLicenceTypesRequest.AlcoholService) =>
                "alcoholService",
            nameof(DetermineRequiredLicenceTypesRequest.AddressResolutionReference) =>
                "addressResolutionReference",
            _ when field.StartsWith("weeklyOpeningHours.", StringComparison.Ordinal) =>
                field,
            _ when char.IsLower(field[0]) => field,
            _ => throw new ArgumentOutOfRangeException(
                nameof(field),
                field,
                "The determination validation field has no approved API JSON path.")
        };
    }

    private static VendorApiValidationErrorResponse Map(
        RegistrationValidationError error)
    {
        return new VendorApiValidationErrorResponse(
            MapField(error.Field),
            MapCode(error.Code),
            error.Message);
    }

    private static VendorApiErrorMapping Create(
        int statusCode,
        string code,
        string message)
    {
        return new VendorApiErrorMapping(
            statusCode,
            new VendorApiErrorResponse(
                code,
                message,
                ValidationErrors: null));
    }

    private static string MapField(string field)
    {
        return field switch
        {
            nameof(RegisterVendorCommand.TradingName) =>
                "tradingName",
            nameof(RegisterVendorCommand.LegalOperatorName) =>
                "legalOperatorName",
            nameof(RegisterVendorCommand.LegalOperatorType) =>
                "legalOperatorType",
            nameof(RegisterVendorCommand.CompanyRegistrationNumber) =>
                "companyRegistrationNumber",
            nameof(RegisterVendorCommand.TradingLocation) =>
                "tradingCharacteristics.tradingLocation",
            nameof(RegisterVendorCommand.WeeklyOpeningHours) =>
                "tradingCharacteristics.weeklyOpeningHours.days",
            nameof(RegisterVendorCommand.ServiceIncludesHotFood) =>
                "tradingCharacteristics.serviceIncludesHotFood",
            nameof(RegisterVendorCommand.AlcoholService) =>
                "tradingCharacteristics.alcoholService",
            nameof(RegisterVendorCommand.ContactName) =>
                "primaryContact.contactName",
            nameof(RegisterVendorCommand.ContactEmail) =>
                "primaryContact.contactEmail",
            nameof(RegisterVendorCommand.ContactTelephone) =>
                "primaryContact.contactTelephone",
            nameof(RegisterVendorCommand.AddressResolutionReference) =>
                "addressResolutionReference",
            nameof(RegisterVendorCommand.Website) =>
                "website",
            nameof(RegisterVendorCommand.BusinessDescription) =>
                "businessDescription",
            nameof(RegisterVendorCommand.AuthorisedToRegisterBusiness) =>
                "registrationDeclarations.authorisedToRegisterBusiness",
            nameof(RegisterVendorCommand.InformationAccurate) =>
                "registrationDeclarations.informationAccurate",
            nameof(RegisterVendorCommand.AcceptHotJoesPlatformTerms) =>
                "registrationDeclarations.acceptHotJoesPlatformTerms",
            _ when field.StartsWith(
                $"{nameof(RegisterVendorCommand.WeeklyOpeningHours)}.",
                StringComparison.Ordinal) =>
                "tradingCharacteristics.weeklyOpeningHours.days",
            _ => throw new ArgumentOutOfRangeException(
                nameof(field),
                field,
                "The Application validation field has no approved API JSON path.")
        };
    }

    private static string MapCode(RegistrationValidationErrorCode code)
    {
        return code switch
        {
            RegistrationValidationErrorCode.Required =>
                "required",
            RegistrationValidationErrorCode.InvalidFormat =>
                "invalidFormat",
            RegistrationValidationErrorCode.LengthOutOfRange =>
                "lengthOutOfRange",
            RegistrationValidationErrorCode.InvalidValue =>
                "invalidValue",
            RegistrationValidationErrorCode.ConditionallyRequired =>
                "conditionallyRequired",
            RegistrationValidationErrorCode.Prohibited =>
                "prohibited",
            _ => throw new ArgumentOutOfRangeException(
                nameof(code),
                code,
                "The Application validation code has no approved API code.")
        };
    }
}
