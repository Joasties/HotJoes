using System.Text.RegularExpressions;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class RegisterVendorCommandValidator
    : IRegisterVendorCommandValidator
{
    private const int MaximumTradingNameLength = 160;
    private const int MaximumLegalOperatorNameLength = 160;
    private const int MaximumContactNameLength = 100;
    private const int MaximumEmailLength = 254;
    private const int MaximumEmailLocalPartLength = 64;
    private const int MaximumEmailDomainLabelLength = 63;
    private const int MaximumBusinessDescriptionLength = 2000;

    private static readonly Regex CompanyRegistrationNumberPattern = new(
        @"^(?:[A-Za-z]{2})?\d{6,8}$",
        RegexOptions.CultureInvariant);

    private static readonly Regex TelephonePattern = new(
        @"^(?:(?:\+44|0)7\d{9}|(?:\+44|0)(?:1|2|3|5|8|9)\d{8,9})$",
        RegexOptions.CultureInvariant);

    public RegisterVendorCommandValidationResult Validate(
        RegisterVendorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var errors = new List<RegistrationValidationError>();

        ValidateRequiredBoundedText(
            command.TradingName,
            nameof(RegisterVendorCommand.TradingName),
            MaximumTradingNameLength,
            "Trading Name",
            errors);
        ValidateRequiredBoundedText(
            command.LegalOperatorName,
            nameof(RegisterVendorCommand.LegalOperatorName),
            MaximumLegalOperatorNameLength,
            "Legal Operator Name",
            errors);
        ValidateEnum(
            command.LegalOperatorType,
            nameof(RegisterVendorCommand.LegalOperatorType),
            "Legal Operator Type",
            errors);

        string? canonicalCompanyRegistrationNumber =
            ValidateCompanyRegistrationNumber(command, errors);

        ValidateEnum(
            command.TradingLocation,
            nameof(RegisterVendorCommand.TradingLocation),
            "Trading Location",
            errors);
        ValidateRequiredBoundedText(
            command.ContactName,
            nameof(RegisterVendorCommand.ContactName),
            MaximumContactNameLength,
            "Contact Name",
            errors);

        string? canonicalEmail = ValidateEmail(command.ContactEmail, errors);
        string? canonicalTelephone = ValidateTelephone(
            command.ContactTelephone,
            errors);

        ValidateRequiredText(
            command.AddressResolutionReference,
            nameof(RegisterVendorCommand.AddressResolutionReference),
            "Address Resolution Reference",
            errors);
        ValidateWebsite(command.Website, errors);
        ValidateBusinessDescription(command.BusinessDescription, errors);
        ValidateDeclaration(
            command.AuthorisedToRegisterBusiness,
            nameof(RegisterVendorCommand.AuthorisedToRegisterBusiness),
            "Authorised to Register Business",
            errors);
        ValidateDeclaration(
            command.InformationAccurate,
            nameof(RegisterVendorCommand.InformationAccurate),
            "Information Accurate",
            errors);
        ValidateDeclaration(
            command.AcceptHotJoesPlatformTerms,
            nameof(RegisterVendorCommand.AcceptHotJoesPlatformTerms),
            "Accept HotJoes Platform Terms",
            errors);

        if (errors.Count > 0)
        {
            return RegisterVendorCommandValidationResult.Invalid(errors);
        }

        return RegisterVendorCommandValidationResult.Accepted(
            CopyWithCanonicalValues(
                command,
                canonicalCompanyRegistrationNumber,
                canonicalEmail!,
                canonicalTelephone!));
    }

    private static void ValidateRequiredBoundedText(
        string value,
        string field,
        int maximumLength,
        string displayName,
        ICollection<RegistrationValidationError> errors)
    {
        if (!ValidateRequiredText(value, field, displayName, errors))
        {
            return;
        }

        if (value.Length > maximumLength)
        {
            errors.Add(
                new RegistrationValidationError(
                    field,
                    RegistrationValidationErrorCode.LengthOutOfRange,
                    $"{displayName} must contain between 1 and {maximumLength} characters."));
        }
    }

    private static bool ValidateRequiredText(
        string value,
        string field,
        string displayName,
        ICollection<RegistrationValidationError> errors)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return true;
        }

        errors.Add(
            new RegistrationValidationError(
                field,
                RegistrationValidationErrorCode.Required,
                $"{displayName} is required."));
        return false;
    }

    private static void ValidateEnum<TEnum>(
        TEnum value,
        string field,
        string displayName,
        ICollection<RegistrationValidationError> errors)
        where TEnum : struct, Enum
    {
        if (Enum.IsDefined(value))
        {
            return;
        }

        errors.Add(
            new RegistrationValidationError(
                field,
                RegistrationValidationErrorCode.InvalidValue,
                $"{displayName} must be a supported value."));
    }

    private static string? ValidateCompanyRegistrationNumber(
        RegisterVendorCommand command,
        ICollection<RegistrationValidationError> errors)
    {
        bool legalOperatorTypeIsDefined = Enum.IsDefined(
            command.LegalOperatorType);
        bool isRequired = legalOperatorTypeIsDefined &&
            RequiresCompanyRegistrationNumber(command.LegalOperatorType);
        bool isSupplied = !string.IsNullOrWhiteSpace(
            command.CompanyRegistrationNumber);

        if (legalOperatorTypeIsDefined && isRequired && !isSupplied)
        {
            errors.Add(
                new RegistrationValidationError(
                    nameof(RegisterVendorCommand.CompanyRegistrationNumber),
                    RegistrationValidationErrorCode.ConditionallyRequired,
                    "Company Registration Number is required for the selected Legal Operator Type."));
            return null;
        }

        if (legalOperatorTypeIsDefined && !isRequired && isSupplied)
        {
            errors.Add(
                new RegistrationValidationError(
                    nameof(RegisterVendorCommand.CompanyRegistrationNumber),
                    RegistrationValidationErrorCode.Prohibited,
                    "Company Registration Number is prohibited for the selected Legal Operator Type."));
            return null;
        }

        if (!isSupplied)
        {
            return null;
        }

        string suppliedValue = command.CompanyRegistrationNumber!;

        if (!CompanyRegistrationNumberPattern.IsMatch(suppliedValue))
        {
            errors.Add(
                new RegistrationValidationError(
                    nameof(RegisterVendorCommand.CompanyRegistrationNumber),
                    RegistrationValidationErrorCode.InvalidFormat,
                    "Company Registration Number must use the supported UK Companies House format."));
            return null;
        }

        return suppliedValue.ToUpperInvariant();
    }

    private static bool RequiresCompanyRegistrationNumber(
        LegalOperatorType legalOperatorType)
    {
        return legalOperatorType is
            LegalOperatorType.LimitedCompany or
            LegalOperatorType.LimitedLiabilityPartnership or
            LegalOperatorType.CharitableIncorporatedOrganisation;
    }

    private static string? ValidateEmail(
        string suppliedEmail,
        ICollection<RegistrationValidationError> errors)
    {
        if (string.IsNullOrEmpty(suppliedEmail))
        {
            errors.Add(
                new RegistrationValidationError(
                    nameof(RegisterVendorCommand.ContactEmail),
                    RegistrationValidationErrorCode.Required,
                    "Contact Email is required."));
            return null;
        }

        string email = suppliedEmail.Trim();
        int separatorIndex = email.IndexOf('@');

        if (email.Length > MaximumEmailLength ||
            separatorIndex <= 0 ||
            separatorIndex != email.LastIndexOf('@') ||
            separatorIndex == email.Length - 1)
        {
            AddInvalidEmailError(errors);
            return null;
        }

        string localPart = email[..separatorIndex];
        string domain = email[(separatorIndex + 1)..];

        if (!IsValidLocalPart(localPart) || !IsValidDomain(domain))
        {
            AddInvalidEmailError(errors);
            return null;
        }

        return localPart + "@" + domain.ToLowerInvariant();
    }

    private static bool IsValidLocalPart(string localPart)
    {
        return localPart.Length is >= 1 and <= MaximumEmailLocalPartLength &&
            localPart[0] != '.' &&
            localPart[^1] != '.' &&
            !localPart.Contains("..", StringComparison.Ordinal) &&
            localPart.All(IsPermittedLocalPartCharacter);
    }

    private static bool IsPermittedLocalPartCharacter(char character)
    {
        return IsAsciiLetterOrDigit(character) ||
            character is '.' or '!' or '#' or '$' or '%' or '&' or '\'' or
                '*' or '+' or '-' or '/' or '=' or '?' or '^' or '_' or '`' or
                '{' or '|' or '}' or '~';
    }

    private static bool IsValidDomain(string domain)
    {
        string[] labels = domain.Split('.');
        return labels.Length >= 2 && labels.All(IsValidDomainLabel);
    }

    private static bool IsValidDomainLabel(string label)
    {
        return label.Length is >= 1 and <= MaximumEmailDomainLabelLength &&
            label[0] != '-' &&
            label[^1] != '-' &&
            label.All(
                character => IsAsciiLetterOrDigit(character) || character == '-');
    }

    private static bool IsAsciiLetterOrDigit(char character)
    {
        return character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or
            >= '0' and <= '9';
    }

    private static void AddInvalidEmailError(
        ICollection<RegistrationValidationError> errors)
    {
        errors.Add(
            new RegistrationValidationError(
                nameof(RegisterVendorCommand.ContactEmail),
                RegistrationValidationErrorCode.InvalidFormat,
                "Contact Email must use the supported email format."));
    }

    private static string? ValidateTelephone(
        string suppliedTelephone,
        ICollection<RegistrationValidationError> errors)
    {
        if (string.IsNullOrEmpty(suppliedTelephone))
        {
            errors.Add(
                new RegistrationValidationError(
                    nameof(RegisterVendorCommand.ContactTelephone),
                    RegistrationValidationErrorCode.Required,
                    "Contact Telephone is required."));
            return null;
        }

        string telephone = suppliedTelephone.Trim();

        if (!HasPermittedTelephoneCharacters(telephone))
        {
            AddInvalidTelephoneError(errors);
            return null;
        }

        string normalized = string.Concat(
            telephone.Where(
                character => character is not (' ' or '-' or '(' or ')')));

        if (!TelephonePattern.IsMatch(normalized))
        {
            AddInvalidTelephoneError(errors);
            return null;
        }

        return normalized[0] == '0'
            ? "+44" + normalized[1..]
            : normalized;
    }

    private static bool HasPermittedTelephoneCharacters(string telephone)
    {
        for (int index = 0; index < telephone.Length; index++)
        {
            char character = telephone[index];

            if (character == '+')
            {
                if (index != 0)
                {
                    return false;
                }

                continue;
            }

            if (character is not (>= '0' and <= '9') and
                not ' ' and
                not '-' and
                not '(' and
                not ')')
            {
                return false;
            }
        }

        return true;
    }

    private static void AddInvalidTelephoneError(
        ICollection<RegistrationValidationError> errors)
    {
        errors.Add(
            new RegistrationValidationError(
                nameof(RegisterVendorCommand.ContactTelephone),
                RegistrationValidationErrorCode.InvalidFormat,
                "Contact Telephone must use the supported UK telephone format."));
    }

    private static void ValidateWebsite(
        string? website,
        ICollection<RegistrationValidationError> errors)
    {
        if (website is null)
        {
            return;
        }

        if (Uri.TryCreate(website, UriKind.Absolute, out Uri? uri) &&
            uri.Scheme == Uri.UriSchemeHttps &&
            !string.IsNullOrEmpty(uri.Host))
        {
            return;
        }

        errors.Add(
            new RegistrationValidationError(
                nameof(RegisterVendorCommand.Website),
                RegistrationValidationErrorCode.InvalidFormat,
                "Website must be a valid HTTPS URL."));
    }

    private static void ValidateBusinessDescription(
        string? businessDescription,
        ICollection<RegistrationValidationError> errors)
    {
        if (businessDescription is null ||
            businessDescription.Length <= MaximumBusinessDescriptionLength)
        {
            return;
        }

        errors.Add(
            new RegistrationValidationError(
                nameof(RegisterVendorCommand.BusinessDescription),
                RegistrationValidationErrorCode.LengthOutOfRange,
                $"Business Description must contain no more than {MaximumBusinessDescriptionLength} characters."));
    }

    private static void ValidateDeclaration(
        bool accepted,
        string field,
        string displayName,
        ICollection<RegistrationValidationError> errors)
    {
        if (accepted)
        {
            return;
        }

        errors.Add(
            new RegistrationValidationError(
                field,
                RegistrationValidationErrorCode.InvalidValue,
                $"{displayName} must be explicitly accepted."));
    }

    private static RegisterVendorCommand CopyWithCanonicalValues(
        RegisterVendorCommand command,
        string? canonicalCompanyRegistrationNumber,
        string canonicalEmail,
        string canonicalTelephone)
    {
        return new RegisterVendorCommand(
            command.TradingName,
            command.LegalOperatorName,
            command.LegalOperatorType,
            canonicalCompanyRegistrationNumber,
            command.TradingLocation,
            command.OpeningHoursStartTime,
            command.OpeningHoursEndTime,
            command.ServiceIncludesHotFood,
            command.AlcoholService,
            command.ContactName,
            canonicalEmail,
            canonicalTelephone,
            command.AddressResolutionReference,
            command.Website,
            command.BusinessDescription,
            command.AuthorisedToRegisterBusiness,
            command.InformationAccurate,
            command.AcceptHotJoesPlatformTerms);
    }
}
