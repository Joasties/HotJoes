using ComplianceApplication = HotJoes.Application.Compliance;
using VendorApplication = HotJoes.Application.Vendor;
using VendorDomain = HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Vendor.Compliance;

namespace HotJoes.IntegrationTests;

public sealed class ComplianceDeterminationAdapterTests
{
    [Fact]
    public void AI_COMP_002_Success_TranslatesCompleteRequestAndDetermination()
    {
        var complianceDetermination = new ComplianceApplication
            .ComplianceDetermination(
                "epic-1-v1",
                Enum.GetValues<ComplianceApplication.RequiredLicenceType>()
                    .Select(type => new ComplianceApplication
                        .ComplianceDeterminationItem(type, true)));
        var compliance = new RecordingComplianceDeterminationService(
            ComplianceApplication.ComplianceDeterminationResult.Succeeded(
                complianceDetermination));
        VendorApplication.IComplianceDeterminationPort sut =
            new ComplianceDeterminationAdapter(compliance);

        VendorApplication.ComplianceDeterminationPortResult actual =
            sut.Determine(CreateVendorRequest());

        var success = Assert.IsType<
            VendorApplication.ComplianceDeterminationPortResult.Success>(actual);
        Assert.Equal("epic-1-v1", success.Determination.RuleSetVersion);
        Assert.Equal(
            Enum.GetValues<VendorApplication.RequiredLicenceType>(),
            success.Determination.Items.Select(item =>
                item.RequiredLicenceType));
        Assert.All(success.Determination.Items, item =>
            Assert.True(item.IsRequired));

        ComplianceApplication.ComplianceDeterminationRequest request =
            Assert.IsType<ComplianceApplication.ComplianceDeterminationRequest>(
                compliance.ReceivedRequest);
        Assert.Equal(
            ComplianceApplication.LegalOperatorType.LimitedCompany,
            request.LegalOperatorType);
        Assert.Equal(
            ComplianceApplication.TradingLocation.Stall,
            request.TradingLocation);
        Assert.Equal(
            Enum.GetValues<ComplianceApplication.TradingDay>(),
            request.WeeklyOpeningHours.Days.Select(day => day.Day));
        Assert.All(request.WeeklyOpeningHours.Days, day =>
        {
            Assert.False(day.IsClosed);
            Assert.False(day.IsOpenAllDay);
            Assert.Equal(new TimeOnly(9, 0), day.StartTime);
            Assert.Equal(new TimeOnly(17, 0), day.EndTime);
        });
        Assert.True(request.ServiceIncludesHotFood);
        Assert.True(request.AlcoholService);
        Assert.Equal("2 High Street", request.BusinessAddress.AddressLine1);
        Assert.Equal("GREENWICH", request.BusinessAddress.PostTown);
        Assert.Equal("SE10 8AA", request.BusinessAddress.Postcode);
        Assert.Equal(
            "Royal Borough of Greenwich",
            request.FoodRegistrationAuthority);
        Assert.Equal(
            "Royal Borough of Greenwich",
            request.PrimaryTradingAuthority);
        Assert.Equal(1, compliance.InvocationCount);
    }

    [Fact]
    public void AI_COMP_002_UnsupportedCoverage_MapsToVendorUnsupportedResult()
    {
        var compliance = new RecordingComplianceDeterminationService(
            ComplianceApplication.ComplianceDeterminationResult.Unsupported());
        VendorApplication.IComplianceDeterminationPort sut =
            new ComplianceDeterminationAdapter(compliance);

        VendorApplication.ComplianceDeterminationPortResult actual =
            sut.Determine(CreateVendorRequest());

        Assert.IsType<VendorApplication.ComplianceDeterminationPortResult
            .UnsupportedDetermination>(actual);
    }

    [Fact]
    public void AI_COMP_002_TemporaryFailure_MapsToVendorTemporaryResult()
    {
        var compliance = new RecordingComplianceDeterminationService(
            ComplianceApplication.ComplianceDeterminationResult
                .TemporarilyUnavailable());
        VendorApplication.IComplianceDeterminationPort sut =
            new ComplianceDeterminationAdapter(compliance);

        VendorApplication.ComplianceDeterminationPortResult actual =
            sut.Determine(CreateVendorRequest());

        Assert.IsType<VendorApplication.ComplianceDeterminationPortResult
            .ComplianceDeterminationTemporarilyUnavailable>(actual);
    }

    private static VendorApplication.ComplianceDeterminationRequest
        CreateVendorRequest() =>
        new(
            VendorDomain.LegalOperatorType.LimitedCompany,
            VendorDomain.TradingLocation.Stall,
            VendorApplication.RegisterVendorWeeklyOpeningHours.EveryDay(
                new TimeOnly(9, 0),
                new TimeOnly(17, 0)),
            serviceIncludesHotFood: true,
            alcoholService: true,
            new VendorDomain.BusinessAddressSnapshot(
                "2 High Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null,
                "Hot Joes"),
            new VendorDomain.FoodRegistrationAuthority(
                "Royal Borough of Greenwich"),
            new VendorDomain.PrimaryTradingAuthority(
                "Royal Borough of Greenwich"));

    private sealed class RecordingComplianceDeterminationService
        : ComplianceApplication.IComplianceDeterminationService
    {
        private readonly ComplianceApplication.ComplianceDeterminationResult
            _result;

        public RecordingComplianceDeterminationService(
            ComplianceApplication.ComplianceDeterminationResult result)
        {
            _result = result;
        }

        public int InvocationCount { get; private set; }
        public ComplianceApplication.ComplianceDeterminationRequest?
            ReceivedRequest
        { get; private set; }

        public ComplianceApplication.ComplianceDeterminationResult Determine(
            ComplianceApplication.ComplianceDeterminationRequest request)
        {
            InvocationCount++;
            ReceivedRequest = request;
            return _result;
        }
    }
}
