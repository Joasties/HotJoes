using System.Text.Json;
using HotJoes.Api.Vendor;
using HotJoes.Application.Vendor;
using Microsoft.AspNetCore.Http;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorApiErrorMapperTests
{
    public static TheoryData<RegisterVendorResult, int, string>
        RegisterFailureMappings => new()
        {
            {
                RegisterVendorResult.ReferenceIsInvalid(),
                StatusCodes.Status400BadRequest,
                "invalidAddressReference"
            },
            {
                RegisterVendorResult.AddressResultIsInvalid(),
                StatusCodes.Status400BadRequest,
                "invalidAddressResult"
            },
            {
                RegisterVendorResult.AggregateInvariantFailed(),
                StatusCodes.Status400BadRequest,
                "aggregateInvariantFailed"
            },
            {
                RegisterVendorResult.IdempotencyConflictDetected(),
                StatusCodes.Status409Conflict,
                "idempotencyConflict"
            },
            {
                RegisterVendorResult.AddressServiceIsTemporarilyUnavailable(),
                StatusCodes.Status503ServiceUnavailable,
                "addressServiceTemporarilyUnavailable"
            },
            {
                RegisterVendorResult.PersistenceOrAtomicRecordingFailed(),
                StatusCodes.Status503ServiceUnavailable,
                "persistenceOrAtomicRecordingFailed"
            }
        };

    public static TheoryData<string, string> ValidationFieldMappings => new()
    {
        { nameof(RegisterVendorCommand.TradingName), "tradingName" },
        { nameof(RegisterVendorCommand.LegalOperatorName), "legalOperatorName" },
        { nameof(RegisterVendorCommand.LegalOperatorType), "legalOperatorType" },
        { nameof(RegisterVendorCommand.CompanyRegistrationNumber), "companyRegistrationNumber" },
        { nameof(RegisterVendorCommand.TradingLocation), "tradingCharacteristics.tradingLocation" },
        { nameof(RegisterVendorCommand.OpeningHoursStartTime), "tradingCharacteristics.openingHours.startTime" },
        { nameof(RegisterVendorCommand.OpeningHoursEndTime), "tradingCharacteristics.openingHours.endTime" },
        { nameof(RegisterVendorCommand.ServiceIncludesHotFood), "tradingCharacteristics.serviceIncludesHotFood" },
        { nameof(RegisterVendorCommand.AlcoholService), "tradingCharacteristics.alcoholService" },
        { nameof(RegisterVendorCommand.ContactName), "primaryContact.contactName" },
        { nameof(RegisterVendorCommand.ContactEmail), "primaryContact.contactEmail" },
        { nameof(RegisterVendorCommand.ContactTelephone), "primaryContact.contactTelephone" },
        { nameof(RegisterVendorCommand.AddressResolutionReference), "addressResolutionReference" },
        { nameof(RegisterVendorCommand.Website), "website" },
        { nameof(RegisterVendorCommand.BusinessDescription), "businessDescription" },
        { nameof(RegisterVendorCommand.AuthorisedToRegisterBusiness), "registrationDeclarations.authorisedToRegisterBusiness" },
        { nameof(RegisterVendorCommand.InformationAccurate), "registrationDeclarations.informationAccurate" },
        { nameof(RegisterVendorCommand.AcceptHotJoesPlatformTerms), "registrationDeclarations.acceptHotJoesPlatformTerms" }
    };

    [Theory]
    [MemberData(nameof(RegisterFailureMappings))]
    public void Map_RegisterFailure_ReturnsExactStatusCodeAndSafeEnvelope(
        RegisterVendorResult result,
        int expectedStatusCode,
        string expectedCode)
    {
        VendorApiErrorMapping mapping = new VendorApiErrorMapper().Map(result);

        Assert.Equal(expectedStatusCode, mapping.StatusCode);
        Assert.Equal(expectedCode, mapping.Response.Code);
        Assert.False(string.IsNullOrWhiteSpace(mapping.Response.Message));
        Assert.Null(mapping.Response.ValidationErrors);
        Assert.DoesNotContain("Exception", mapping.Response.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Npgsql", mapping.Response.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Map_RequestValidationFailure_ReturnsOneCompleteValidationEnvelope()
    {
        RegistrationValidationError[] errors =
        [
            new(
                nameof(RegisterVendorCommand.TradingName),
                RegistrationValidationErrorCode.Required,
                "Trading Name is required."),
            new(
                nameof(RegisterVendorCommand.InformationAccurate),
                RegistrationValidationErrorCode.InvalidValue,
                "Information Accurate must be accepted."),
            new(
                nameof(RegisterVendorCommand.CompanyRegistrationNumber),
                RegistrationValidationErrorCode.ConditionallyRequired,
                "Company Registration Number is required.")
        ];

        VendorApiErrorMapping mapping = new VendorApiErrorMapper().Map(
            RegisterVendorResult.RequestValidationFailed(errors));

        Assert.Equal(StatusCodes.Status400BadRequest, mapping.StatusCode);
        Assert.Equal("registrationValidationFailed", mapping.Response.Code);
        Assert.NotNull(mapping.Response.ValidationErrors);
        Assert.Collection(
            mapping.Response.ValidationErrors,
            error => AssertValidationError(
                error,
                "tradingName",
                "required",
                "Trading Name is required."),
            error => AssertValidationError(
                error,
                "registrationDeclarations.informationAccurate",
                "invalidValue",
                "Information Accurate must be accepted."),
            error => AssertValidationError(
                error,
                "companyRegistrationNumber",
                "conditionallyRequired",
                "Company Registration Number is required."));
    }

    [Theory]
    [MemberData(nameof(ValidationFieldMappings))]
    public void Map_ValidationError_TranslatesApplicationFieldToApiJsonPath(
        string applicationField,
        string expectedJsonPath)
    {
        RegisterVendorResult result = RegisterVendorResult.RequestValidationFailed(
        [
            new RegistrationValidationError(
                applicationField,
                RegistrationValidationErrorCode.Prohibited,
                "The supplied value is prohibited.")
        ]);

        VendorApiErrorMapping mapping = new VendorApiErrorMapper().Map(result);

        VendorApiValidationErrorResponse error = Assert.Single(
            Assert.IsAssignableFrom<IReadOnlyList<VendorApiValidationErrorResponse>>(
                mapping.Response.ValidationErrors));
        Assert.Equal(expectedJsonPath, error.Field);
    }

    [Theory]
    [InlineData(RegistrationValidationErrorCode.Required, "required")]
    [InlineData(RegistrationValidationErrorCode.InvalidFormat, "invalidFormat")]
    [InlineData(RegistrationValidationErrorCode.LengthOutOfRange, "lengthOutOfRange")]
    [InlineData(RegistrationValidationErrorCode.InvalidValue, "invalidValue")]
    [InlineData(RegistrationValidationErrorCode.ConditionallyRequired, "conditionallyRequired")]
    [InlineData(RegistrationValidationErrorCode.Prohibited, "prohibited")]
    public void Map_ValidationError_UsesOnlyApprovedStableEntryCode(
        RegistrationValidationErrorCode applicationCode,
        string expectedApiCode)
    {
        RegisterVendorResult result = RegisterVendorResult.RequestValidationFailed(
        [
            new RegistrationValidationError(
                nameof(RegisterVendorCommand.TradingName),
                applicationCode,
                "Client-safe explanation.")
        ]);

        VendorApiErrorMapping mapping = new VendorApiErrorMapper().Map(result);

        Assert.Equal(
            expectedApiCode,
            Assert.Single(mapping.Response.ValidationErrors!).Code);
    }

    [Fact]
    public void Map_VendorNotFound_ReturnsApprovedNotFoundEnvelope()
    {
        VendorApiErrorMapping mapping = new VendorApiErrorMapper().Map(
            RetrieveRegisteredVendorResult.VendorNotFound());

        Assert.Equal(StatusCodes.Status404NotFound, mapping.StatusCode);
        Assert.Equal("vendorNotFound", mapping.Response.Code);
        Assert.Null(mapping.Response.ValidationErrors);
    }

    [Fact]
    public void MalformedRequest_ReturnsApprovedBadRequestEnvelope()
    {
        VendorApiErrorMapping mapping =
            new VendorApiErrorMapper().MalformedRequest();

        Assert.Equal(StatusCodes.Status400BadRequest, mapping.StatusCode);
        Assert.Equal("requestMalformed", mapping.Response.Code);
        Assert.Null(mapping.Response.ValidationErrors);
    }

    [Fact]
    public void Serialize_NonValidationFailure_AlwaysIncludesExplicitNullValidationErrors()
    {
        VendorApiErrorMapping mapping = new VendorApiErrorMapper().Map(
            RegisterVendorResult.ReferenceIsInvalid());

        string json = JsonSerializer.Serialize(
            mapping.Response,
            VendorApiJsonOptions.Create());

        using JsonDocument document = JsonDocument.Parse(json);
        Assert.Equal(
            JsonValueKind.Null,
            document.RootElement.GetProperty("validationErrors").ValueKind);
    }

    [Fact]
    public void MappingSurface_ContainsNoSupersededValidationOrUnprocessableEntityCode()
    {
        var mapper = new VendorApiErrorMapper();
        string[] topLevelCodes =
        [
            mapper.Map(
                RegisterVendorResult.RequestValidationFailed(
                [
                    new RegistrationValidationError(
                        nameof(RegisterVendorCommand.TradingName),
                        RegistrationValidationErrorCode.Required,
                        "Trading Name is required.")
                ])).Response.Code,
            mapper.MalformedRequest().Response.Code
        ];
        int[] mappedStatuses =
        [
            mapper.MalformedRequest().StatusCode,
            mapper.Map(RegisterVendorResult.ReferenceIsInvalid()).StatusCode,
            mapper.Map(RegisterVendorResult.IdempotencyConflictDetected()).StatusCode,
            mapper.Map(RegisterVendorResult.AddressServiceIsTemporarilyUnavailable()).StatusCode,
            mapper.Map(RetrieveRegisteredVendorResult.VendorNotFound()).StatusCode
        ];

        Assert.DoesNotContain("registrationDeclarationFailed", topLevelCodes);
        Assert.DoesNotContain("conditionalRuleFailed", topLevelCodes);
        Assert.DoesNotContain("unprocessableEntity", topLevelCodes);
        Assert.DoesNotContain(StatusCodes.Status422UnprocessableEntity, mappedStatuses);
    }

    private static void AssertValidationError(
        VendorApiValidationErrorResponse actual,
        string expectedField,
        string expectedCode,
        string expectedMessage)
    {
        Assert.Equal(expectedField, actual.Field);
        Assert.Equal(expectedCode, actual.Code);
        Assert.Equal(expectedMessage, actual.Message);
    }
}
