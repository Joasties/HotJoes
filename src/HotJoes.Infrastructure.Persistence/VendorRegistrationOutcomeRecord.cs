namespace HotJoes.Infrastructure.Persistence;

internal sealed class VendorRegistrationOutcomeRecord
{
    public Guid VendorId { get; set; }

    public short FingerprintVersion { get; set; }

    public byte[] SemanticFingerprintSha256 { get; set; } = null!;

    public string ResultVendorState { get; set; } = null!;
}
