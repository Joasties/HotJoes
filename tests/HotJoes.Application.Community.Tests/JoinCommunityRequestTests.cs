using System.Reflection;
using HotJoes.Application.Community;

namespace HotJoes.Application.Community.Tests;

public sealed class JoinCommunityRequestTests
{
    [Fact]
    public void VR_COMMUNITY_001_Construction_RetainsExactlyVendorIdAndContactPreference()
    {
        Guid vendorId = Guid.NewGuid();
        var request = new JoinCommunityRequest(
            vendorId,
            ContactPreference.WhatsApp);

        Assert.Equal(vendorId, request.VendorId);
        Assert.Equal(ContactPreference.WhatsApp, request.ContactPreference);
    }

    [Fact]
    public void AI_APP_006_PublicSurface_IsClosedImmutableAndTransportIndependent()
    {
        Type requestType = typeof(JoinCommunityRequest);
        PropertyInfo[] properties = requestType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public);

        Assert.True(requestType.IsSealed);
        Assert.Equal(
            ["ContactPreference", "VendorId"],
            properties.Select(property => property.Name).Order());
        Assert.All(properties, property => Assert.Null(property.SetMethod));
        Assert.DoesNotContain(properties, property =>
            property.PropertyType.Namespace?.StartsWith(
                "Microsoft.AspNetCore",
                StringComparison.Ordinal) is true);
        Assert.DoesNotContain(properties, property =>
            property.Name.Contains("Contact", StringComparison.Ordinal) &&
            property.Name != "ContactPreference");
        Assert.DoesNotContain(properties, property =>
            property.Name.Contains("Consent", StringComparison.Ordinal) ||
            property.Name.Contains("Recipient", StringComparison.Ordinal) ||
            property.Name.Contains("Delivery", StringComparison.Ordinal) ||
            property.Name.Contains("Participation", StringComparison.Ordinal));
    }

    [Fact]
    public void VR_COMMUNITY_002_ContactPreference_ContainsExactlyTheApprovedValues()
    {
        Assert.Equal(
            ["Email", "Sms", "WhatsApp"],
            Enum.GetNames<ContactPreference>());
    }
}
