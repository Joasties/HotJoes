using HotJoes.Application.Compliance;

namespace HotJoes.IntegrationTests;

public sealed class ComplianceDeterminationStubTests
{
    private const string SupportedAuthority =
        "Royal Borough of Greenwich";

    [Fact]
    public void VR_DETERMINATION_005_EquivalentCanonicalInput_IsDeterministicAndVersioned()
    {
        var stub = new StubComplianceApplication();
        ComplianceDeterminationRequest canonical = CreateRequest();
        ComplianceDeterminationRequest reordered = CreateRequest(
            weeklyOpeningHours: new WeeklyOpeningHours(
                canonical.WeeklyOpeningHours.Days.Reverse()));

        ComplianceDetermination first = Success(stub.Determine(canonical));
        ComplianceDetermination second = Success(stub.Determine(reordered));

        Assert.False(string.IsNullOrWhiteSpace(first.RuleSetVersion));
        Assert.Equal(first.RuleSetVersion, second.RuleSetVersion);
        Assert.Equal(first.Items, second.Items);
    }

    [Fact]
    public void VR_DETERMINATION_006_Result_IsCompleteDuplicateFreeAndCanonicallyOrdered()
    {
        ComplianceDetermination determination = Determine(CreateRequest());

        Assert.Equal(
            Enum.GetValues<RequiredLicenceType>(),
            determination.Items.Select(item => item.RequiredLicenceType));
        Assert.Equal(5, determination.Items.Count);
        Assert.Equal(
            5,
            determination.Items
                .Select(item => item.RequiredLicenceType)
                .Distinct()
                .Count());
    }

    [Theory]
    [InlineData(TradingLocation.Restaurant, false, false)]
    [InlineData(TradingLocation.Restaurant, true, true)]
    [InlineData(TradingLocation.Kitchen, false, true)]
    [InlineData(TradingLocation.Stall, true, false)]
    public void VR_DETERMINATION_007_FoodBusinessRegistration_IsAlwaysRequired(
        TradingLocation location,
        bool hotFood,
        bool alcohol)
    {
        ComplianceDetermination determination = Determine(CreateRequest(
            tradingLocation: location,
            serviceIncludesHotFood: hotFood,
            alcoholService: alcohol,
            primaryTradingAuthority: location == TradingLocation.Stall
                ? SupportedAuthority
                : null));

        AssertRequired(
            determination,
            RequiredLicenceType.FoodBusinessRegistration,
            expected: true);
    }

    [Theory]
    [InlineData(TradingLocation.Restaurant, false)]
    [InlineData(TradingLocation.Kitchen, false)]
    [InlineData(TradingLocation.Stall, true)]
    public void VR_DETERMINATION_008_StreetTradingLicence_RequiresStallAndAuthority(
        TradingLocation location,
        bool expected)
    {
        ComplianceDetermination determination = Determine(CreateRequest(
            tradingLocation: location,
            primaryTradingAuthority: location == TradingLocation.Stall
                ? SupportedAuthority
                : null));

        AssertRequired(
            determination,
            RequiredLicenceType.StreetTradingLicence,
            expected);
    }

    [Theory]
    [MemberData(nameof(LateNightSchedules))]
    public void VR_DETERMINATION_009_To_011_LateNightRule_UsesHotFoodAndExactOverlap(
        WeeklyOpeningHours hours,
        bool hotFood,
        bool expected)
    {
        ComplianceDetermination determination = Determine(CreateRequest(
            weeklyOpeningHours: hours,
            serviceIncludesHotFood: hotFood));

        AssertRequired(
            determination,
            RequiredLicenceType.LateNightRefreshmentLicence,
            expected);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void VR_DETERMINATION_012_AlcoholServiceControlsBothLicenceItems(
        bool alcoholService)
    {
        ComplianceDetermination determination = Determine(CreateRequest(
            alcoholService: alcoholService));

        AssertRequired(
            determination,
            RequiredLicenceType.PremisesLicence,
            alcoholService);
        AssertRequired(
            determination,
            RequiredLicenceType.PersonalLicenceHolder,
            alcoholService);
    }

    [Fact]
    public void VR_DETERMINATION_013_UnsupportedAuthorityCoverage_FailsClosed()
    {
        var stub = new StubComplianceApplication();

        ComplianceDeterminationResult result = stub.Determine(CreateRequest(
            foodRegistrationAuthority: "Unsupported Synthetic Authority"));

        Assert.IsType<ComplianceDeterminationResult.UnsupportedCoverage>(result);
    }

    [Fact]
    public void VR_DETERMINATION_022_And_AI_COMP_003_StubIsApplicationOwnedAndHasNoSideEffectDependencies()
    {
        Type stubType = typeof(StubComplianceApplication);
        string[] prohibitedAssemblyFragments =
        [
            "EntityFrameworkCore",
            "HttpClient",
            "Npgsql",
            "RabbitMQ"
        ];

        Assert.Equal(
            "HotJoes.Application.Compliance",
            stubType.Assembly.GetName().Name);
        Assert.Empty(stubType.GetFields(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic));
        Assert.DoesNotContain(
            stubType.Assembly.GetReferencedAssemblies(),
            reference => prohibitedAssemblyFragments.Any(fragment =>
                reference.Name?.Contains(
                    fragment,
                    StringComparison.OrdinalIgnoreCase) is true));
    }

    public static TheoryData<WeeklyOpeningHours, bool, bool>
        LateNightSchedules =>
        new()
        {
            { AllClosed(), true, false },
            { WithMondayOpenAllDay(), true, true },
            { WithMondayInterval(9, 0, 17, 0), true, false },
            { WithMondayInterval(5, 0, 23, 0), true, false },
            { WithMondayInterval(22, 0, 23, 0), true, false },
            { WithMondayInterval(23, 0, 5, 0), true, true },
            { WithMondayInterval(23, 0, 0, 0), true, true },
            { WithMondayInterval(0, 0, 5, 0), true, true },
            { WithMondayInterval(22, 0, 4, 0), true, true },
            { WithMondayInterval(23, 0, 5, 0), false, false },
            { WithMondayOpenAllDay(), false, false }
        };

    private static ComplianceDetermination Determine(
        ComplianceDeterminationRequest request) =>
        Success(new StubComplianceApplication().Determine(request));

    private static ComplianceDetermination Success(
        ComplianceDeterminationResult result) =>
        Assert.IsType<ComplianceDeterminationResult.Success>(result)
            .Determination;

    private static void AssertRequired(
        ComplianceDetermination determination,
        RequiredLicenceType type,
        bool expected)
    {
        ComplianceDeterminationItem item = Assert.Single(
            determination.Items,
            item => item.RequiredLicenceType == type);
        Assert.Equal(expected, item.IsRequired);
    }

    private static ComplianceDeterminationRequest CreateRequest(
        TradingLocation tradingLocation = TradingLocation.Restaurant,
        WeeklyOpeningHours? weeklyOpeningHours = null,
        bool serviceIncludesHotFood = false,
        bool alcoholService = false,
        string foodRegistrationAuthority = SupportedAuthority,
        string? primaryTradingAuthority = null) =>
        new(
            LegalOperatorType.LimitedCompany,
            tradingLocation,
            weeklyOpeningHours ?? EveryDay(9, 0, 17, 0),
            serviceIncludesHotFood,
            alcoholService,
            new BusinessAddress(
                "2 High Street",
                null,
                null,
                "GREENWICH",
                "SE10 8AA",
                null,
                "Hot Joes"),
            foodRegistrationAuthority,
            primaryTradingAuthority);

    private static WeeklyOpeningHours EveryDay(
        int startHour,
        int startMinute,
        int endHour,
        int endMinute) =>
        new(Enum.GetValues<TradingDay>().Select(day =>
            new DailyOpeningHours(
                day,
                false,
                false,
                new TimeOnly(startHour, startMinute),
                new TimeOnly(endHour, endMinute))));

    private static WeeklyOpeningHours AllClosed() =>
        new(Enum.GetValues<TradingDay>().Select(day =>
            new DailyOpeningHours(day, true, false, null, null)));

    private static WeeklyOpeningHours WithMondayOpenAllDay() =>
        WithMonday(new DailyOpeningHours(
            TradingDay.Monday,
            false,
            true,
            null,
            null));

    private static WeeklyOpeningHours WithMondayInterval(
        int startHour,
        int startMinute,
        int endHour,
        int endMinute) =>
        WithMonday(new DailyOpeningHours(
            TradingDay.Monday,
            false,
            false,
            new TimeOnly(startHour, startMinute),
            new TimeOnly(endHour, endMinute)));

    private static WeeklyOpeningHours WithMonday(DailyOpeningHours monday) =>
        new(Enum.GetValues<TradingDay>().Select(day =>
            day == TradingDay.Monday
                ? monday
                : new DailyOpeningHours(day, true, false, null, null)));
}
