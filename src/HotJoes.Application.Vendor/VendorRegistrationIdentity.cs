using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class VendorRegistrationIdentity : IEquatable<VendorRegistrationIdentity>
{
    private VendorRegistrationIdentity(
        string normalizedTradingName,
        string normalizedLegalOperatorName,
        CanonicalAddressId canonicalAddressId)
    {
        NormalizedTradingName = normalizedTradingName;
        NormalizedLegalOperatorName = normalizedLegalOperatorName;
        CanonicalAddressId = canonicalAddressId;
    }

    public string NormalizedTradingName { get; }

    public string NormalizedLegalOperatorName { get; }

    public CanonicalAddressId CanonicalAddressId { get; }

    public static VendorRegistrationIdentity Create(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(addressValues);

        return new VendorRegistrationIdentity(
            command.TradingName.Trim(),
            command.LegalOperatorName.Trim(),
            addressValues.CanonicalAddressId);
    }

    public bool Equals(VendorRegistrationIdentity? other)
    {
        return other is not null
            && StringComparer.OrdinalIgnoreCase.Equals(
                NormalizedTradingName,
                other.NormalizedTradingName)
            && StringComparer.OrdinalIgnoreCase.Equals(
                NormalizedLegalOperatorName,
                other.NormalizedLegalOperatorName)
            && CanonicalAddressId == other.CanonicalAddressId;
    }

    public override bool Equals(object? obj)
    {
        return obj is VendorRegistrationIdentity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(NormalizedTradingName),
            StringComparer.OrdinalIgnoreCase.GetHashCode(NormalizedLegalOperatorName),
            CanonicalAddressId);
    }
}
