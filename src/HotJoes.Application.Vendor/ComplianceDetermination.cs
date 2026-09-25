namespace HotJoes.Application.Vendor;

public sealed class ComplianceDetermination
{
    private readonly IReadOnlyList<ComplianceDeterminationItem> _items;

    public ComplianceDetermination(
        string ruleSetVersion,
        IEnumerable<ComplianceDeterminationItem> items)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruleSetVersion);
        ArgumentNullException.ThrowIfNull(items);

        ComplianceDeterminationItem[] copiedItems = items.ToArray();
        if (copiedItems.Any(item => item is null))
        {
            throw new ArgumentException(
                "Compliance Determination items cannot contain null.",
                nameof(items));
        }

        RequiredLicenceType[] canonicalOrder =
            Enum.GetValues<RequiredLicenceType>();
        if (!copiedItems
            .Select(item => item.RequiredLicenceType)
            .SequenceEqual(canonicalOrder))
        {
            throw new ArgumentException(
                "Compliance Determination must contain every Required Licence Type exactly once in canonical order.",
                nameof(items));
        }

        RuleSetVersion = ruleSetVersion;
        _items = Array.AsReadOnly(copiedItems);
    }

    public string RuleSetVersion { get; }
    public IReadOnlyList<ComplianceDeterminationItem> Items => _items;
}
