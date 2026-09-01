namespace HotJoes.Api.Vendor.Configuration;

public enum SecretRotationPhase
{
    ReplacementValidation = 1,
    OverlapEstablishment = 2,
    ReferencePublication = 3,
    ConsumerCutover = 4,
    HealthAndDrainConfirmation = 5,
    ProtectedResourceRevocation = 6,
    KeyVaultVersionRetirement = 7
}
