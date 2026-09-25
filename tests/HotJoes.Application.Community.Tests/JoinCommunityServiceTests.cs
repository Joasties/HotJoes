using HotJoes.Application.Community;

namespace HotJoes.Application.Community.Tests;

public sealed class JoinCommunityServiceTests
{
    [Fact]
    public async Task VR_COMMUNITY_004_And_005_ValidFirstRequest_VerifiesBeforeCommitAndReturnsRecorded()
    {
        Guid vendorId = Guid.NewGuid();
        var sequence = new List<string>();
        var verification = new RecordingVendorVerificationPort(
            VendorRegistrationVerificationResult.Registered(),
            sequence);
        CommunityParticipation participation = CreateParticipation(vendorId);
        var committer = new RecordingCommunityParticipationCommitter(
            CommunityParticipationCommitResult.Recorded(participation),
            sequence);
        JoinCommunityService service = CreateService(
            verification,
            committer);

        JoinCommunityResult result = await service.JoinAsync(
            new JoinCommunityRequest(vendorId, ContactPreference.Email));

        var recorded = Assert.IsType<
            JoinCommunityResult.CommunityParticipationRecorded>(result);
        Assert.Same(participation, recorded.Participation);
        Assert.Equal(["verify", "commit"], sequence);
        Assert.Equal(vendorId, verification.VendorId);
        Assert.NotNull(committer.Request);
        Assert.Equal(vendorId, committer.Request.VendorId);
        Assert.Equal(
            ContactPreference.Email,
            committer.Request.ContactPreference);
    }

    [Fact]
    public async Task VR_COMMUNITY_007_InvalidRequest_ReturnsAllErrorsWithoutCollaborators()
    {
        var verification = new ProhibitedVendorVerificationPort();
        var committer = new ProhibitedCommunityParticipationCommitter();
        JoinCommunityService service = CreateService(
            verification,
            committer);

        JoinCommunityResult result = await service.JoinAsync(
            new JoinCommunityRequest(
                Guid.Empty,
                (ContactPreference)int.MaxValue));

        var invalid = Assert.IsType<
            JoinCommunityResult.RequestValidationFailure>(result);
        Assert.Equal(2, invalid.Errors.Count);
        Assert.Equal(0, verification.CallCount);
        Assert.Equal(0, committer.CallCount);
    }

    [Fact]
    public async Task VR_COMMUNITY_006_VendorNotFound_ReturnsControlledFailureWithoutCommit()
    {
        var verification = new RecordingVendorVerificationPort(
            VendorRegistrationVerificationResult.NotFound(),
            []);
        var committer = new ProhibitedCommunityParticipationCommitter();
        JoinCommunityService service = CreateService(
            verification,
            committer);

        JoinCommunityResult result = await service.JoinAsync(
            CreateRequest());

        Assert.IsType<JoinCommunityResult.VendorNotFound>(result);
        Assert.Equal(0, committer.CallCount);
    }

    [Fact]
    public async Task VR_COMMUNITY_015_VerificationUnavailable_ReturnsControlledFailureWithoutCommit()
    {
        var verification = new RecordingVendorVerificationPort(
            VendorRegistrationVerificationResult.TemporarilyUnavailable(),
            []);
        var committer = new ProhibitedCommunityParticipationCommitter();
        JoinCommunityService service = CreateService(
            verification,
            committer);

        JoinCommunityResult result = await service.JoinAsync(
            CreateRequest());

        Assert.IsType<
            JoinCommunityResult.VendorVerificationUnavailable>(result);
        Assert.Equal(0, committer.CallCount);
    }

    [Fact]
    public async Task VR_COMMUNITY_010_EquivalentReplay_ReturnsOriginalCommittedValues()
    {
        CommunityParticipation original = CreateParticipation(Guid.NewGuid());
        var committer = new RecordingCommunityParticipationCommitter(
            CommunityParticipationCommitResult.AlreadyRecorded(original),
            []);
        JoinCommunityService service = CreateService(
            RegisteredVerification(),
            committer);

        JoinCommunityResult result = await service.JoinAsync(
            new JoinCommunityRequest(
                original.VendorId,
                original.ContactPreference));

        var replay = Assert.IsType<
            JoinCommunityResult.CommunityParticipationAlreadyRecorded>(
                result);
        Assert.Same(original, replay.Participation);
    }

    [Fact]
    public async Task VR_COMMUNITY_011_DifferentPreference_ReturnsConflict()
    {
        var committer = new RecordingCommunityParticipationCommitter(
            CommunityParticipationCommitResult.Conflict(),
            []);
        JoinCommunityService service = CreateService(
            RegisteredVerification(),
            committer);

        JoinCommunityResult result = await service.JoinAsync(
            CreateRequest());

        Assert.IsType<
            JoinCommunityResult.CommunityParticipationConflict>(result);
    }

    [Fact]
    public async Task VR_COMMUNITY_009_PersistenceUnavailable_ReturnsControlledFailure()
    {
        var committer = new RecordingCommunityParticipationCommitter(
            CommunityParticipationCommitResult.TemporarilyUnavailable(),
            []);
        JoinCommunityService service = CreateService(
            RegisteredVerification(),
            committer);

        JoinCommunityResult result = await service.JoinAsync(
            CreateRequest());

        Assert.IsType<
            JoinCommunityResult.CommunityPersistenceUnavailable>(result);
    }

    private static JoinCommunityService CreateService(
        IVendorRegistrationVerificationPort verification,
        ICommunityParticipationCommitter committer) =>
        new(
            new JoinCommunityRequestValidator(),
            verification,
            committer);

    private static JoinCommunityRequest CreateRequest() =>
        new(Guid.NewGuid(), ContactPreference.Email);

    private static CommunityParticipation CreateParticipation(Guid vendorId) =>
        new(
            Guid.NewGuid(),
            vendorId,
            ContactPreference.Email,
            new DateTimeOffset(2026, 9, 22, 10, 30, 0, TimeSpan.Zero));

    private static RecordingVendorVerificationPort RegisteredVerification() =>
        new(VendorRegistrationVerificationResult.Registered(), []);

    private sealed class RecordingVendorVerificationPort
        : IVendorRegistrationVerificationPort
    {
        private readonly VendorRegistrationVerificationResult _result;
        private readonly ICollection<string> _sequence;

        public RecordingVendorVerificationPort(
            VendorRegistrationVerificationResult result,
            ICollection<string> sequence)
        {
            _result = result;
            _sequence = sequence;
        }

        public int CallCount { get; private set; }
        public Guid? VendorId { get; private set; }

        public ValueTask<VendorRegistrationVerificationResult> VerifyAsync(
            Guid vendorId,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            VendorId = vendorId;
            _sequence.Add("verify");
            return ValueTask.FromResult(_result);
        }
    }

    private sealed class ProhibitedVendorVerificationPort
        : IVendorRegistrationVerificationPort
    {
        public int CallCount { get; private set; }

        public ValueTask<VendorRegistrationVerificationResult> VerifyAsync(
            Guid vendorId,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            throw new InvalidOperationException(
                "Vendor verification must not run after validation failure.");
        }
    }

    private sealed class RecordingCommunityParticipationCommitter
        : ICommunityParticipationCommitter
    {
        private readonly CommunityParticipationCommitResult _result;
        private readonly ICollection<string> _sequence;

        public RecordingCommunityParticipationCommitter(
            CommunityParticipationCommitResult result,
            ICollection<string> sequence)
        {
            _result = result;
            _sequence = sequence;
        }

        public int CallCount { get; private set; }
        public JoinCommunityRequest? Request { get; private set; }

        public ValueTask<CommunityParticipationCommitResult> CommitAsync(
            JoinCommunityRequest request,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            Request = request;
            _sequence.Add("commit");
            return ValueTask.FromResult(_result);
        }
    }

    private sealed class ProhibitedCommunityParticipationCommitter
        : ICommunityParticipationCommitter
    {
        public int CallCount { get; private set; }

        public ValueTask<CommunityParticipationCommitResult> CommitAsync(
            JoinCommunityRequest request,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            throw new InvalidOperationException(
                "Community persistence must not run on this path.");
        }
    }
}
