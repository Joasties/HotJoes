using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HotJoes.Application.Vendor;
using ApplicationRequest = HotJoes.Application.Vendor.DetermineRequiredLicenceTypesRequest;

namespace HotJoes.Api.Vendor.Tests;

public sealed class DetermineRequiredLicenceTypesEndpointTests
{
    [Fact]
    public async Task VR_API_014_And_016_PostMapsOnceAndReturnsCompleteDetermination()
    {
        await using VendorApiFactory factory = new();
        factory.Determination.NextResult = DetermineRequiredLicenceTypesResult.Succeeded(
            CompleteDetermination());
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendor-registration/required-licence-types",
            Json(DetermineRequiredLicenceTypesRequestContractTests.ValidRequest));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(response.Headers.Location);
        Assert.Equal(1, factory.Determination.InvocationCount);
        Assert.True(factory.Determination.LastCancellationTokenCanBeCanceled);
        ApplicationRequest request =
            Assert.IsType<ApplicationRequest>(
                factory.Determination.LastRequest);
        Assert.Equal("addr-resolution-example", request.AddressResolutionReference);

        using JsonDocument body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal("epic1-v1", body.RootElement.GetProperty("ruleSetVersion").GetString());
        JsonElement[] items = body.RootElement.GetProperty("items")
            .EnumerateArray()
            .ToArray();
        Assert.Equal(
            [
                "foodBusinessRegistration",
                "streetTradingLicence",
                "lateNightRefreshmentLicence",
                "premisesLicence",
                "personalLicenceHolder"
            ],
            items.Select(item => item.GetProperty("requiredLicenceType").GetString()));
        Assert.Equal([true, false, true, false, false],
            items.Select(item => item.GetProperty("isRequired").GetBoolean()));
        Assert.Equal(
            ["items", "ruleSetVersion"],
            body.RootElement.EnumerateObject()
                .Select(property => property.Name)
                .Order(StringComparer.Ordinal));
    }

    [Theory]
    [MemberData(nameof(ControlledFailures))]
    public async Task VR_API_017_ControlledFailureUsesApprovedEnvelope(
        DetermineRequiredLicenceTypesResult result,
        HttpStatusCode status,
        string code,
        bool hasValidationErrors)
    {
        await using VendorApiFactory factory = new();
        factory.Determination.NextResult = result;
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendor-registration/required-licence-types",
            Json(DetermineRequiredLicenceTypesRequestContractTests.ValidRequest));

        Assert.Equal(status, response.StatusCode);
        Assert.Null(response.Headers.Location);
        using JsonDocument body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal(code, body.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            hasValidationErrors ? JsonValueKind.Array : JsonValueKind.Null,
            body.RootElement.GetProperty("validationErrors").ValueKind);
        Assert.False(body.RootElement.TryGetProperty("ruleSetVersion", out _));
        Assert.False(body.RootElement.TryGetProperty("items", out _));
        Assert.Equal(1, factory.Determination.InvocationCount);
    }

    [Theory]
    [MemberData(nameof(StructurallyMalformedBodies))]
    public async Task VR_API_015_MalformedBodyReturnsRequestMalformedWithoutInvocation(
        string body)
    {
        await using VendorApiFactory factory = new();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendor-registration/required-licence-types",
            Json(body));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.Determination.InvocationCount);
        using JsonDocument responseBody = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal(
            "requestMalformed",
            responseBody.RootElement.GetProperty("code").GetString());
    }

    public static TheoryData<DetermineRequiredLicenceTypesResult, HttpStatusCode, string, bool>
        ControlledFailures => new()
        {
            {
                DetermineRequiredLicenceTypesResult.ValidationFailed(
                [
                    new RegistrationValidationError(
                        nameof(ApplicationRequest.AddressResolutionReference),
                        RegistrationValidationErrorCode.Required,
                        "Address Resolution Reference is required.")
                ]),
                HttpStatusCode.BadRequest,
                "determinationValidationFailed",
                true
            },
            { DetermineRequiredLicenceTypesResult.ReferenceIsInvalid(), HttpStatusCode.BadRequest, "invalidAddressReference", false },
            { DetermineRequiredLicenceTypesResult.AddressResultIsInvalid(), HttpStatusCode.BadRequest, "invalidAddressResult", false },
            { DetermineRequiredLicenceTypesResult.Unsupported(), HttpStatusCode.Conflict, "complianceDeterminationUnsupported", false },
            { DetermineRequiredLicenceTypesResult.AddressIsTemporarilyUnavailable(), HttpStatusCode.ServiceUnavailable, "addressServiceTemporarilyUnavailable", false },
            { DetermineRequiredLicenceTypesResult.ComplianceIsTemporarilyUnavailable(), HttpStatusCode.ServiceUnavailable, "complianceDeterminationTemporarilyUnavailable", false }
        };

    public static TheoryData<string> StructurallyMalformedBodies => new()
    {
        "{}",
        "{ not-json }",
        DetermineRequiredLicenceTypesRequestContractTests.ValidRequest.Replace(
            "\"legalOperatorType\": \"limitedCompany\",",
            string.Empty,
            StringComparison.Ordinal),
        DetermineRequiredLicenceTypesRequestContractTests.ValidRequest.Replace(
            "\"tradingLocation\": \"kitchen\"",
            "\"tradingLocation\": \"not-a-location\"",
            StringComparison.Ordinal),
        DetermineRequiredLicenceTypesRequestContractTests.ValidRequest.Replace(
            "\"weeklyOpeningHours\": {",
            "\"weeklyOpeningHours\": [",
            StringComparison.Ordinal),
        DetermineRequiredLicenceTypesRequestContractTests.ValidRequest.Replace(
            "\"startTime\": \"17:00:00\"",
            "\"startTime\": \"seventeen\"",
            StringComparison.Ordinal)
    };

    private static ComplianceDetermination CompleteDetermination() =>
        new(
            "epic1-v1",
            [
                new(RequiredLicenceType.FoodBusinessRegistration, true),
                new(RequiredLicenceType.StreetTradingLicence, false),
                new(RequiredLicenceType.LateNightRefreshmentLicence, true),
                new(RequiredLicenceType.PremisesLicence, false),
                new(RequiredLicenceType.PersonalLicenceHolder, false)
            ]);

    private static StringContent Json(string value) =>
        new(value, Encoding.UTF8, "application/json");
}
