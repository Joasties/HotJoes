using HotJoes.Api.Vendor.Configuration;

namespace HotJoes.IntegrationTests;

public sealed class SecretRotationCoordinatorTests
{
    [Fact]
    public async Task AI_SEC_002_ValidReplacement_CompletesApprovedOrder()
    {
        var boundary = new ScriptedBoundary();
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        SecretRotationResult result = await coordinator.RotateAsync(Request());

        Assert.Equal(SecretRotationResultKind.Completed, result.Kind);
        Assert.Equal(
            new[]
            {
                "verify-cutover-strategy",
                "validate-replacement",
                "establish-overlap",
                "publish-replacement-reference",
                "cut-over-consumers",
                "confirm-health-drain-and-durable-work",
                "revoke-protected-resource-credential",
                "retire-key-vault-version"
            },
            boundary.Calls);
    }

    [Theory]
    [InlineData(InvalidRequest.EqualVersions)]
    [InlineData(InvalidRequest.DifferentPurpose)]
    [InlineData(InvalidRequest.DifferentSecretName)]
    [InlineData(InvalidRequest.EmptyConsumers)]
    [InlineData(InvalidRequest.UnsupportedStrategy)]
    public async Task AI_SEC_002_InvalidRequest_StopsBeforeExternalAction(
        InvalidRequest invalidRequest)
    {
        var boundary = new ScriptedBoundary();
        SecretRotationRequest request = Invalid(Request(), invalidRequest);
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        SecretRotationResult result = await coordinator.RotateAsync(request);

        Assert.Equal(SecretRotationResultKind.InvalidRequest, result.Kind);
        Assert.Empty(boundary.Calls);
    }

    [Theory]
    [InlineData(
        SecretRotationPhase.ReplacementValidation,
        SecretRotationResultKind.ReplacementValidationFailed)]
    [InlineData(
        SecretRotationPhase.OverlapEstablishment,
        SecretRotationResultKind.OverlapUnavailable)]
    [InlineData(
        SecretRotationPhase.ReferencePublication,
        SecretRotationResultKind.ReferencePublicationFailed)]
    [InlineData(
        SecretRotationPhase.ConsumerCutover,
        SecretRotationResultKind.ConsumerCutoverFailed)]
    [InlineData(
        SecretRotationPhase.HealthAndDrainConfirmation,
        SecretRotationResultKind.HealthAndDrainFailed)]
    public async Task AI_SEC_002_PreRevocationFailure_RetainsCurrentCredential(
        SecretRotationPhase failurePhase,
        SecretRotationResultKind expectedKind)
    {
        var boundary = new ScriptedBoundary(failurePhase);
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        SecretRotationResult result = await coordinator.RotateAsync(Request());

        Assert.Equal(expectedKind, result.Kind);
        Assert.DoesNotContain(
            "revoke-protected-resource-credential",
            boundary.Calls);
        Assert.DoesNotContain(
            "retire-key-vault-version",
            boundary.Calls);

        if (failurePhase >= SecretRotationPhase.ReferencePublication)
        {
            Assert.Contains("restore-current-reference", boundary.Calls);
        }
    }

    [Fact]
    public async Task AI_SEC_002_UnverifiedAtomicRefresh_StopsBeforePublication()
    {
        var boundary = new ScriptedBoundary
        {
            AtomicRefreshIsVerified = false
        };
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        SecretRotationResult result = await coordinator.RotateAsync(
            Request(SecretRotationStrategy.VerifiedAtomicRefresh));

        Assert.Equal(
            SecretRotationResultKind.CutoverStrategyNotVerified,
            result.Kind);
        Assert.Equal(
            new[] { "verify-cutover-strategy" },
            boundary.Calls);
    }

    [Fact]
    public async Task AI_SEC_002_RollingReplacement_RequiresAllConsumersAndWorkSafe()
    {
        var boundary = new ScriptedBoundary
        {
            HealthAndDrainConfirmed = false
        };
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        SecretRotationResult result = await coordinator.RotateAsync(
            Request(
                SecretRotationStrategy.HealthGatedRollingReplacement));

        Assert.Equal(
            SecretRotationResultKind.HealthAndDrainFailed,
            result.Kind);
        Assert.DoesNotContain(
            "revoke-protected-resource-credential",
            boundary.Calls);
        Assert.Contains("restore-current-reference", boundary.Calls);
    }

    [Fact]
    public async Task AI_SEC_002_CancellationBeforeRevocation_RestoresAndPropagates()
    {
        var boundary = new ScriptedBoundary
        {
            CancelAt = SecretRotationPhase.HealthAndDrainConfirmation
        };
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => coordinator.RotateAsync(Request()));

        Assert.Contains("restore-current-reference", boundary.Calls);
        Assert.DoesNotContain(
            "revoke-protected-resource-credential",
            boundary.Calls);
    }

    [Fact]
    public async Task AI_SEC_002_ProtectedResourceRevocationFails_DoesNotRetireVaultVersion()
    {
        var boundary = new ScriptedBoundary(
            SecretRotationPhase.ProtectedResourceRevocation);
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        SecretRotationResult result = await coordinator.RotateAsync(Request());

        Assert.Equal(
            SecretRotationResultKind.ProtectedResourceRevocationFailed,
            result.Kind);
        Assert.DoesNotContain(
            "retire-key-vault-version",
            boundary.Calls);
        Assert.DoesNotContain("restore-current-reference", boundary.Calls);
    }

    [Fact]
    public async Task AI_SEC_002_VaultRetirementFails_ReportsRecoverablePartialCompletion()
    {
        var boundary = new ScriptedBoundary(
            SecretRotationPhase.KeyVaultVersionRetirement);
        SecretRotationCoordinator coordinator = Coordinator(boundary);

        SecretRotationResult result = await coordinator.RotateAsync(Request());

        Assert.Equal(
            SecretRotationResultKind.KeyVaultVersionRetirementFailed,
            result.Kind);
        Assert.Contains(
            "revoke-protected-resource-credential",
            boundary.Calls);
        Assert.DoesNotContain("restore-current-reference", boundary.Calls);
    }

    [Fact]
    public void AI_SEC_002_ProtocolContracts_ExposeNoSecretValue()
    {
        Type[] contractTypes =
        [
            typeof(SecretRotationRequest),
            typeof(SecretRotationResult)
        ];

        Assert.DoesNotContain(
            contractTypes.SelectMany(type => type.GetProperties()),
            property => property.Name.Contains(
                "Value",
                StringComparison.OrdinalIgnoreCase));
        Assert.All(
            contractTypes.SelectMany(type => type.GetProperties()),
            property => Assert.NotEqual(typeof(AzureKeyVaultResolvedSecret),
                property.PropertyType));
    }

    private static SecretRotationCoordinator Coordinator(
        ScriptedBoundary boundary)
    {
        return new SecretRotationCoordinator(
            boundary,
            boundary,
            boundary,
            boundary,
            boundary,
            boundary,
            boundary);
    }

    private static SecretRotationRequest Request(
        SecretRotationStrategy strategy =
            SecretRotationStrategy.HealthGatedRollingReplacement)
    {
        return new SecretRotationRequest(
            new Uri("https://hotjoes-production.vault.azure.net"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                "vendor-api-persistence",
                "current-version"),
            new RequiredSecretReference(
                "vendor-persistence-connection",
                "vendor-api-persistence",
                "replacement-version"),
            ["vendor-api-a", "vendor-api-b"],
            strategy);
    }

    private static SecretRotationRequest Invalid(
        SecretRotationRequest request,
        InvalidRequest invalidRequest)
    {
        return invalidRequest switch
        {
            InvalidRequest.EqualVersions => request with
            {
                Replacement = request.Current
            },
            InvalidRequest.DifferentPurpose => request with
            {
                Replacement = request.Replacement with
                {
                    Purpose = "different-purpose"
                }
            },
            InvalidRequest.DifferentSecretName => request with
            {
                Replacement = request.Replacement with
                {
                    Name = "different-secret"
                }
            },
            InvalidRequest.EmptyConsumers => request with
            {
                ConsumerNames = []
            },
            InvalidRequest.UnsupportedStrategy => request with
            {
                Strategy = (SecretRotationStrategy)999
            },
            _ => throw new ArgumentOutOfRangeException(
                nameof(invalidRequest))
        };
    }

    public enum InvalidRequest
    {
        EqualVersions = 1,
        DifferentPurpose = 2,
        DifferentSecretName = 3,
        EmptyConsumers = 4,
        UnsupportedStrategy = 5
    }

    private sealed class ScriptedBoundary :
        ISecretRotationCandidateValidator,
        ISecretRotationOverlapCoordinator,
        ISecretRotationReferencePublisher,
        ISecretRotationConsumerCutover,
        ISecretRotationHealthGate,
        ISecretRotationCredentialRevoker,
        ISecretRotationVersionRetirer
    {
        private readonly SecretRotationPhase? _failurePhase;
        private readonly List<string> _calls = [];

        public ScriptedBoundary(SecretRotationPhase? failurePhase = null)
        {
            _failurePhase = failurePhase;
        }

        public IReadOnlyList<string> Calls => _calls;

        public bool AtomicRefreshIsVerified { get; init; } = true;

        public bool HealthAndDrainConfirmed { get; init; } = true;

        public SecretRotationPhase? CancelAt { get; init; }

        public Task<bool> ValidateReplacementAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            return Execute(
                SecretRotationPhase.ReplacementValidation,
                "validate-replacement",
                cancellationToken);
        }

        public Task<bool> EstablishOverlapAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            return Execute(
                SecretRotationPhase.OverlapEstablishment,
                "establish-overlap",
                cancellationToken);
        }

        public Task<bool> IsStrategyVerifiedAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            _calls.Add("verify-cutover-strategy");
            return Task.FromResult(AtomicRefreshIsVerified);
        }

        public Task<bool> PublishReplacementAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            return Execute(
                SecretRotationPhase.ReferencePublication,
                "publish-replacement-reference",
                cancellationToken);
        }

        public Task RestoreCurrentAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            _calls.Add("restore-current-reference");
            return Task.CompletedTask;
        }

        public Task<bool> CutOverAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            return Execute(
                SecretRotationPhase.ConsumerCutover,
                "cut-over-consumers",
                cancellationToken);
        }

        public Task<bool> ConfirmHealthyDrainedAndDurableAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!HealthAndDrainConfirmed)
            {
                _calls.Add("confirm-health-drain-and-durable-work");
                return Task.FromResult(false);
            }

            return Execute(
                SecretRotationPhase.HealthAndDrainConfirmation,
                "confirm-health-drain-and-durable-work",
                cancellationToken);
        }

        public Task<bool> RevokeCurrentCredentialAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            return Execute(
                SecretRotationPhase.ProtectedResourceRevocation,
                "revoke-protected-resource-credential",
                cancellationToken);
        }

        public Task<bool> RetireCurrentVersionAsync(
            SecretRotationRequest request,
            CancellationToken cancellationToken = default)
        {
            return Execute(
                SecretRotationPhase.KeyVaultVersionRetirement,
                "retire-key-vault-version",
                cancellationToken);
        }

        private Task<bool> Execute(
            SecretRotationPhase phase,
            string call,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _calls.Add(call);

            if (CancelAt == phase)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            return Task.FromResult(_failurePhase != phase);
        }
    }
}
