using System.Net;
using System.Text;
using System.Text.Json;
using HotJoes.Application.Community;
using ApplicationRequest = HotJoes.Application.Community.JoinCommunityRequest;

namespace HotJoes.Api.Vendor.Tests;

public sealed class JoinCommunityEndpointTests
{
    private static readonly Guid ParticipationId =
        Guid.Parse("b9c32512-6459-4210-a1cf-b6f999bf7018");
    private static readonly Guid VendorId =
        Guid.Parse("187c1149-b8be-4b44-9472-812545833661");
    private static readonly DateTimeOffset JoinedAt =
        DateTimeOffset.Parse("2026-09-23T08:15:30.0000000+00:00");

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task VR_API_018_To_020_PostMapsOnceAndReturnsStableCreatedResult(
        bool equivalentReplay)
    {
        await using VendorApiFactory factory = new();
        CommunityParticipation participation = new(
            ParticipationId,
            VendorId,
            ContactPreference.WhatsApp,
            JoinedAt);
        factory.Community.NextResult = equivalentReplay
            ? JoinCommunityResult.AlreadyRecorded(participation)
            : JoinCommunityResult.Recorded(participation);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/community-participations",
            Json($$"""
                {
                  "vendorId": "{{VendorId:D}}",
                  "contactPreference": "whatsApp",
                  "compatibleFutureMember": "ignored"
                }
                """));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(
            $"/community-participations/{ParticipationId:D}",
            response.Headers.Location?.OriginalString);
        Assert.Equal(1, factory.Community.InvocationCount);
        Assert.True(factory.Community.LastCancellationTokenCanBeCanceled);
        ApplicationRequest applicationRequest =
            Assert.IsType<ApplicationRequest>(factory.Community.LastRequest);
        Assert.Equal(VendorId, applicationRequest.VendorId);
        Assert.Equal(ContactPreference.WhatsApp, applicationRequest.ContactPreference);

        using JsonDocument body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal(
            new[]
            {
                "communityParticipationId",
                "contactPreference",
                "joinedAt",
                "vendorId"
            },
            body.RootElement.EnumerateObject()
                .Select(property => property.Name)
                .Order(StringComparer.Ordinal));
        Assert.Equal(ParticipationId, body.RootElement
            .GetProperty("communityParticipationId").GetGuid());
        Assert.Equal(VendorId, body.RootElement.GetProperty("vendorId").GetGuid());
        Assert.Equal("whatsApp", body.RootElement
            .GetProperty("contactPreference").GetString());
        Assert.Equal(JoinedAt, body.RootElement.GetProperty("joinedAt")
            .GetDateTimeOffset());
    }

    [Theory]
    [MemberData(nameof(ControlledFailures))]
    public async Task VR_API_021_ControlledFailuresUseApprovedMapping(
        JoinCommunityResult result,
        HttpStatusCode status,
        string code,
        bool hasValidationErrors)
    {
        await using VendorApiFactory factory = new();
        factory.Community.NextResult = result;
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/community-participations",
            Json(ValidRequest));

        Assert.Equal(status, response.StatusCode);
        Assert.Null(response.Headers.Location);
        Assert.Equal(1, factory.Community.InvocationCount);
        using JsonDocument body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal(code, body.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            hasValidationErrors ? JsonValueKind.Array : JsonValueKind.Null,
            body.RootElement.GetProperty("validationErrors").ValueKind);
    }

    [Theory]
    [MemberData(nameof(MalformedBodies))]
    public async Task VR_API_018_MalformedRequestReturns400WithoutInvocation(
        string body)
    {
        await using VendorApiFactory factory = new();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/community-participations",
            Json(body));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.Community.InvocationCount);
        using JsonDocument responseBody = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal("requestMalformed", responseBody.RootElement
            .GetProperty("code").GetString());
    }

    [Fact]
    public async Task VR_API_022_OneRequestInvokesApplicationOnlyOnce()
    {
        await using VendorApiFactory factory = new();
        factory.Community.NextResult = JoinCommunityResult.PersistenceIsUnavailable();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/community-participations",
            Json(ValidRequest));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(1, factory.Community.InvocationCount);
    }

    public static TheoryData<JoinCommunityResult, HttpStatusCode, string, bool>
        ControlledFailures => new()
        {
            {
                JoinCommunityResult.ValidationFailed(
                [
                    new CommunityValidationError(
                        nameof(JoinCommunityRequest.VendorId),
                        "required",
                        "Vendor ID is required.")
                ]),
                HttpStatusCode.BadRequest,
                "communityParticipationValidationFailed",
                true
            },
            { JoinCommunityResult.VendorWasNotFound(), HttpStatusCode.NotFound, "vendorNotFound", false },
            { JoinCommunityResult.Conflict(), HttpStatusCode.Conflict, "communityParticipationConflict", false },
            { JoinCommunityResult.VerificationIsUnavailable(), HttpStatusCode.ServiceUnavailable, "vendorVerificationUnavailable", false },
            { JoinCommunityResult.PersistenceIsUnavailable(), HttpStatusCode.ServiceUnavailable, "communityPersistenceUnavailable", false }
        };

    public static TheoryData<string> MalformedBodies => new()
    {
        "{}",
        "{ not-json }",
        $$"""{"vendorId":"{{VendorId:D}}"}""",
        """{"vendorId":"not-a-uuid","contactPreference":"email"}""",
        $$"""{"vendorId":"{{VendorId:D}}","contactPreference":"carrierPigeon"}"""
    };

    private static string ValidRequest =>
        $$"""{"vendorId":"{{VendorId:D}}","contactPreference":"email"}""";

    private static StringContent Json(string value) =>
        new(value, Encoding.UTF8, "application/json");
}
