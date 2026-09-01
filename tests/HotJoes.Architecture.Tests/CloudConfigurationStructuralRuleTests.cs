using HotJoes.Api.Vendor.Configuration;
using System.Reflection;

namespace HotJoes.Architecture.Tests;

public sealed class CloudConfigurationStructuralRuleTests
{
    [Fact]
    public void AI_CFG_001_ControlledProductionBoundary_HasNoViolations()
    {
        ArchitectureRepository repository =
            ArchitectureRepository.FindFromTestAssembly();
        ProjectDependencyGraph graph = repository.LoadProjectGraph();
        ArchitectureSourceCatalog sources =
            ArchitectureSourceCatalog.LoadProductionSources();

        IReadOnlyList<ArchitectureViolation> violations =
            CloudConfigurationStructuralRuleSet.Evaluate(graph, sources);

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData("Azure.Data.AppConfiguration")]
    [InlineData("Azure.Identity")]
    [InlineData("Azure.Security.KeyVault.Secrets")]
    [InlineData("Microsoft.Extensions.Configuration.AzureAppConfiguration")]
    public void AI_CFG_001_InnerProjectCloudProviderPackage_IsDetected(
        string packageReference)
    {
        ProjectDependencyGraph graph = Graph(
            Node(
                "HotJoes.Application.Vendor",
                packageReferences: [packageReference]));

        IReadOnlyList<ArchitectureViolation> violations =
            CloudConfigurationStructuralRuleSet.Evaluate(
                graph,
                Sources());

        AssertViolation(violations);
    }

    [Theory]
    [InlineData(
        "src/HotJoes.Domain.Vendor/CloudIdentity.cs",
        "using Azure.Identity;")]
    [InlineData(
        "src/HotJoes.Application.Vendor/SecretReference.cs",
        "using Azure.Security.KeyVault.Secrets;")]
    [InlineData(
        "src/HotJoes.Application.Address/ConfigurationReader.cs",
        "using Microsoft.Extensions.Configuration;")]
    public void AI_CFG_001_InnerSourceCloudOrConfigurationType_IsDetected(
        string relativePath,
        string content)
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(relativePath, content));

        IReadOnlyList<ArchitectureViolation> violations =
            CloudConfigurationStructuralRuleSet.Evaluate(
                Graph(),
                sources);

        AssertViolation(violations);
    }

    [Fact]
    public void AI_CFG_001_ArbitraryStringKeyOutsideCompositionBoundary_IsDetected()
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                "src/HotJoes.Api.Vendor/VendorEndpointMappings.cs",
                "string? endpoint = configuration[\"Address:Endpoint\"];"));

        IReadOnlyList<ArchitectureViolation> violations =
            CloudConfigurationStructuralRuleSet.Evaluate(
                Graph(),
                sources);

        AssertViolation(violations);
    }

    [Theory]
    [InlineData("src/HotJoes.Api.Vendor/Program.cs")]
    [InlineData(
        "src/HotJoes.Api.Vendor/Configuration/AzureReplicaAdapter.cs")]
    public void AI_CFG_001_StringKeyAtApprovedCompositionBoundary_IsPermitted(
        string relativePath)
    {
        ArchitectureSourceCatalog sources = Sources(
            new SourceFileDescriptor(
                relativePath,
                "string? endpoint = configuration[\"Address:Endpoint\"];"));

        IReadOnlyList<ArchitectureViolation> violations =
            CloudConfigurationStructuralRuleSet.Evaluate(
                Graph(),
                sources);

        Assert.Empty(violations);
    }

    [Fact]
    public void AI_CFG_001_VendorApiBootstrapBoundary_IsStronglyTypedAndCloudNeutral()
    {
        Type[] boundaryTypes =
        [
            typeof(IConfigurationSnapshotReplica<>),
            typeof(IConfigurationSnapshotValidator<>),
            typeof(IRequiredSecretResolver<>),
            typeof(IConfigurationSnapshotActivator<>),
            typeof(ConfigurationBootstrapResult<>),
            typeof(ConfigurationSnapshotBootstrapper<>)
        ];

        Assert.All(
            boundaryTypes,
            type =>
            {
                Assert.True(type.IsGenericTypeDefinition);
                Assert.Single(type.GetGenericArguments());
                Assert.Equal(
                    "HotJoes.Api.Vendor.Configuration",
                    type.Namespace);
            });
        Assert.DoesNotContain(
            boundaryTypes.SelectMany(PublicSignatureTypes),
            IsCloudProviderType);
    }

    private static ProjectDependencyGraph Graph(
        params ProjectDependencyNode[] nodes)
    {
        return new ProjectDependencyGraph(nodes);
    }

    private static ProjectDependencyNode Node(
        string name,
        string[]? projectReferences = null,
        string[]? packageReferences = null)
    {
        return new ProjectDependencyNode(
            name,
            projectPath: $"src/{name}/{name}.csproj",
            projectReferences: projectReferences ?? [],
            packageReferences: packageReferences ?? []);
    }

    private static ArchitectureSourceCatalog Sources(
        params SourceFileDescriptor[] files)
    {
        return ArchitectureSourceCatalog.FromFiles(files);
    }

    private static void AssertViolation(
        IReadOnlyList<ArchitectureViolation> violations)
    {
        Assert.Contains(
            violations,
            violation => violation.ObligationId == "AI-CFG-001");
    }

    private static IEnumerable<Type> PublicSignatureTypes(Type boundaryType)
    {
        const BindingFlags declaredPublicMembers =
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        foreach (ConstructorInfo constructor in
            boundaryType.GetConstructors(declaredPublicMembers))
        {
            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                yield return parameter.ParameterType;
            }
        }

        foreach (MethodInfo method in
            boundaryType.GetMethods(declaredPublicMembers))
        {
            yield return method.ReturnType;

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                yield return parameter.ParameterType;
            }
        }

        foreach (PropertyInfo property in
            boundaryType.GetProperties(declaredPublicMembers))
        {
            yield return property.PropertyType;
        }
    }

    private static bool IsCloudProviderType(Type type)
    {
        Type candidate = type;

        while (candidate.HasElementType)
        {
            candidate = candidate.GetElementType()!;
        }

        if (candidate.IsGenericParameter)
        {
            return false;
        }

        if (IsCloudProviderAssembly(candidate.Assembly.GetName().Name))
        {
            return true;
        }

        return candidate.IsGenericType &&
            candidate.GetGenericArguments().Any(IsCloudProviderType);
    }

    private static bool IsCloudProviderAssembly(string? assemblyName)
    {
        return assemblyName is not null &&
            (assemblyName.StartsWith("Azure.", StringComparison.Ordinal) ||
                assemblyName.Contains(
                    "AzureAppConfiguration",
                    StringComparison.Ordinal));
    }
}
