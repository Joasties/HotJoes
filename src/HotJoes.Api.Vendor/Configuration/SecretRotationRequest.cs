namespace HotJoes.Api.Vendor.Configuration;

public sealed record SecretRotationRequest(
    Uri VaultUri,
    RequiredSecretReference Current,
    RequiredSecretReference Replacement,
    IReadOnlyList<string> ConsumerNames,
    SecretRotationStrategy Strategy);
