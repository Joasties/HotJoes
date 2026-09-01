namespace HotJoes.Api.Vendor.Configuration;

public enum SecretRotationResultKind
{
    Completed = 1,
    InvalidRequest = 2,
    CutoverStrategyNotVerified = 3,
    ReplacementValidationFailed = 4,
    OverlapUnavailable = 5,
    ReferencePublicationFailed = 6,
    ConsumerCutoverFailed = 7,
    HealthAndDrainFailed = 8,
    ProtectedResourceRevocationFailed = 9,
    KeyVaultVersionRetirementFailed = 10
}
