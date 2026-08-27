using HotJoes.Application.Vendor;
using Microsoft.AspNetCore.Http;

namespace HotJoes.Api.Vendor;

public sealed class VendorApiErrorMapper
{
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
            nameof(RegisterVendorCommand.OpeningHoursStartTime) =>
                "tradingCharacteristics.openingHours.startTime",
            nameof(RegisterVendorCommand.OpeningHoursEndTime) =>
                "tradingCharacteristics.openingHours.endTime",
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
