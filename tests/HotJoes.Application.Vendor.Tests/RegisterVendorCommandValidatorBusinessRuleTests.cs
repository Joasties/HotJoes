using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorCommandValidatorBusinessRuleTests
{
    [Theory]
    [InlineData("TradingName")]
    [InlineData("LegalOperatorName")]
    [InlineData("ContactName")]
    [InlineData("AddressResolutionReference")]
    public void Validate_WhenRequiredTextIsEmpty_ReturnsRequired(
        string field)
    {
        var validator = new RegisterVendorCommandValidator();
        RegisterVendorCommand command = field switch
        {
            nameof(RegisterVendorCommand.TradingName) =>
                CreateCommand(tradingName: string.Empty),
            nameof(RegisterVendorCommand.LegalOperatorName) =>
                CreateCommand(legalOperatorName: string.Empty),
            nameof(RegisterVendorCommand.ContactName) =>
                CreateCommand(contactName: string.Empty),
            nameof(RegisterVendorCommand.AddressResolutionReference) =>
                CreateCommand(addressResolutionReference: string.Empty),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        RegisterVendorCommandValidationResult result = validator.Validate(command);

        AssertSingleError(
            result,
            field,
            RegistrationValidationErrorCode.Required);
    }

    [Fact]
    public void Validate_WhenFieldDeclarationAndConditionalRulesFail_ReturnsOneCompleteFailure()
    {
        var validator = new RegisterVendorCommandValidator();
        RegisterVendorCommand command = CreateCommand(
            tradingName: string.Empty,
            legalOperatorType: LegalOperatorType.LimitedCompany,
            companyRegistrationNumber: null,
            contactName: string.Empty,
            website: "http://hotjoes.example",
            businessDescription: new string('x', 2001),
            authorisedToRegisterBusiness: false,
            informationAccurate: false,
            acceptHotJoesPlatformTerms: false);

        RegisterVendorCommandValidationResult result = validator.Validate(command);

        var failure = Assert.IsType<RegisterVendorCommandValidationResult.Failure>(
            result);
        AssertErrorsContain(
            failure,
            (nameof(RegisterVendorCommand.TradingName), RegistrationValidationErrorCode.Required),
            (nameof(RegisterVendorCommand.CompanyRegistrationNumber), RegistrationValidationErrorCode.ConditionallyRequired),
            (nameof(RegisterVendorCommand.ContactName), RegistrationValidationErrorCode.Required),
            (nameof(RegisterVendorCommand.Website), RegistrationValidationErrorCode.InvalidFormat),
            (nameof(RegisterVendorCommand.BusinessDescription), RegistrationValidationErrorCode.LengthOutOfRange),
            (nameof(RegisterVendorCommand.AuthorisedToRegisterBusiness), RegistrationValidationErrorCode.InvalidValue),
            (nameof(RegisterVendorCommand.InformationAccurate), RegistrationValidationErrorCode.InvalidValue),
            (nameof(RegisterVendorCommand.AcceptHotJoesPlatformTerms), RegistrationValidationErrorCode.InvalidValue));
    }

    [Theory]
    [InlineData(LegalOperatorType.LimitedCompany)]
    [InlineData(LegalOperatorType.LimitedLiabilityPartnership)]
    [InlineData(LegalOperatorType.CharitableIncorporatedOrganisation)]
    public void Validate_WhenLegalOperatorRequiresCrnAndItIsAbsent_ReturnsConditionallyRequired(
        LegalOperatorType legalOperatorType)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(
                legalOperatorType: legalOperatorType,
                companyRegistrationNumber: null));

        AssertSingleError(
            result,
            nameof(RegisterVendorCommand.CompanyRegistrationNumber),
            RegistrationValidationErrorCode.ConditionallyRequired);
    }

    [Theory]
    [InlineData(LegalOperatorType.SoleTrader)]
    [InlineData(LegalOperatorType.GeneralPartnership)]
    [InlineData(LegalOperatorType.CharitableCommunityGroup)]
    public void Validate_WhenLegalOperatorProhibitsCrnAndItIsSupplied_ReturnsProhibited(
        LegalOperatorType legalOperatorType)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(
                legalOperatorType: legalOperatorType,
                companyRegistrationNumber: "12345678"));

        AssertSingleError(
            result,
            nameof(RegisterVendorCommand.CompanyRegistrationNumber),
            RegistrationValidationErrorCode.Prohibited);
    }

    [Theory]
    [InlineData("123456", "123456")]
    [InlineData("12345678", "12345678")]
    [InlineData("ab123456", "AB123456")]
    public void Validate_WhenCrnIsValid_ReturnsCanonicalUppercaseCrn(
        string suppliedCrn,
        string expectedCrn)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(companyRegistrationNumber: suppliedCrn));

        var success = Assert.IsType<RegisterVendorCommandValidationResult.Success>(
            result);
        Assert.Equal(expectedCrn, success.Command.CompanyRegistrationNumber);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("ABC123456")]
    [InlineData("AB12345")]
    [InlineData("AB123456789")]
    [InlineData("12-345678")]
    public void Validate_WhenRequiredCrnFormatIsInvalid_ReturnsInvalidFormat(
        string suppliedCrn)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(companyRegistrationNumber: suppliedCrn));

        AssertSingleError(
            result,
            nameof(RegisterVendorCommand.CompanyRegistrationNumber),
            RegistrationValidationErrorCode.InvalidFormat);
    }

    [Theory]
    [InlineData("TradingName", 161)]
    [InlineData("LegalOperatorName", 161)]
    [InlineData("ContactName", 101)]
    public void Validate_WhenBoundedRequiredTextExceedsMaximum_ReturnsLengthOutOfRange(
        string field,
        int length)
    {
        var validator = new RegisterVendorCommandValidator();
        string value = new('x', length);
        RegisterVendorCommand command = field switch
        {
            nameof(RegisterVendorCommand.TradingName) =>
                CreateCommand(tradingName: value),
            nameof(RegisterVendorCommand.LegalOperatorName) =>
                CreateCommand(legalOperatorName: value),
            nameof(RegisterVendorCommand.ContactName) =>
                CreateCommand(contactName: value),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        RegisterVendorCommandValidationResult result = validator.Validate(command);

        AssertSingleError(
            result,
            field,
            RegistrationValidationErrorCode.LengthOutOfRange);
    }

    [Fact]
    public void Validate_WhenControlledValuesAreUndefined_ReturnsBothInvalidValueErrors()
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(
                legalOperatorType: (LegalOperatorType)999,
                tradingLocation: (TradingLocation)999));

        var failure = Assert.IsType<RegisterVendorCommandValidationResult.Failure>(
            result);
        AssertErrorsContain(
            failure,
            (nameof(RegisterVendorCommand.LegalOperatorType), RegistrationValidationErrorCode.InvalidValue),
            (nameof(RegisterVendorCommand.TradingLocation), RegistrationValidationErrorCode.InvalidValue));
    }

    [Fact]
    public void Validate_WhenOptionalValuesAreAbsentAndHoursAreOvernight_ReturnsAcceptedCommand()
    {
        var validator = new RegisterVendorCommandValidator();
        RegisterVendorCommand command = CreateCommand(
            openingHoursStartTime: new TimeOnly(23, 0),
            openingHoursEndTime: new TimeOnly(5, 0),
            website: null,
            businessDescription: null);

        RegisterVendorCommandValidationResult result = validator.Validate(command);

        var success = Assert.IsType<RegisterVendorCommandValidationResult.Success>(
            result);
        Assert.Equal(new TimeOnly(23, 0), success.Command.OpeningHoursStartTime);
        Assert.Equal(new TimeOnly(5, 0), success.Command.OpeningHoursEndTime);
        Assert.Null(success.Command.Website);
        Assert.Null(success.Command.BusinessDescription);
    }

    [Fact]
    public void Validate_WhenNamesContainSurroundingWhitespace_PreservesRegisteredDisplayValues()
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(
                tradingName: "  Hot Joes Greenwich  ",
                legalOperatorName: "  Hot Joes Limited  "));

        var success = Assert.IsType<RegisterVendorCommandValidationResult.Success>(
            result);
        Assert.Equal("  Hot Joes Greenwich  ", success.Command.TradingName);
        Assert.Equal("  Hot Joes Limited  ", success.Command.LegalOperatorName);
    }

    private static void AssertSingleError(
        RegisterVendorCommandValidationResult result,
        string field,
        RegistrationValidationErrorCode code)
    {
        var failure = Assert.IsType<RegisterVendorCommandValidationResult.Failure>(
            result);
        RegistrationValidationError error = Assert.Single(failure.Errors);
        Assert.Equal(field, error.Field);
        Assert.Equal(code, error.Code);
    }

    private static void AssertErrorsContain(
        RegisterVendorCommandValidationResult.Failure failure,
        params (string Field, RegistrationValidationErrorCode Code)[] expected)
    {
        Assert.Equal(expected.Length, failure.Errors.Count);
        Assert.All(
            expected,
            item => Assert.Contains(
                failure.Errors,
                error => error.Field == item.Field && error.Code == item.Code));
    }

    private static RegisterVendorCommand CreateCommand(
        string tradingName = "Hot Joes Greenwich",
        string legalOperatorName = "Hot Joes Limited",
        LegalOperatorType legalOperatorType = LegalOperatorType.LimitedCompany,
        string? companyRegistrationNumber = "12345678",
        TradingLocation tradingLocation = TradingLocation.Stall,
        TimeOnly? openingHoursStartTime = null,
        TimeOnly? openingHoursEndTime = null,
        string contactName = "Joseph Bloggs",
        string addressResolutionReference = "address-resolution-reference-001",
        string? website = "https://hotjoes.example",
        string? businessDescription = "Hot food from our Greenwich market stall.",
        bool authorisedToRegisterBusiness = true,
        bool informationAccurate = true,
        bool acceptHotJoesPlatformTerms = true)
    {
        return new RegisterVendorCommand(
            tradingName,
            legalOperatorName,
            legalOperatorType,
            companyRegistrationNumber,
            tradingLocation,
            openingHoursStartTime ?? new TimeOnly(9, 0),
            openingHoursEndTime ?? new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            contactName: contactName,
            contactEmail: "Jordan.Smith@example.test",
            contactTelephone: "+442071234567",
            addressResolutionReference: addressResolutionReference,
            website: website,
            businessDescription: businessDescription,
            authorisedToRegisterBusiness: authorisedToRegisterBusiness,
            informationAccurate: informationAccurate,
            acceptHotJoesPlatformTerms: acceptHotJoesPlatformTerms);
    }
}
