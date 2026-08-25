using AddressApplication = HotJoes.Application.Address;
using VendorApplication = HotJoes.Application.Vendor;
using VendorDomain = HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Vendor.Address;

namespace HotJoes.IntegrationTests;

public sealed class AddressResolutionTechnicalFailureTests
{
    [Theory]
    [InlineData(AddressApplication.AddressTechnicalFailure.Timeout)]
    [InlineData(AddressApplication.AddressTechnicalFailure.Unavailable)]
    [InlineData(AddressApplication.AddressTechnicalFailure.TransientFailure)]
    public void Resolve_WhenAddressFailsTechnically_ReturnsRetryableFailureWithoutAutomaticRetryAndAllowsLaterAttempt(
        AddressApplication.AddressTechnicalFailure technicalFailure)
    {
        const string reference = "address-resolution-reference-001";
        var addressService = new SequencedAddressResolutionService(
            AddressApplication.AddressResolutionResult.FailedTechnically(technicalFailure),
            AddressApplication.AddressResolutionResult.Succeeded(
                CreateCompleteAddressResult()));
        VendorApplication.IAddressResolver sut =
            new AddressResolutionAdapter(addressService);

        var firstAttempt = sut.Resolve(reference, VendorDomain.TradingLocation.Stall);

        Assert.IsType<
            VendorApplication.AddressResolutionResult.AddressServiceTemporarilyUnavailable>(
            firstAttempt);
        Assert.Equal(1, addressService.InvocationCount);
        Assert.Equal(reference, addressService.ReceivedReferences[0]);

        var laterCallerControlledAttempt = sut.Resolve(
            reference,
            VendorDomain.TradingLocation.Stall);

        Assert.IsType<VendorApplication.AddressResolutionResult.Success>(
            laterCallerControlledAttempt);
        Assert.Equal(2, addressService.InvocationCount);
        Assert.All(
            addressService.ReceivedReferences,
            receivedReference => Assert.Equal(reference, receivedReference));
    }

    private static AddressApplication.CompleteAddressResult CreateCompleteAddressResult()
    {
        return new AddressApplication.CompleteAddressResult(
            "canonical-address-001",
            "Hot Joes Limited",
            "10 Market Street",
            null,
            null,
            "GREENWICH",
            "SE10 9NN",
            null,
            "Greenwich Borough Council",
            "Greenwich Borough Council");
    }

    private sealed class SequencedAddressResolutionService
        : AddressApplication.IAddressResolutionService
    {
        private readonly Queue<AddressApplication.AddressResolutionResult> _results;

        public SequencedAddressResolutionService(
            params AddressApplication.AddressResolutionResult[] results)
        {
            _results = new Queue<AddressApplication.AddressResolutionResult>(results);
        }

        public int InvocationCount { get; private set; }

        public List<string> ReceivedReferences { get; } = [];

        public AddressApplication.AddressResolutionResult ResolveAddress(
            string addressResolutionReference,
            AddressApplication.TradingLocation tradingLocation)
        {
            InvocationCount++;
            ReceivedReferences.Add(addressResolutionReference);

            return _results.Dequeue();
        }
    }
}
