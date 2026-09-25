namespace HotJoes.Application.Address;

public sealed class StubAddressApplication
    : IAddressResolutionService, IAddressSearchService
{
    private readonly Dictionary<string, BoundResolution> _boundResults =
        new(StringComparer.Ordinal);
    private readonly Dictionary<string, CompleteAddressResult> _currentAddressData =
        new(StringComparer.Ordinal);

    public string SelectAddress(
        TradingLocation tradingLocation,
        CompleteAddressResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var reference = CreateReference();
        _boundResults.Add(
            reference,
            new ValidBoundResolution(tradingLocation, result));
        _currentAddressData[result.CanonicalAddressId] = result;

        return reference;
    }

    public void AddKnownResult(
        string reference,
        TradingLocation tradingLocation,
        CompleteAddressResult result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        ArgumentNullException.ThrowIfNull(result);

        _boundResults.Add(
            reference,
            new ValidBoundResolution(tradingLocation, result));
        _currentAddressData[result.CanonicalAddressId] = result;
    }

    public string AddKnownResultWithoutCanonicalAddressId(
        TradingLocation tradingLocation)
    {
        var reference = CreateReference();
        _boundResults.Add(reference, new InvalidBoundResolution(tradingLocation));

        return reference;
    }

    public void SimulateCurrentAddressDataChange(
        string canonicalAddressId,
        CompleteAddressResult result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(canonicalAddressId);
        ArgumentNullException.ThrowIfNull(result);

        if (!_currentAddressData.ContainsKey(canonicalAddressId))
        {
            throw new InvalidOperationException(
                "The canonical Address identifier has not been configured.");
        }

        _currentAddressData[canonicalAddressId] = result;
    }

    public AddressResolutionResult ResolveAddress(
        string addressResolutionReference,
        TradingLocation tradingLocation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(addressResolutionReference);

        if (!_boundResults.TryGetValue(addressResolutionReference, out var boundResult))
        {
            return AddressResolutionResult.ReferenceIsInvalid();
        }

        if (boundResult.TradingLocation != tradingLocation)
        {
            return AddressResolutionResult.InvalidAddress();
        }

        return boundResult switch
        {
            ValidBoundResolution valid =>
                AddressResolutionResult.Succeeded(valid.Result),
            InvalidBoundResolution =>
                AddressResolutionResult.InvalidAddress(),
            _ => throw new InvalidOperationException(
                "The configured Address Resolution scenario is unsupported.")
        };
    }

    public IReadOnlyList<AddressSearchCandidate> Search(
        string query,
        TradingLocation tradingLocation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        string term = query.Trim();
        return _boundResults
            .Where(pair =>
                pair.Value is ValidBoundResolution valid
                && valid.TradingLocation == tradingLocation
                && DisplayLines(valid.Result).Any(line =>
                    line.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Select(pair =>
            {
                var valid = (ValidBoundResolution)pair.Value;
                return new AddressSearchCandidate(
                    pair.Key,
                    DisplayLines(valid.Result));
            })
            .OrderBy(candidate => candidate.DisplayLines[0], StringComparer.Ordinal)
            .ThenBy(candidate => candidate.AddressResolutionReference, StringComparer.Ordinal)
            .ToArray();
    }

    private static IReadOnlyList<string> DisplayLines(CompleteAddressResult result) =>
        new[]
        {
            result.AddressLine1,
            result.AddressLine2,
            result.AddressLine3,
            result.AddressLine4,
            result.PostTown,
            result.Postcode,
            result.County
        }
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line!)
        .ToArray();

    private static string CreateReference()
    {
        return Guid.NewGuid().ToString("N");
    }

    private abstract record BoundResolution(TradingLocation TradingLocation);

    private sealed record ValidBoundResolution(
        TradingLocation TradingLocation,
        CompleteAddressResult Result)
        : BoundResolution(TradingLocation);

    private sealed record InvalidBoundResolution(TradingLocation TradingLocation)
        : BoundResolution(TradingLocation);
}
