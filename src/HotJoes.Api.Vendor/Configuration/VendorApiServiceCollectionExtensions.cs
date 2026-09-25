using HotJoes.Api.Vendor.AddressBootstrap;
using HotJoes.Application.Address;
using HotJoes.Application.Community;
using HotJoes.Application.Compliance;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Community.Persistence;
using HotJoes.Infrastructure.Vendor.Persistence;
using HotJoes.Infrastructure.Vendor.Address;
using HotJoes.Infrastructure.Vendor.Compliance;
using Microsoft.EntityFrameworkCore;

namespace HotJoes.Api.Vendor.Configuration;

public static class VendorApiServiceCollectionExtensions
{
    public static IServiceCollection AddVendorApiComposition(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentRootPath)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentRootPath);

        string vendorDatabase = configuration.GetConnectionString(
            "VendorDatabase") ?? throw new InvalidOperationException(
                "ConnectionStrings:VendorDatabase is required.");
        string communityDatabase = configuration.GetConnectionString(
            "CommunityDatabase") ?? throw new InvalidOperationException(
                "ConnectionStrings:CommunityDatabase is required.");

        services.AddDbContext<VendorRegistrationDbContext>(options =>
            options.UseNpgsql(vendorDatabase));
        services.AddDbContext<CommunityPersistenceDbContext>(options =>
            options.UseNpgsql(communityDatabase));

        services.AddSingleton<StubAddressApplication>(_ =>
        {
            string configuredPath = configuration[
                "AddressBootstrap:CataloguePath"]
                ?? throw new InvalidOperationException(
                    "AddressBootstrap:CataloguePath is required.");
            string path = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(contentRootPath, configuredPath);

            return AddressBootstrapCatalogue
                .Load(path)
                .CreateAddressApplication();
        });
        services.AddSingleton<IAddressResolutionService>(services =>
            services.GetRequiredService<StubAddressApplication>());
        services.AddSingleton<IAddressSearchService>(services =>
            services.GetRequiredService<StubAddressApplication>());
        services.AddScoped<IAddressResolver, AddressResolutionAdapter>();

        services.AddSingleton<StubComplianceApplication>();
        services.AddSingleton<IComplianceDeterminationService>(services =>
            services.GetRequiredService<StubComplianceApplication>());
        services.AddSingleton<IComplianceDeterminationPort,
            ComplianceDeterminationAdapter>();
        services.AddSingleton<DetermineRequiredLicenceTypesRequestValidator>();
        services.AddScoped<IDetermineRequiredLicenceTypesService,
            DetermineRequiredLicenceTypesService>();

        services.AddSingleton<IRegisterVendorCommandValidator,
            RegisterVendorCommandValidator>();
        services.AddScoped<AddressResolutionInvoker>();
        services.AddScoped<IRegistrationOutcomeDeterminer,
            PostgreSqlRegistrationOutcomeDeterminer>();
        services.AddSingleton<VendorRegisteredIntegrationEventMapper>();
        services.AddSingleton<VendorRegisteredIntegrationEventSerializer>();
        services.AddScoped<INewVendorRegistrationCommitter,
            PostgreSqlNewVendorRegistrationCommitter>();
        services.AddSingleton<IRegistrationIdentifierGenerator,
            SystemRegistrationIdentifierGenerator>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<INewVendorRegistrationProcessor,
            NewVendorRegistrationProcessor>();
        services.AddScoped<IRegisterVendorService, RegisterVendorService>();

        services.AddScoped<IVendorRepository, PostgreSqlVendorRepository>();
        services.AddSingleton<RegisteredVendorDetailsMapper>();
        services.AddScoped<IRetrieveRegisteredVendorService,
            RetrieveRegisteredVendorService>();

        services.AddScoped<IVendorRegistrationVerificationPort,
            PostgreSqlVendorRegistrationVerificationAdapter>();
        services.AddSingleton<JoinCommunityRequestValidator>();
        services.AddSingleton<CommunityParticipationRecordedIntegrationEventMapper>();
        services.AddSingleton<CommunityParticipationRecordedIntegrationEventSerializer>();
        services.AddSingleton<ICommunityPersistenceIdentityGenerator,
            SystemCommunityPersistenceIdentityGenerator>();
        services.AddScoped<ICommunityParticipationCommitter,
            PostgreSqlCommunityParticipationCommitter>();
        services.AddScoped<IJoinCommunityService, JoinCommunityService>();

        return services;
    }
}
