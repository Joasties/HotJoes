using HotJoes.Application.Community;

namespace HotJoes.Application.Community.Tests;

public sealed class JoinCommunityRequestValidatorTests
{
    [Fact]
    public void VR_COMMUNITY_007_MultipleIndependentErrors_AreReturnedTogether()
    {
        var request = new JoinCommunityRequest(
            Guid.Empty,
            (ContactPreference)int.MaxValue);
        var validator = new JoinCommunityRequestValidator();

        JoinCommunityRequestValidationResult result =
            validator.Validate(request);

        var invalid = Assert.IsType<
            JoinCommunityRequestValidationResult.Invalid>(result);
        Assert.Collection(
            invalid.Errors.OrderBy(error => error.Field),
            error => Assert.Equal("contactPreference", error.Field),
            error => Assert.Equal("vendorId", error.Field));
        Assert.All(
            invalid.Errors,
            error => Assert.Equal("invalidValue", error.Code));
    }

    [Theory]
    [InlineData(ContactPreference.Email)]
    [InlineData(ContactPreference.Sms)]
    [InlineData(ContactPreference.WhatsApp)]
    public void VR_COMMUNITY_002_EachApprovedPreference_IsAccepted(
        ContactPreference preference)
    {
        var request = new JoinCommunityRequest(Guid.NewGuid(), preference);
        var validator = new JoinCommunityRequestValidator();

        JoinCommunityRequestValidationResult result =
            validator.Validate(request);

        var accepted = Assert.IsType<
            JoinCommunityRequestValidationResult.Accepted>(result);
        Assert.Same(request, accepted.Request);
    }
}
