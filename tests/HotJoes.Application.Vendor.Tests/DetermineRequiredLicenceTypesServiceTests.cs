using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class DetermineRequiredLicenceTypesServiceTests
{
    [Fact]
    public async Task VR_DETERMINATION_003_And_004_ValidRequest_ResolvesAddressBeforeOneComplianceCall()
    {
        DetermineRequiredLicenceTypesRequest request = CreateRequest();
        AddressAuthoritativeValues address = CreateAddress();
        var sequence = new List<string>();
        var resolver = new RecordingAddressResolver(
            AddressResolutionResult.Succeeded(address),
            sequence);
        ComplianceDetermination determination = CreateDetermination();
        var compliance = new RecordingComplianceDeterminationPort(
            ComplianceDeterminationPortResult.Succeeded(determination),
            sequence);
        var service = CreateService(resolver, compliance);

        DetermineRequiredLicenceTypesResult result =
            await service.DetermineAsync(request);

        var success = Assert.IsType<
            DetermineRequiredLicenceTypesResult.Success>(result);
        Assert.Same(determination, success.Determination);
        Assert.Equal(["address", "compliance"], sequence);
        Assert.Equal(1, resolver.CallCount);
        Assert.Equal(request.AddressResolutionReference, resolver.Reference);
        Assert.Equal(request.TradingLocation, resolver.TradingLocation);
        Assert.Equal(1, compliance.CallCount);

        ComplianceDeterminationRequest complianceRequest =
            Assert.IsType<ComplianceDeterminationRequest>(
                compliance.Request);
        Assert.Equal(request.LegalOperatorType,
            complianceRequest.LegalOperatorType);
        Assert.Equal(request.TradingLocation,
            complianceRequest.TradingLocation);
        Assert.Same(request.WeeklyOpeningHours,
            complianceRequest.WeeklyOpeningHours);
        Assert.Equal(request.ServiceIncludesHotFood,
            complianceRequest.ServiceIncludesHotFood);
        Assert.Equal(request.AlcoholService,
            complianceRequest.AlcoholService);
        Assert.Same(address.BusinessAddressSnapshot,
            complianceRequest.BusinessAddress);
        Assert.Same(address.FoodRegistrationAuthority,
            complianceRequest.FoodRegistrationAuthority);
        Assert.Same(address.PrimaryTradingAuthority,
            complianceRequest.PrimaryTradingAuthority);
    }

    [Theory]
    [MemberData(nameof(AddressFailures))]
    public async Task VR_DETERMINATION_019_And_020_AddressFailure_IsPreservedWithoutComplianceCall(
        AddressResolutionResult addressFailure,
        Type expectedResultType)
    {
        var compliance = new ProhibitedComplianceDeterminationPort();
        var service = CreateService(
            new RecordingAddressResolver(addressFailure, []),
            compliance);

        DetermineRequiredLicenceTypesResult result =
            await service.DetermineAsync(CreateRequest());

        Assert.IsType(expectedResultType, result);
        Assert.Equal(0, compliance.CallCount);
    }

    [Fact]
    public async Task VR_DETERMINATION_021_TemporaryComplianceFailure_IsReturnedWithoutAssumedDetermination()
    {
        var compliance = new RecordingComplianceDeterminationPort(
            ComplianceDeterminationPortResult.TemporarilyUnavailable(),
            []);
        var service = CreateService(
            new RecordingAddressResolver(
                AddressResolutionResult.Succeeded(CreateAddress()),
                []),
            compliance);

        DetermineRequiredLicenceTypesResult result =
            await service.DetermineAsync(CreateRequest());

        Assert.IsType<
            DetermineRequiredLicenceTypesResult
                .ComplianceDeterminationTemporarilyUnavailable>(result);
        Assert.Equal(1, compliance.CallCount);
    }

    public static TheoryData<AddressResolutionResult, Type> AddressFailures =>
        new()
        {
            {
                AddressResolutionResult.ReferenceIsInvalid(),
                typeof(DetermineRequiredLicenceTypesResult.InvalidReference)
            },
            {
                AddressResolutionResult.InvalidAddress(),
                typeof(DetermineRequiredLicenceTypesResult.InvalidAddressResult)
            },
            {
                AddressResolutionResult.TemporarilyUnavailable(),
                typeof(DetermineRequiredLicenceTypesResult
                    .AddressServiceTemporarilyUnavailable)
            }
        };

    private static DetermineRequiredLicenceTypesService CreateService(
        IAddressResolver resolver,
        IComplianceDeterminationPort compliance) =>
        new(
            new DetermineRequiredLicenceTypesRequestValidator(),
            new AddressResolutionInvoker(resolver),
            compliance);

    private static DetermineRequiredLicenceTypesRequest CreateRequest() =>
        new(
            LegalOperatorType.LimitedCompany,
            TradingLocation.Stall,
            RegisterVendorWeeklyOpeningHours.EveryDay(
                new TimeOnly(9, 0),
                new TimeOnly(17, 0)),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "address-resolution-reference-001");

    private static AddressAuthoritativeValues CreateAddress() =>
        new(
            new CanonicalAddressId("canonical-address-001"),
            new BusinessAddressSnapshot(
                "2 High Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null,
                "Hot Joes"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));

    private static ComplianceDetermination CreateDetermination() =>
        new(
            "epic1-v1",
            Enum.GetValues<RequiredLicenceType>().Select(type =>
                new ComplianceDeterminationItem(
                    type,
                    type == RequiredLicenceType.FoodBusinessRegistration)));

    private sealed class RecordingAddressResolver : IAddressResolver
    {
        private readonly AddressResolutionResult _result;
        private readonly ICollection<string> _sequence;

        public RecordingAddressResolver(
            AddressResolutionResult result,
            ICollection<string> sequence)
        {
            _result = result;
            _sequence = sequence;
        }

        public int CallCount { get; private set; }
        public string? Reference { get; private set; }
        public TradingLocation? TradingLocation { get; private set; }

        public AddressResolutionResult Resolve(
            string addressResolutionReference,
            TradingLocation tradingLocation)
        {
            CallCount++;
            Reference = addressResolutionReference;
            TradingLocation = tradingLocation;
            _sequence.Add("address");
            return _result;
        }
    }

    private sealed class RecordingComplianceDeterminationPort
        : IComplianceDeterminationPort
    {
        private readonly ComplianceDeterminationPortResult _result;
        private readonly ICollection<string> _sequence;

        public RecordingComplianceDeterminationPort(
            ComplianceDeterminationPortResult result,
            ICollection<string> sequence)
        {
            _result = result;
            _sequence = sequence;
        }

        public int CallCount { get; private set; }
        public ComplianceDeterminationRequest? Request { get; private set; }

        public ComplianceDeterminationPortResult Determine(
            ComplianceDeterminationRequest request)
        {
            CallCount++;
            Request = request;
            _sequence.Add("compliance");
            return _result;
        }
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
                "Compliance must not be called after Address failure.");
        }
    }
}
