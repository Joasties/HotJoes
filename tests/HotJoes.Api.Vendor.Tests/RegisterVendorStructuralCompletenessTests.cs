using System.Net;
using System.Text.Json;

namespace HotJoes.Api.Vendor.Tests;

public sealed class RegisterVendorStructuralCompletenessTests
{
    public static TheoryData<string> RequiredMemberPaths => new()
    {
        { "tradingName" },
        { "legalOperatorName" },
        { "legalOperatorType" },
        { "tradingCharacteristics" },
        { "tradingCharacteristics.tradingLocation" },
        { "tradingCharacteristics.openingHours" },
        { "tradingCharacteristics.openingHours.startTime" },
        { "tradingCharacteristics.openingHours.endTime" },
        { "tradingCharacteristics.serviceIncludesHotFood" },
        { "tradingCharacteristics.alcoholService" },
        { "primaryContact" },
        { "primaryContact.contactName" },
        { "primaryContact.contactEmail" },
        { "primaryContact.contactTelephone" },
        { "addressResolutionReference" },
        { "registrationDeclarations" },
        { "registrationDeclarations.authorisedToRegisterBusiness" },
        { "registrationDeclarations.informationAccurate" },
        { "registrationDeclarations.acceptHotJoesPlatformTerms" }
    };

    [Theory]
    [MemberData(nameof(RequiredMemberPaths))]
    public async Task Post_EachRequiredMemberAbsent_ReturnsRequestMalformed(
        string path)
    {
        string json = VendorApiBoundaryTestHelpers.RemoveMember(
            VendorApiTestData.CompleteRequest,
            path);

        await AssertMalformedWithoutApplicationInvocation(json);
    }

    [Theory]
    [MemberData(nameof(RequiredMemberPaths))]
    public async Task Post_EachRequiredMemberHasWrongTokenOrNesting_ReturnsRequestMalformed(
        string path)
    {
        string json = VendorApiBoundaryTestHelpers.ReplaceMemberWithWrongToken(
            VendorApiTestData.CompleteRequest,
            path);

        await AssertMalformedWithoutApplicationInvocation(json);
    }

    private static async Task AssertMalformedWithoutApplicationInvocation(
        string json)
    {
        using var factory = new VendorApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            VendorApiBoundaryTestHelpers.JsonContent(json));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.Registration.InvocationCount);

        using JsonDocument body =
            await VendorApiBoundaryTestHelpers.ReadJsonAsync(response);
        Assert.Equal(
            "requestMalformed",
            body.RootElement.GetProperty("code").GetString());
    }
}
