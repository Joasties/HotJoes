using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class DetermineRequiredLicenceTypesAuthorityContextTests
{
    [Fact]
    public async Task VR_DETERMINATION_008_StallWithoutPrimaryTradingAuthority_ReturnsInvalidAddressWithoutComplianceCall()
    {
        var compliance = new ProhibitedComplianceDeterminationPort();
        var service = new DetermineRequiredLicenceTypesService(
            new DetermineRequiredLicenceTypesRequestValidator(),
            new AddressResolutionInvoker(
                new StallAddressWithoutPrimaryTradingAuthority()),
            compliance);
        var request = new DetermineRequiredLicenceTypesRequest(
            LegalOperatorType.LimitedCompany,
            TradingLocation.Stall,
            RegisterVendorWeeklyOpeningHours.EveryDay(
                new TimeOnly(9, 0),
                new TimeOnly(17, 0)),
            serviceIncludesHotFood: false,
            alcoholService: false,
            "address-resolution-reference-001");

        DetermineRequiredLicenceTypesResult result =
            await service.DetermineAsync(request);

        Assert.IsType<
            DetermineRequiredLicenceTypesResult.InvalidAddressResult>(result);
        Assert.Equal(0, compliance.CallCount);
    }

    private sealed class StallAddressWithoutPrimaryTradingAuthority
        : IAddressResolver
    {
        public AddressResolutionResult Resolve(
            string addressResolutionReference,
            TradingLocation tradingLocation) =>
            AddressResolutionResult.Succeeded(
                new AddressAuthoritativeValues(
                    new CanonicalAddressId("canonical-address-001"),
                    new BusinessAddressSnapshot(
                        "2 High Street",
                        null,
                        null,
                        "GREENWICH",
                        "SE10 8AA",
                        null,
                        "Hot Joes"),
                    new FoodRegistrationAuthority(
                        "Royal Borough of Greenwich"),
                    primaryTradingAuthority: null));
    }

    private sealed class ProhibitedComplianceDeterminationPort
        : IComplianceDeterminationPort
    {
        public int CallCount { get; private set; }

        public ComplianceDeterminationPortResult Determine(
            ComplianceDeterminationRequest request)
        {
            CallCount++;
            throw new InvalidOperationException(
                "Compliance must not be called without required authoritative context.");
        }
    }
}
