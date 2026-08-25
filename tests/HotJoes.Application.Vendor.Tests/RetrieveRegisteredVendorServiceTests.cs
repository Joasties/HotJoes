using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RetrieveRegisteredVendorServiceTests
{
    [Fact]
    public async Task RetrieveAsync_ExistingVendor_LoadsByIdAndReturnsMappedDetails()
    {
        VendorAggregate vendor = CreateVendor();
        var repository = new RecordingVendorRepository(vendor);
        var service = new RetrieveRegisteredVendorService(
            repository,
            new RegisteredVendorDetailsMapper());
        using var cancellationSource = new CancellationTokenSource();

        RetrieveRegisteredVendorResult result = await service.RetrieveAsync(
            vendor.Id,
            cancellationSource.Token);

        var found = Assert.IsType<RetrieveRegisteredVendorResult.Found>(result);
        Assert.Equal(vendor.Id, found.Details.VendorId);
        Assert.Equal(
            vendor.RegisteredInformation.TradingName.Value,
            found.Details.TradingName);
        Assert.Equal(1, repository.FindInvocationCount);
        Assert.Equal(vendor.Id, repository.RequestedVendorId);
        Assert.Equal(
            cancellationSource.Token,
            repository.CancellationToken);
        Assert.Equal(0, repository.AddInvocationCount);
    }

    [Fact]
    public async Task RetrieveAsync_UnknownVendor_ReturnsControlledNotFound()
    {
        var repository = new RecordingVendorRepository(vendor: null);
        var service = new RetrieveRegisteredVendorService(
            repository,
            new RegisteredVendorDetailsMapper());
        var vendorId = new VendorId(
            Guid.Parse("2268c7b9-89de-4428-bc1d-9270ae8a8719"));

        RetrieveRegisteredVendorResult result = await service.RetrieveAsync(
            vendorId,
            CancellationToken.None);

        Assert.IsType<RetrieveRegisteredVendorResult.NotFound>(result);
        Assert.Equal(1, repository.FindInvocationCount);
        Assert.Equal(vendorId, repository.RequestedVendorId);
        Assert.Equal(0, repository.AddInvocationCount);
    }

    [Fact]
    public void PublicSurface_IsClosedTransportIndependentAndUsesOnlyApprovedCollaborators()
    {
        Type resultType = typeof(RetrieveRegisteredVendorResult);
        Type[] outcomes = resultType
            .GetNestedTypes(BindingFlags.Public)
            .OrderBy(type => type.Name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[]
            {
                typeof(RetrieveRegisteredVendorResult.Found),
                typeof(RetrieveRegisteredVendorResult.NotFound)
            },
            outcomes);
        Assert.True(resultType.IsAbstract);
        Assert.All(
            outcomes,
            outcome => Assert.True(outcome.IsSealed));

        PropertyInfo detailsProperty = Assert.Single(
            typeof(RetrieveRegisteredVendorResult.Found)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public));
        Assert.Equal("Details", detailsProperty.Name);
        Assert.Equal(typeof(RegisteredVendorDetails), detailsProperty.PropertyType);
        Assert.Null(detailsProperty.SetMethod);
        Assert.Empty(
            typeof(RetrieveRegisteredVendorResult.NotFound)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public));

        ConstructorInfo constructor = Assert.Single(
            typeof(RetrieveRegisteredVendorService).GetConstructors());
        Assert.Equal(
            new[]
            {
                typeof(IVendorRepository),
                typeof(RegisteredVendorDetailsMapper)
            },
            constructor
                .GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray());

        Assert.DoesNotContain(
            outcomes.SelectMany(type => type.GetProperties()),
            property => property.PropertyType == typeof(VendorAggregate));
        Assert.DoesNotContain(
            outcomes.SelectMany(type => type.GetProperties()),
            property =>
                property.PropertyType.Namespace?.Contains(
                    "Infrastructure",
                    StringComparison.Ordinal) is true);
    }

    private static VendorAggregate CreateVendor()
    {
        var information = new VendorRegistrationInformation(
            LegalOperatorType.SoleTrader,
            new VendorName("Retrieval Service Operator"),
            new VendorName("Retrieval Service Kitchen"),
            companyRegistrationNumber: null,
            new PrimaryContact(
                "Jordan Smith",
                new EmailAddress("jordan@example.test"),
                new TelephoneNumber("+44 20 7946 0123")),
            new CanonicalAddressId(
                "canonical-address-retrieval-service"),
            new BusinessAddressSnapshot(
                "32 Example Street",
                addressLine2: null,
                addressLine3: null,
                "LONDON",
                "AB1 2CD",
                county: null,
                recipientOrOrganisationName: null),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            primaryTradingAuthority: null,
            new TradingCharacteristics(
                TradingLocation.Kitchen,
                new OpeningHours(
                    new TimeOnly(17, 0),
                    new TimeOnly(2, 0)),
                serviceIncludesHotFood: true,
                alcoholService: false));

        return VendorAggregate.Register(
            new VendorId(
                Guid.Parse("7b43063f-3469-4b7a-bcc2-66300bebaf7c")),
            information,
            website: null,
            businessDescription: null,
            new DateTimeOffset(2026, 8, 25, 21, 0, 0, TimeSpan.Zero));
    }

    private sealed class RecordingVendorRepository : IVendorRepository
    {
        private readonly VendorAggregate? _vendor;

        public RecordingVendorRepository(VendorAggregate? vendor)
        {
            _vendor = vendor;
        }

        public int FindInvocationCount { get; private set; }

        public int AddInvocationCount { get; private set; }

        public VendorId? RequestedVendorId { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public Task<VendorAggregate?> FindAsync(
            VendorId vendorId,
            CancellationToken cancellationToken)
        {
            FindInvocationCount++;
            RequestedVendorId = vendorId;
            CancellationToken = cancellationToken;
            return Task.FromResult(_vendor);
        }

        public Task AddAsync(
            VendorAggregate vendor,
            CancellationToken cancellationToken)
        {
            AddInvocationCount++;
            throw new InvalidOperationException(
                "Retrieval must not add or save a Vendor.");
        }
    }
}
