namespace HotJoes.Application.Address;

public sealed class StubAddressApplication : IAddressResolutionService
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
