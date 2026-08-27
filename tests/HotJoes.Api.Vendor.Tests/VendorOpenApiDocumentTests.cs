using System.Net;
using System.Text.Json;

namespace HotJoes.Api.Vendor.Tests;

public sealed class VendorOpenApiDocumentTests
{
    private const string TimePattern =
        "^([01]\\d|2[0-3]):[0-5]\\d:[0-5]\\d$";

    [Fact]
    public async Task GeneratedDocument_ExposesOnlyApprovedEpicOneOperations()
    {
        using JsonDocument document = await ReadDocument();
        JsonElement paths = document.RootElement.GetProperty("paths");

        Assert.Equal(
            new[] { "/vendors", "/vendors/{vendorId}" },
            paths.EnumerateObject()
                .Select(path => path.Name)
                .Order(StringComparer.Ordinal)
                .ToArray());
        Assert.Equal(
            new[] { "post" },
            paths.GetProperty("/vendors")
                .EnumerateObject()
                .Select(operation => operation.Name)
                .ToArray());
        Assert.Equal(
            new[] { "get" },
            paths.GetProperty("/vendors/{vendorId}")
                .EnumerateObject()
                .Select(operation => operation.Name)
                .ToArray());
    }

    [Fact]
    public async Task RegisterOperation_DescribesExactRequestShapeAndWireFormats()
    {
        using JsonDocument document = await ReadDocument();
        JsonElement root = document.RootElement;
        JsonElement operation = OpenApiDocumentAssertions.Operation(
            root,
            "/vendors",
            "post");
        JsonElement request = OpenApiDocumentAssertions.RequestSchema(
            root,
            operation);

        OpenApiDocumentAssertions.HasExactRequiredMembers(
            request,
            "tradingName",
            "legalOperatorName",
            "legalOperatorType",
            "tradingCharacteristics",
            "primaryContact",
            "addressResolutionReference",
            "registrationDeclarations");

        OpenApiDocumentAssertions.HasEnum(
            OpenApiDocumentAssertions.PropertySchema(
                root,
                request,
                "legalOperatorType"),
            "soleTrader",
            "generalPartnership",
            "limitedCompany",
            "limitedLiabilityPartnership",
            "charitableCommunityGroup",
            "charitableIncorporatedOrganisation");

        JsonElement trading = OpenApiDocumentAssertions.PropertySchema(
            root,
            request,
            "tradingCharacteristics");
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            trading,
            "tradingLocation",
            "openingHours",
            "serviceIncludesHotFood",
            "alcoholService");
        OpenApiDocumentAssertions.HasEnum(
            OpenApiDocumentAssertions.PropertySchema(
                root,
                trading,
                "tradingLocation"),
            "restaurant",
            "stall",
            "kitchen");

        JsonElement hours = OpenApiDocumentAssertions.PropertySchema(
            root,
            trading,
            "openingHours");
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            hours,
            "startTime",
            "endTime");
        Assert.Equal(
            TimePattern,
            OpenApiDocumentAssertions.PropertySchema(
                root,
                hours,
                "startTime").GetProperty("pattern").GetString());

        JsonElement contact = OpenApiDocumentAssertions.PropertySchema(
            root,
            request,
            "primaryContact");
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            contact,
            "contactName",
            "contactEmail",
            "contactTelephone");

        JsonElement declarations = OpenApiDocumentAssertions.PropertySchema(
            root,
            request,
            "registrationDeclarations");
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            declarations,
            "authorisedToRegisterBusiness",
            "informationAccurate",
            "acceptHotJoesPlatformTerms");

        foreach (string optional in new[]
                 {
                     "companyRegistrationNumber",
                     "website",
                     "businessDescription"
                 })
        {
            OpenApiDocumentAssertions.IsNullable(
                OpenApiDocumentAssertions.PropertySchema(root, request, optional));
        }
    }

    [Fact]
    public async Task RegisterOperation_DescribesSuccessErrorsAndLocation()
    {
        using JsonDocument document = await ReadDocument();
        JsonElement root = document.RootElement;
        JsonElement operation = OpenApiDocumentAssertions.Operation(
            root,
            "/vendors",
            "post");
        JsonElement responses = operation.GetProperty("responses");

        Assert.Equal(
            new[] { "201", "400", "409", "503" },
            responses.EnumerateObject()
                .Select(response => response.Name)
                .Order(StringComparer.Ordinal)
                .ToArray());
        Assert.True(
            responses.GetProperty("201")
                .GetProperty("headers")
                .TryGetProperty("Location", out _));

        JsonElement success = OpenApiDocumentAssertions.ResponseSchema(
            root,
            operation,
            "201");
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            success,
            "vendorId",
            "vendorState");
        Assert.Equal(
            "uuid",
            OpenApiDocumentAssertions.PropertySchema(
                root,
                success,
                "vendorId").GetProperty("format").GetString());
        OpenApiDocumentAssertions.HasEnum(
            OpenApiDocumentAssertions.PropertySchema(
                root,
                success,
                "vendorState"),
            "pendingActivation");

        AssertErrorSchema(root, operation, "400");
        AssertErrorSchema(root, operation, "409");
        AssertErrorSchema(root, operation, "503");
    }

    [Fact]
    public async Task RetrieveOperation_DescribesRouteAndCompleteNullableResponse()
    {
        using JsonDocument document = await ReadDocument();
        JsonElement root = document.RootElement;
        JsonElement operation = OpenApiDocumentAssertions.Operation(
            root,
            "/vendors/{vendorId}",
            "get");

        JsonElement parameter = Assert.Single(
            operation.GetProperty("parameters").EnumerateArray());
        Assert.Equal("vendorId", parameter.GetProperty("name").GetString());
        Assert.Equal("path", parameter.GetProperty("in").GetString());
        Assert.True(parameter.GetProperty("required").GetBoolean());
        Assert.Equal(
            "uuid",
            parameter.GetProperty("schema").GetProperty("format").GetString());

        Assert.Equal(
            new[] { "200", "400", "404" },
            operation.GetProperty("responses")
                .EnumerateObject()
                .Select(response => response.Name)
                .Order(StringComparer.Ordinal)
                .ToArray());

        JsonElement response = OpenApiDocumentAssertions.ResponseSchema(
            root,
            operation,
            "200");
        Assert.Equal(
            "uuid",
            OpenApiDocumentAssertions.PropertySchema(
                root,
                response,
                "vendorId").GetProperty("format").GetString());
        Assert.Equal(
            "date-time",
            OpenApiDocumentAssertions.PropertySchema(
                root,
                response,
                "registeredAt").GetProperty("format").GetString());

        foreach (string optional in new[]
                 {
                     "companyRegistrationNumber",
                     "primaryTradingAuthority",
                     "website",
                     "businessDescription"
                 })
        {
            Assert.Contains(
                optional,
                response.GetProperty("required")
                    .EnumerateArray()
                    .Select(item => item.GetString()));
            OpenApiDocumentAssertions.IsNullable(
                OpenApiDocumentAssertions.PropertySchema(root, response, optional));
        }

        JsonElement address = OpenApiDocumentAssertions.PropertySchema(
            root,
            response,
            "businessAddressSnapshot");
        foreach (string optional in new[]
                 {
                     "addressLine2",
                     "addressLine3",
                     "county",
                     "recipientOrOrganisationName"
                 })
        {
            Assert.Contains(
                optional,
                address.GetProperty("required")
                    .EnumerateArray()
                    .Select(item => item.GetString()));
            OpenApiDocumentAssertions.IsNullable(
                OpenApiDocumentAssertions.PropertySchema(root, address, optional));
        }

        AssertErrorSchema(root, operation, "400");
        AssertErrorSchema(root, operation, "404");
    }

    [Fact]
    public async Task GeneratedDocument_AdvertisesNoCustomOrExcludedContractSurface()
    {
        using JsonDocument document = await ReadDocument();
        string json = JsonSerializer.Serialize(document.RootElement);

        Assert.DoesNotContain("Idempotency-Key", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("correlation", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("/api/v", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("422", json, StringComparison.Ordinal);
        Assert.DoesNotContain("application/vnd", json, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertErrorSchema(
        JsonElement document,
        JsonElement operation,
        string status)
    {
        JsonElement error = OpenApiDocumentAssertions.ResponseSchema(
            document,
            operation,
            status);
        OpenApiDocumentAssertions.HasExactRequiredMembers(
            error,
            "code",
            "message",
            "validationErrors");
        OpenApiDocumentAssertions.IsNullable(
            OpenApiDocumentAssertions.PropertySchema(
                document,
                error,
                "validationErrors"));
    }

    private static async Task<JsonDocument> ReadDocument()
    {
        using var factory = new VendorOpenApiFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(
            "/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }
}
