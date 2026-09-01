using System.Diagnostics;
using System.Text;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VendorAggregate = HotJoes.Domain.Vendor.Vendor;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class PostgreSqlOutboxTraceContextTests
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlOutboxTraceContextTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_CurrentW3CActivity_PersistsTraceMetadataWithoutChangingEvent()
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        NewVendorRegistrationCommit commit = CreateCommit();
        var serializer = new VendorRegisteredIntegrationEventSerializer();
        byte[] expectedBytes = serializer
            .Serialize(commit.IntegrationEvent)
            .SerializedEvent
            .ToArray();
        INewVendorRegistrationCommitter committer =
            new PostgreSqlNewVendorRegistrationCommitter(context, serializer);

        using var activity = new Activity("register-vendor")
            .SetIdFormat(ActivityIdFormat.W3C)
            .SetParentId(
                "00-4bf92f3577b34da6a3ce929d0e0e4736-" +
                "00f067aa0ba902b7-01")
            .Start();
        activity.TraceStateString = "vendor=hotjoes";

        await committer.CommitAsync(commit, CancellationToken.None);
        context.ChangeTracker.Clear();

        VendorRegistrationOutboxRecord persisted = await context
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .SingleAsync(record =>
                record.EventId == commit.IntegrationEvent.EventId);

        Assert.Equal(activity.Id, persisted.TraceParent);
        Assert.Equal(activity.TraceStateString, persisted.TraceState);
        Assert.Equal(expectedBytes, persisted.SerializedEvent);
        string persistedJson = Encoding.UTF8.GetString(
            persisted.SerializedEvent);
        Assert.DoesNotContain(
            "traceparent",
            persistedJson,
            StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "tracestate",
            persistedJson,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CommitAsync_NoCurrentActivity_PersistsNullTraceMetadata()
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        NewVendorRegistrationCommit commit = CreateCommit();
        var serializer = new VendorRegisteredIntegrationEventSerializer();
        INewVendorRegistrationCommitter committer =
            new PostgreSqlNewVendorRegistrationCommitter(context, serializer);
        Activity? previous = Activity.Current;

        try
        {
            Activity.Current = null;
            await committer.CommitAsync(commit, CancellationToken.None);
        }
        finally
        {
            Activity.Current = previous;
        }

        context.ChangeTracker.Clear();
        VendorRegistrationOutboxRecord persisted = await context
            .Set<VendorRegistrationOutboxRecord>()
            .AsNoTracking()
            .SingleAsync(record =>
                record.EventId == commit.IntegrationEvent.EventId);

        Assert.Null(persisted.TraceParent);
        Assert.Null(persisted.TraceState);
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static NewVendorRegistrationCommit CreateCommit()
    {
        string uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
        RegisterVendorCommand command = CreateCommand(uniqueSuffix);
        AddressAuthoritativeValues addressValues = CreateAddressValues(
            uniqueSuffix);
        VendorAggregate vendor = CreateVendor(command, addressValues);
        var originalResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(vendor.Id));
        VendorRegistered completedFact = Assert.IsType<VendorRegistered>(
            Assert.Single(vendor.DomainEvents));
        VendorRegisteredIntegrationEvent integrationEvent =
            new VendorRegisteredIntegrationEventMapper().Map(
                completedFact,
                vendor,
                Guid.NewGuid(),
                vendor.RegisteredAt);

        return new NewVendorRegistrationCommit(
            vendor,
            VendorRegistrationIdentity.Create(command, addressValues),
            RegistrationSemanticFingerprint.Create(command, addressValues),
            originalResult,
            integrationEvent);
    }

    private static RegisterVendorCommand CreateCommand(string uniqueSuffix)
    {
        return new RegisterVendorCommand(
            $"Trace Context Kitchen {uniqueSuffix}",
            $"Trace Context Operator {uniqueSuffix} Ltd",
            LegalOperatorType.LimitedCompany,
            "SC123456",
            TradingLocation.Stall,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            serviceIncludesHotFood: true,
            alcoholService: false,
            "Alex Morgan",
            "alex@example.test",
            "+442079460123",
            "address-resolution-trace-context",
            "https://trace-context.example.test",
            "Hot food market stall.",
            authorisedToRegisterBusiness: true,
            informationAccurate: true,
            acceptHotJoesPlatformTerms: true);
    }

    private static AddressAuthoritativeValues CreateAddressValues(
        string uniqueSuffix)
    {
        return new AddressAuthoritativeValues(
            new CanonicalAddressId(
                $"canonical-address-trace-context-{uniqueSuffix}"),
            new BusinessAddressSnapshot(
                "2 High Street",
                "Market Square",
                "Stall 7",
                "GREENWICH",
                "SE10 8AA",
                "Greater London",
                $"Trace Context Kitchen {uniqueSuffix}"),
            new FoodRegistrationAuthority("Greenwich Borough Council"),
            new PrimaryTradingAuthority("Greenwich Borough Council"));
    }

    private static VendorAggregate CreateVendor(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
    {
        var information = new VendorRegistrationInformation(
            command.LegalOperatorType,
            new VendorName(command.LegalOperatorName),
            new VendorName(command.TradingName),
            new CompanyRegistrationNumber(command.CompanyRegistrationNumber!),
            new PrimaryContact(
                command.ContactName,
                new EmailAddress(command.ContactEmail),
                new TelephoneNumber(command.ContactTelephone)),
            addressValues.CanonicalAddressId,
            addressValues.BusinessAddressSnapshot,
            addressValues.FoodRegistrationAuthority,
            addressValues.PrimaryTradingAuthority,
            new TradingCharacteristics(
                command.TradingLocation,
                new OpeningHours(
                    command.OpeningHoursStartTime,
                    command.OpeningHoursEndTime),
                command.ServiceIncludesHotFood,
                command.AlcoholService));

        return VendorAggregate.Register(
            new VendorId(Guid.NewGuid()),
            information,
            new Uri(command.Website!),
            command.BusinessDescription,
            new DateTimeOffset(2026, 8, 29, 10, 0, 0, TimeSpan.Zero));
    }
}
