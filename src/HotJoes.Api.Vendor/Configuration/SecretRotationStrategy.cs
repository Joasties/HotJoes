namespace HotJoes.Api.Vendor.Configuration;

public enum SecretRotationStrategy
{
    VerifiedAtomicRefresh = 1,
    HealthGatedRollingReplacement = 2
}
