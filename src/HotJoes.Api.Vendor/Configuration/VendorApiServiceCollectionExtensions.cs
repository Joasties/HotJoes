using HotJoes.Api.Vendor.AddressBootstrap;
using HotJoes.Application.Address;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;
using HotJoes.Infrastructure.Persistence;
using HotJoes.Infrastructure.Vendor.Address;
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

        services.AddDbContext<VendorRegistrationDbContext>(options =>
        {
            string connectionString = configuration.GetConnectionString(
                "VendorDatabase") ?? throw new InvalidOperationException(
                    "ConnectionStrings:VendorDatabase is required.");
            options.UseNpgsql(connectionString);
        });

        services.AddSingleton<IAddressResolutionService>(_ =>
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
        services.AddScoped<IAddressResolver, AddressResolutionAdapter>();

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

        return services;
    }
}
