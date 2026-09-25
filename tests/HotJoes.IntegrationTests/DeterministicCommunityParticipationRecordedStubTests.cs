using HotJoes.Infrastructure.CommunityConsumer;

namespace HotJoes.IntegrationTests;

public sealed class DeterministicCommunityParticipationRecordedStubTests
{
    [Theory]
    [InlineData(CommunityContactPreference.Email)]
    [InlineData(CommunityContactPreference.Sms)]
    [InlineData(CommunityContactPreference.WhatsApp)]
    public async Task VR_COMMUNITY_020_AcceptsEveryControlledPreferenceWithoutExternalEffect(
        CommunityContactPreference preference)
    {
        var stub = new DeterministicCommunityParticipationRecordedStub();
        var message = new CommunityParticipationRecordedMessage(
            Guid.Parse("7bb04edf-b6b0-4e52-8600-c9ac9b71cc36"),
            Guid.Parse("b590cd67-9bf2-46fd-b4aa-02f25f404275"),
            Guid.Parse("7ecf7027-d8f8-4bfb-8559-c65896e52e48"),
            new DateTimeOffset(2026, 9, 23, 17, 0, 0, TimeSpan.Zero),
            preference);

        await stub.ProcessAsync(message);

        Assert.Empty(typeof(DeterministicCommunityParticipationRecordedStub)
            .GetFields(System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic));
    }
}
