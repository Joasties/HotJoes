using System.Text.RegularExpressions;

namespace HotJoes.Domain.Vendor;

public sealed record CompanyRegistrationNumber
{
    private static readonly Regex ValidFormat = new(
        "^(?:[A-Za-z]{2})?\\d{6,8}$",
        RegexOptions.CultureInvariant);

    public CompanyRegistrationNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ValidFormat.IsMatch(value))
        {
            throw new ArgumentException("Invalid Company Registration Number.", nameof(value));
        }

        Value = value.ToUpperInvariant();
    }

    public string Value { get; }
}
