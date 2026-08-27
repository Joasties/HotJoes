using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorCommandValidatorContactTests
{
    [Theory]
    [MemberData(nameof(ValidEmailCases))]
    public void Validate_WhenContactEmailMatchesApprovedProfile_ReturnsCanonicalEmail(
        string suppliedEmail,
        string expectedEmail)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(suppliedEmail, "+442071234567"));

        var success = Assert.IsType<RegisterVendorCommandValidationResult.Success>(
            result);
        Assert.Equal(expectedEmail, success.Command.ContactEmail);
    }

    [Theory]
    [MemberData(nameof(InvalidEmailCases))]
    public void Validate_WhenContactEmailViolatesApprovedProfile_ReturnsInvalidFormat(
        string suppliedEmail)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand(suppliedEmail, "+442071234567"));

        var failure = Assert.IsType<RegisterVendorCommandValidationResult.Failure>(
            result);
        RegistrationValidationError error = Assert.Single(failure.Errors);
        Assert.Equal(nameof(RegisterVendorCommand.ContactEmail), error.Field);
        Assert.Equal(RegistrationValidationErrorCode.InvalidFormat, error.Code);
    }

    [Theory]
    [MemberData(nameof(ValidTelephoneCases))]
    public void Validate_WhenContactTelephoneMatchesApprovedProfile_ReturnsCanonicalTelephone(
        string suppliedTelephone,
        string expectedTelephone)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand("Jordan.Smith@example.test", suppliedTelephone));

        var success = Assert.IsType<RegisterVendorCommandValidationResult.Success>(
            result);
        Assert.Equal(expectedTelephone, success.Command.ContactTelephone);
    }

    [Theory]
    [MemberData(nameof(InvalidTelephoneCases))]
    public void Validate_WhenContactTelephoneViolatesApprovedProfile_ReturnsInvalidFormat(
        string suppliedTelephone)
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand("Jordan.Smith@example.test", suppliedTelephone));

        var failure = Assert.IsType<RegisterVendorCommandValidationResult.Failure>(
            result);
        RegistrationValidationError error = Assert.Single(failure.Errors);
        Assert.Equal(nameof(RegisterVendorCommand.ContactTelephone), error.Field);
        Assert.Equal(RegistrationValidationErrorCode.InvalidFormat, error.Code);
    }

    [Fact]
    public void Validate_WhenEmailAndTelephoneAreBothInvalid_ReturnsBothErrors()
    {
        var validator = new RegisterVendorCommandValidator();

        RegisterVendorCommandValidationResult result = validator.Validate(
            CreateCommand("invalid-email", "44 7123 456789"));

        var failure = Assert.IsType<RegisterVendorCommandValidationResult.Failure>(
            result);
        Assert.Equal(2, failure.Errors.Count);
        Assert.Contains(
            failure.Errors,
            error => error.Field == nameof(RegisterVendorCommand.ContactEmail));
        Assert.Contains(
            failure.Errors,
            error => error.Field == nameof(RegisterVendorCommand.ContactTelephone));
    }

    public static IEnumerable<object[]> ValidEmailCases()
    {
        yield return ["Jordan.Smith@example.test", "Jordan.Smith@example.test"];
        yield return ["  Jordan.Smith@Example.TEST  ", "Jordan.Smith@example.test"];
        yield return ["a@example.test", "a@example.test"];
        yield return [new string('a', 64) + "@example.test", new string('a', 64) + "@example.test"];
        yield return ["a!#$%&'*+-/=?^_`{|}~z@example.test", "a!#$%&'*+-/=?^_`{|}~z@example.test"];

        string maximumDomain =
            new string('a', 63) + "." +
            new string('b', 63) + "." +
            new string('c', 61);
        string maximumEmail = new string('d', 64) + "@" + maximumDomain;
        yield return [maximumEmail, maximumEmail];
    }

    public static IEnumerable<object[]> InvalidEmailCases()
    {
        yield return ["plain-address"];
        yield return ["two@@example.test"];
        yield return ["@example.test"];
        yield return [new string('a', 65) + "@example.test"];
        yield return [".local@example.test"];
        yield return ["local.@example.test"];
        yield return ["local..part@example.test"];
        yield return ["local@localhost"];
        yield return ["local@-example.test"];
        yield return ["local@example-.test"];
        yield return ["local@exam_ple.test"];
        yield return ["local@" + new string('a', 64) + ".test"];
        yield return ["Display Name <local@example.test>"];
        yield return ["local(comment)@example.test"];
        yield return ["\"local\"@example.test"];
        yield return ["local@[127.0.0.1]"];
        yield return ["löcal@example.test"];

        string overMaximumDomain =
            new string('a', 63) + "." +
            new string('b', 63) + "." +
            new string('c', 62);
        yield return [new string('d', 64) + "@" + overMaximumDomain];
    }

    public static TheoryData<string, string> ValidTelephoneCases =>
        new()
        {
            { "07123 456789", "+447123456789" },
            { "07123-456789", "+447123456789" },
            { "+44 7123 456789", "+447123456789" },
            { "(020) 7123 4567", "+442071234567" },
            { "0161 123 4567", "+441611234567" },
            { "0800 123 4567", "+448001234567" },
            { "  07123 456789  ", "+447123456789" }
        };

    public static TheoryData<string> InvalidTelephoneCases =>
        new()
        {
            "44 7123 456789",
            "020+71234567",
            "+44+7123456789",
            "0044 7123 456789",
            "07123 45678",
            "07123 4567890",
            "04123 456789",
            "06123 456789",
            "999",
            "07123 45678A",
            "07123.456789"
        };

    private static RegisterVendorCommand CreateCommand(
        string contactEmail,
        string contactTelephone)
    {
        return new RegisterVendorCommand(
            tradingName: "Hot Joes Greenwich",
            legalOperatorName: "Hot Joes Limited",
            legalOperatorType: LegalOperatorType.LimitedCompany,
            companyRegistrationNumber: "12345678",
            tradingLocation: TradingLocation.Stall,
            openingHoursStartTime: new TimeOnly(9, 0),
            openingHoursEndTime: new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            contactName: "Joseph Bloggs",
            contactEmail: contactEmail,
            contactTelephone: contactTelephone,
            addressResolutionReference: "address-resolution-reference-001",
            website: "https://hotjoes.example",
            businessDescription: "Hot food from our Greenwich market stall.",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }
}
