using System.Reflection;
using HotJoes.Application.Community;

namespace HotJoes.Application.Community.Tests;

public sealed class CommunityParticipationTests
{
    [Fact]
    public void AI_COMMUNITY_002_Participation_IsImmutableAndContainsOnlyApprovedInformation()
    {
        Guid participationId = Guid.NewGuid();
        Guid vendorId = Guid.NewGuid();
        DateTimeOffset joinedAt = DateTimeOffset.UtcNow;
        var participation = new CommunityParticipation(
            participationId,
            vendorId,
            ContactPreference.Email,
            joinedAt);

        Assert.Equal(participationId, participation.CommunityParticipationId);
        Assert.Equal(vendorId, participation.VendorId);
        Assert.Equal(ContactPreference.Email, participation.ContactPreference);
        Assert.Equal(joinedAt, participation.JoinedAt);

        PropertyInfo[] properties = typeof(CommunityParticipation)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);
        Assert.Equal(
            [
                "CommunityParticipationId",
                "ContactPreference",
                "JoinedAt",
                "VendorId"
            ],
            properties.Select(property => property.Name).Order());
        Assert.All(properties, property => Assert.Null(property.SetMethod));
    }

    [Fact]
    public void AI_COMMUNITY_002_ApplicationSurface_ContainsNoLifecycleMutationOperation()
    {
        string[] prohibitedFragments =
        [
            "Amend",
            "Delete",
            "Expire",
            "Update",
            "Withdraw"
        ];

        Type[] publicTypes = typeof(CommunityParticipation).Assembly
            .GetExportedTypes();

        Assert.DoesNotContain(publicTypes, type =>
            prohibitedFragments.Any(fragment =>
                type.Name.Contains(fragment, StringComparison.Ordinal)));
    }
}
