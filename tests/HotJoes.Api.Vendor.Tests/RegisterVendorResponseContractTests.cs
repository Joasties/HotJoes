using System.Text.Json;
using HotJoes.Api.Vendor;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class RegisterVendorResponseContractTests
{
    [Fact]
    public void Serialize_Success_ContainsOnlyApprovedMinimumLowerCamelMembers()
    {
        var vendorId = new VendorId(
            Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE"));
        RegisterVendorResult result = RegisterVendorResult.Succeeded(vendorId);

        RegisterVendorResponse response =
            new RegisterVendorResponseMapper().Map(
                Assert.IsType<RegisterVendorResult.Success>(result));
        string json = JsonSerializer.Serialize(
            response,
            VendorApiJsonOptions.Create());

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        Assert.Equal(2, root.EnumerateObject().Count());
        Assert.Equal(
            "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            root.GetProperty("vendorId").GetString());
        Assert.Equal(
            "pendingActivation",
            root.GetProperty("vendorState").GetString());
        Assert.False(root.TryGetProperty("registeredAt", out _));
        Assert.False(root.TryGetProperty("tradingPreference", out _));
    }
}
