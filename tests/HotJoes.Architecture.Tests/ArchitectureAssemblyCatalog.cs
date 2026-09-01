using System.Reflection;

namespace HotJoes.Architecture.Tests;

public sealed class ArchitectureAssemblyCatalog
{
    private ArchitectureAssemblyCatalog(
        IEnumerable<StructuralTypeDescriptor> types)
    {
        Types = types
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyList<StructuralTypeDescriptor> Types { get; }

    public static ArchitectureAssemblyCatalog LoadControlledAssemblies()
    {
        Assembly[] assemblies =
        [
            typeof(HotJoes.Api.Vendor.VendorApiErrorMapper).Assembly,
            typeof(HotJoes.Application.Address.IAddressResolutionService)
                .Assembly,
            typeof(HotJoes.Application.Vendor.RegisterVendorService)
                .Assembly,
            typeof(HotJoes.Domain.Vendor.Vendor).Assembly,
            typeof(HotJoes.Infrastructure.ComplianceConsumer
                .ComplianceDeliveryProcessor).Assembly,
            typeof(HotJoes.Infrastructure.Health
                .Epic1HealthEvaluator).Assembly,
            typeof(HotJoes.Infrastructure.Persistence
                .PostgreSqlVendorRepository).Assembly,
            typeof(HotJoes.Infrastructure.Vendor.Address
                .AddressResolutionAdapter).Assembly,
            typeof(HotJoes.Infrastructure.VendorRelay
                .VendorOutboxRelayRunner).Assembly
        ];

        StructuralTypeDescriptor[] types = assemblies
            .Distinct()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsNestedPrivate)
            .Select(StructuralTypeDescriptor.FromType)
            .ToArray();

        return new ArchitectureAssemblyCatalog(types);
    }

    public static ArchitectureAssemblyCatalog FromTypes(
        IEnumerable<StructuralTypeDescriptor> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new ArchitectureAssemblyCatalog(types);
    }
}
