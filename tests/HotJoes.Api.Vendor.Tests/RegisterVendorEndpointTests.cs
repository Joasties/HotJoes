using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class RegisterVendorEndpointTests
{
    public static TheoryData<RegisterVendorResult, HttpStatusCode, string>
        ControlledFailures => new()
        {
            {
                RegisterVendorResult.ReferenceIsInvalid(),
                HttpStatusCode.BadRequest,
                "invalidAddressReference"
            },
            {
                RegisterVendorResult.AddressResultIsInvalid(),
                HttpStatusCode.BadRequest,
                "invalidAddressResult"
            },
            {
                RegisterVendorResult.AggregateInvariantFailed(),
                HttpStatusCode.BadRequest,
                "aggregateInvariantFailed"
            },
            {
                RegisterVendorResult.IdempotencyConflictDetected(),
                HttpStatusCode.Conflict,
                "idempotencyConflict"
            },
            {
                RegisterVendorResult.AddressServiceIsTemporarilyUnavailable(),
                HttpStatusCode.ServiceUnavailable,
                "addressServiceTemporarilyUnavailable"
            },
            {
                RegisterVendorResult.PersistenceOrAtomicRecordingFailed(),
                HttpStatusCode.ServiceUnavailable,
                "persistenceOrAtomicRecordingFailed"
            }
        };

    [Fact]
    public async Task Post_ValidRequest_InvokesApplicationOnceAndReturnsCreatedResource()
    {
        using var factory = new VendorApiFactory();
        var vendorId = new VendorId(
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        factory.Registration.NextResult = RegisterVendorResult.Succeeded(vendorId);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            Json(VendorApiTestData.CompleteRequest));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(
            "/vendors/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            response.Headers.Location?.OriginalString);
        Assert.Equal(1, factory.Registration.InvocationCount);
        Assert.True(factory.Registration.LastCancellationTokenCanBeCanceled);

        RegisterVendorCommand command = Assert.IsType<RegisterVendorCommand>(
            factory.Registration.LastCommand);
        Assert.Equal("Hot Joe's Kitchen", command.TradingName);
        Assert.Equal(TradingLocation.Kitchen, command.TradingLocation);

        using JsonDocument body = await ReadJson(response);
        Assert.Equal(
            "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            body.RootElement.GetProperty("vendorId").GetString());
        Assert.Equal(
            "pendingActivation",
            body.RootElement.GetProperty("vendorState").GetString());
    }

    [Fact]
    public async Task Post_EquivalentReplay_ReturnsOriginalCreatedResponseAgain()
    {
        using var factory = new VendorApiFactory();
        var vendorId = new VendorId(
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        factory.Registration.NextResult = RegisterVendorResult.Succeeded(vendorId);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage first = await client.PostAsync(
            "/vendors",
            Json(VendorApiTestData.CompleteRequest));
        using HttpResponseMessage replay = await client.PostAsync(
            "/vendors",
            Json(VendorApiTestData.CompleteRequest));

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, replay.StatusCode);
        Assert.Equal(first.Headers.Location, replay.Headers.Location);
        Assert.Equal(await first.Content.ReadAsStringAsync(), await replay.Content.ReadAsStringAsync());
    }

    [Theory]
    [MemberData(nameof(ControlledFailures))]
    public async Task Post_ControlledFailure_ReturnsExactMappedEnvelope(
        RegisterVendorResult result,
        HttpStatusCode expectedStatus,
        string expectedCode)
    {
        using var factory = new VendorApiFactory();
        factory.Registration.NextResult = result;
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            Json(VendorApiTestData.CompleteRequest));

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using JsonDocument body = await ReadJson(response);
        Assert.Equal(expectedCode, body.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            JsonValueKind.Null,
            body.RootElement.GetProperty("validationErrors").ValueKind);
    }

    [Fact]
    public async Task Post_ApplicationValidationFailure_ReturnsUnifiedValidationEnvelope()
    {
        using var factory = new VendorApiFactory();
        factory.Registration.NextResult = RegisterVendorResult.RequestValidationFailed(
        [
            new RegistrationValidationError(
                nameof(RegisterVendorCommand.TradingName),
                RegistrationValidationErrorCode.Required,
                "Trading Name is required."),
            new RegistrationValidationError(
                nameof(RegisterVendorCommand.InformationAccurate),
                RegistrationValidationErrorCode.InvalidValue,
                "Information Accurate must be accepted.")
        ]);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            Json(VendorApiTestData.CompleteRequest));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using JsonDocument body = await ReadJson(response);
        Assert.Equal(
            "registrationValidationFailed",
            body.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            2,
            body.RootElement.GetProperty("validationErrors").GetArrayLength());
    }

    private static StringContent Json(string value)
    {
        var content = new StringContent(value, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
        {
            CharSet = "utf-8"
        };
        return content;
    }

    private static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }
}
