using System.Data.Common;
using HotJoes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class VendorRegistrationPostgreSqlEnforcementTests
{
    private readonly PostgreSqlFixture _fixture;

    public VendorRegistrationPostgreSqlEnforcementTests(
        PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CompositeIdentity_RejectsSecondVendorWithSameNormalizedIdentity()
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        context.Set<VendorRegistrationRecord>().Add(CreateVendorRecord(
            Guid.Parse("2d139d42-df36-42bd-88f2-671a5ea4e982"),
            "identity-unique-address"));
        await context.SaveChangesAsync();

        context.Set<VendorRegistrationRecord>().Add(CreateVendorRecord(
            Guid.Parse("cbd5af1f-60da-41b7-8a24-ee84eef1ae1c"),
            "identity-unique-address"));

        await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());
    }

    [Fact]
    public async Task RequiredOwnership_RejectsOrphanOutcomeAndOutboxRecords()
    {
        Guid absentVendorId =
            Guid.Parse("b5295c15-f7fa-4712-98d8-ed340d982209");

        await using (VendorRegistrationDbContext outcomeContext = CreateContext())
        {
            await outcomeContext.Database.EnsureCreatedAsync();
            outcomeContext.Set<VendorRegistrationOutcomeRecord>().Add(
                CreateOutcomeRecord(absentVendorId));

            await Assert.ThrowsAsync<DbUpdateException>(
                () => outcomeContext.SaveChangesAsync());
        }

        await using (VendorRegistrationDbContext outboxContext = CreateContext())
        {
            outboxContext.Set<VendorRegistrationOutboxRecord>().Add(
                CreateOutboxRecord(
                    Guid.Parse("21b8ec23-feb3-4151-9975-f55677ea36d9"),
                    absentVendorId));

            await Assert.ThrowsAsync<DbUpdateException>(
                () => outboxContext.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task VendorDeletion_WithOwnedOutcomeAndOutbox_IsRestricted()
    {
        Guid vendorId = Guid.Parse("5598d3e6-35f9-469a-8728-c0b7cd970fb4");
        Guid eventId = Guid.Parse("aa237c38-2dbc-440d-be8c-d814873e7d47");

        await using (VendorRegistrationDbContext arrangeContext = CreateContext())
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Set<VendorRegistrationRecord>().Add(
                CreateVendorRecord(vendorId, "restrict-delete-address"));
            arrangeContext.Set<VendorRegistrationOutcomeRecord>().Add(
                CreateOutcomeRecord(vendorId));
            arrangeContext.Set<VendorRegistrationOutboxRecord>().Add(
                CreateOutboxRecord(eventId, vendorId));
            await arrangeContext.SaveChangesAsync();
        }

        await using (VendorRegistrationDbContext deleteContext = CreateContext())
        {
            VendorRegistrationRecord vendor = await deleteContext
                .Set<VendorRegistrationRecord>()
                .SingleAsync(record => record.VendorId == vendorId);
            deleteContext.Remove(vendor);

            await Assert.ThrowsAsync<DbUpdateException>(
                () => deleteContext.SaveChangesAsync());
        }

        await using VendorRegistrationDbContext verifyContext = CreateContext();
        Assert.True(await verifyContext.Set<VendorRegistrationRecord>()
            .AnyAsync(record => record.VendorId == vendorId));
        Assert.True(await verifyContext.Set<VendorRegistrationOutcomeRecord>()
            .AnyAsync(record => record.VendorId == vendorId));
        Assert.True(await verifyContext.Set<VendorRegistrationOutboxRecord>()
            .AnyAsync(record => record.EventId == eventId));
    }

    [Fact]
    public async Task ByteConstraints_RejectInvalidFingerprintAndEmptySerializedEvent()
    {
        Guid fingerprintVendorId =
            Guid.Parse("0f1765c1-b01d-4908-afbb-d48c9201c11d");

        await using (VendorRegistrationDbContext fingerprintContext =
            CreateContext())
        {
            await fingerprintContext.Database.EnsureCreatedAsync();
            fingerprintContext.Set<VendorRegistrationRecord>().Add(
                CreateVendorRecord(
                    fingerprintVendorId,
                    "invalid-fingerprint-address"));
            fingerprintContext.Set<VendorRegistrationOutcomeRecord>().Add(
                new VendorRegistrationOutcomeRecord
                {
                    VendorId = fingerprintVendorId,
                    FingerprintVersion = 1,
                    SemanticFingerprintSha256 = new byte[31],
                    ResultVendorState = "pendingActivation"
                });

            await Assert.ThrowsAsync<DbUpdateException>(
                () => fingerprintContext.SaveChangesAsync());
        }

        Guid outboxVendorId =
            Guid.Parse("f0858bf8-e9ae-42b8-baf3-8d373b47a06e");

        await using (VendorRegistrationDbContext outboxContext = CreateContext())
        {
            outboxContext.Set<VendorRegistrationRecord>().Add(
                CreateVendorRecord(
                    outboxVendorId,
                    "empty-outbox-address"));
            outboxContext.Set<VendorRegistrationOutboxRecord>().Add(
                new VendorRegistrationOutboxRecord
                {
                    EventId = Guid.Parse(
                        "d26de1c9-7b60-46ee-b590-7987ce5aaf3f"),
                    VendorId = outboxVendorId,
                    EventVersion = 1,
                    SerializedEvent = [],
                    PublishedAtUtc = null
                });

            await Assert.ThrowsAsync<DbUpdateException>(
                () => outboxContext.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task PostgreSqlSchema_ContainsRequiredNamedIndexesAndUnpublishedPredicate()
    {
        await using VendorRegistrationDbContext context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        await context.Database.OpenConnectionAsync();

        await using DbCommand command =
            context.Database.GetDbConnection().CreateCommand();
        command.CommandText =
            """
            SELECT indexname, indexdef
            FROM pg_indexes
            WHERE schemaname = current_schema()
              AND tablename IN (
                  'vendor_registrations',
                  'vendor_registration_outcomes',
                  'vendor_registration_outbox')
            """;

        var definitions = new Dictionary<string, string>(
            StringComparer.Ordinal);
        await using DbDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            definitions.Add(reader.GetString(0), reader.GetString(1));
        }

        AssertPrimaryKeyIndex(
            definitions.Values,
            "vendor_registrations",
            "vendor_id");
        AssertPrimaryKeyIndex(
            definitions.Values,
            "vendor_registration_outcomes",
            "vendor_id");
        AssertPrimaryKeyIndex(
            definitions.Values,
            "vendor_registration_outbox",
            "event_id");
        Assert.Contains("uq_vendor_registrations_identity", definitions.Keys);
        Assert.Contains(
            "uq_vendor_registration_outbox_vendor_id",
            definitions.Keys);
        Assert.Contains(
            "ix_vendor_registration_outbox_unpublished",
            definitions.Keys);
        Assert.Contains(
            "WHERE (published_at_utc IS NULL)",
            definitions["ix_vendor_registration_outbox_unpublished"],
            StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertPrimaryKeyIndex(
        IEnumerable<string> indexDefinitions,
        string tableName,
        string columnName)
    {
        Assert.Contains(
            indexDefinitions,
            definition =>
                definition.Contains(
                    $" ON public.{tableName} ",
                    StringComparison.OrdinalIgnoreCase)
                && definition.EndsWith(
                    $"({columnName})",
                    StringComparison.OrdinalIgnoreCase));
    }

    private VendorRegistrationDbContext CreateContext()
    {
        DbContextOptions<VendorRegistrationDbContext> options =
            new DbContextOptionsBuilder<VendorRegistrationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

        return new VendorRegistrationDbContext(options);
    }

    private static VendorRegistrationRecord CreateVendorRecord(
        Guid vendorId,
        string canonicalAddressId)
    {
        return new VendorRegistrationRecord
        {
            VendorId = vendorId,
            VendorState = "pendingActivation",
            TradingPreference = "offline",
            RegisteredAtUtc = new DateTimeOffset(
                2026,
                8,
                25,
                12,
                0,
                0,
                TimeSpan.Zero),
            LegalOperatorType = "soleTrader",
            LegalOperatorName = "Constraint Test Operator",
            NormalizedLegalOperatorName = "constraint test operator",
            TradingName = "Constraint Test Kitchen",
            NormalizedTradingName = "constraint test kitchen",
            CompanyRegistrationNumber = null,
            ContactName = "Primary Contact",
            ContactEmail = "contact@example.test",
            ContactTelephone = "+44 20 7946 0123",
            CanonicalAddressId = canonicalAddressId,
            RecipientOrOrganisationName = null,
            AddressLine1 = "10 Example Street",
            AddressLine2 = null,
            AddressLine3 = null,
            PostTown = "LONDON",
            Postcode = "AB1 2CD",
            County = null,
            FoodRegistrationAuthority = "Greenwich Borough Council",
            PrimaryTradingAuthority = null,
            TradingLocation = "kitchen",
            OpeningHoursStart = new TimeOnly(8, 0),
            OpeningHoursEnd = new TimeOnly(22, 0),
            ServiceIncludesHotFood = true,
            AlcoholService = false,
            Website = null,
            BusinessDescription = null
        };
    }

    private static VendorRegistrationOutcomeRecord CreateOutcomeRecord(
        Guid vendorId)
    {
        return new VendorRegistrationOutcomeRecord
        {
            VendorId = vendorId,
            FingerprintVersion = 1,
            SemanticFingerprintSha256 = new byte[32],
            ResultVendorState = "pendingActivation"
        };
    }

    private static VendorRegistrationOutboxRecord CreateOutboxRecord(
        Guid eventId,
        Guid vendorId)
    {
        return new VendorRegistrationOutboxRecord
        {
            EventId = eventId,
            VendorId = vendorId,
            EventVersion = 1,
            SerializedEvent = [1],
            PublishedAtUtc = null
        };
    }
}
