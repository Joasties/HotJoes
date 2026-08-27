using System.Net;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class RegisterVendorOptionalMemberRuntimeTests
{
    public static TheoryData<string> OptionalMemberPaths => new()
    {
        { "companyRegistrationNumber" },
        { "website" },
        { "businessDescription" }
    };

    [Theory]
    [MemberData(nameof(OptionalMemberPaths))]
    public async Task Post_OptionalMemberOmittedOrNull_MapsEquivalentAbsence(
        string path)
    {
        string explicitNull = VendorApiBoundaryTestHelpers.SetOptionalNull(
            VendorApiTestData.CompleteRequest,
            path);
        string omitted = VendorApiBoundaryTestHelpers.RemoveMember(
            VendorApiTestData.CompleteRequest,
            path);

        RegisterVendorCommand nullCommand = await PostAndCaptureCommand(explicitNull);
        RegisterVendorCommand omittedCommand = await PostAndCaptureCommand(omitted);

        Assert.Equal(ReadOptional(nullCommand, path), ReadOptional(omittedCommand, path));
        Assert.Null(ReadOptional(nullCommand, path));
    }

    private static async Task<RegisterVendorCommand> PostAndCaptureCommand(
        string json)
    {
        using var factory = new VendorApiFactory();
        factory.Registration.NextResult = RegisterVendorResult.Succeeded(
            new VendorId(Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")));
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/vendors",
            VendorApiBoundaryTestHelpers.JsonContent(json));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(1, factory.Registration.InvocationCount);
        return Assert.IsType<RegisterVendorCommand>(
            factory.Registration.LastCommand);
    }

    private static string? ReadOptional(
        RegisterVendorCommand command,
        string path)
    {
        return path switch
        {
            "companyRegistrationNumber" => command.CompanyRegistrationNumber,
            "website" => command.Website,
            "businessDescription" => command.BusinessDescription,
            _ => throw new ArgumentOutOfRangeException(nameof(path), path, null)
        };
    }
}
