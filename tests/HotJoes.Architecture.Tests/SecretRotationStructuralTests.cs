using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.Architecture.Tests;

public sealed class SecretRotationStructuralTests
{
    [Fact]
    public void AI_SEC_002_RotationProtocol_IsOwnedByDeployableEdge()
    {
        Type[] protocolTypes =
        [
            typeof(SecretRotationRequest),
            typeof(SecretRotationResult),
            typeof(SecretRotationCoordinator),
            typeof(ISecretRotationCandidateValidator),
            typeof(ISecretRotationOverlapCoordinator),
            typeof(ISecretRotationReferencePublisher),
            typeof(ISecretRotationConsumerCutover),
            typeof(ISecretRotationHealthGate),
            typeof(ISecretRotationCredentialRevoker),
            typeof(ISecretRotationVersionRetirer)
        ];

        Assert.All(
            protocolTypes,
            type => Assert.Equal(
                "HotJoes.Api.Vendor.Configuration",
                type.Namespace));
        Assert.DoesNotContain(
            protocolTypes.SelectMany(type => type.GetProperties()),
            property => property.Name.Contains(
                "Value",
                StringComparison.OrdinalIgnoreCase));

        Type[] coordinatorDependencies =
            Assert.Single(typeof(SecretRotationCoordinator).GetConstructors())
                .GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray();

        Assert.Equal(
            new[]
            {
                typeof(ISecretRotationCandidateValidator),
                typeof(ISecretRotationOverlapCoordinator),
                typeof(ISecretRotationReferencePublisher),
                typeof(ISecretRotationConsumerCutover),
                typeof(ISecretRotationHealthGate),
                typeof(ISecretRotationCredentialRevoker),
                typeof(ISecretRotationVersionRetirer)
            },
            coordinatorDependencies);
        Assert.DoesNotContain(
            coordinatorDependencies,
            type => type.Assembly.GetName().Name?.StartsWith(
                "Azure.",
                StringComparison.Ordinal) == true);
    }
}
