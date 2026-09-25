namespace HotJoes.Application.Compliance;

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
